using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public enum PlayerState 
{
    None,
    Idle,
    Crouch,
   // NormalAiming,
    PickUping,
    Attack,
    Equiped,
    UnEquiped,
    BackSlide,
    Hurt,
    Die
}

public class Player_Controller : MonoBehaviour
{
    private Rigidbody2D playerRigid;
    private CapsuleCollider2D capsuleCollider;
    private Animator bodyAnimator;
    private Animator RightArmAimator;
    private Animator LeftArmAimator;

    private Transform ArmsTransform;

    private Vector2 InputMoveDir;
    [SerializeField] private float MoveSpeed = 10f;
    [SerializeField] private float MaxSpeed = 4f;

    [SerializeField] private float CrouchHeiht = 0.7f;
   
    public bool isContactCover {  get; private set; }
    private LayerMask Cover;
    private Vector2 normalArmPos;

    private Vector2 mousePosition;
    private Vector2 InputRotation;
    private Vector2 PlayerRotation;

    private bool isHeadAiming;
    private bool isLegAiming;
    private bool isLightAiming;

    private bool isRunning;
    private bool isForward;

    private LayerMask layer;
    public Monster_LongRange Monster_target {  get; private set; }
    public MyLight Lights_target { get; private set; }

    public PlayerState state {  get; set; }
    private bool isBattle;
    public bool isWaveStart;

    private float lastAttackTime = 0f;

    public NewInventory playerInventory; //{  get; private set; }
    public Gun playerGun {  get; private set; }

    [SerializeField]
    private float Damage = 5;
    public float AttackDamage {  get; private set; }
    [SerializeField]
    public float condition {  get; private set; }
    [SerializeField]
    public float focus { get; private set; }
    private bool isHealing = false;

    //[SerializeField]
    //private GameObject magazine;
    private GameObject PickUpItemObject;

    public bool isLightUnder {  get; private set; }
    private float EvasionPer;
    private bool isEvasionPer_change = false;

    public bool isContectDepot = false;

    public bool isBleeding { get; private set; }

    public bool isGunMalfunction {get; private set; } 
    public bool isCheckingBullet {  get; private set; }

    private void Awake()
    {
        playerInventory = new NewInventory();
        playerGun = new Gun();

        isBleeding = false;
    }

    private void Start()
    {
        if(transform.CompareTag("Player")) 
        {
            playerRigid = GetComponent<Rigidbody2D>();
            capsuleCollider = transform.GetChild(1).GetChild(0).GetComponent<CapsuleCollider2D>();
            bodyAnimator = transform.GetChild(0).GetComponent<Animator>();
            RightArmAimator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
            LeftArmAimator = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Animator>();
            ArmsTransform = transform.GetChild(0).GetChild(0).transform;
            state = PlayerState.Idle;

            Game_UI_Manager.Instance.Init_Player_Grip();
            Game_UI_Manager.Instance.Init_Player_Inven();
        }       

        isBattle = false;
        isLightUnder = false;
        isWaveStart = false;
        isContactCover = false;

        layer = 64;
        Cover = 4096;
        EvasionPer = 10;
        isEvasionPer_change = false;

        isForward = true;

        if (!GameManager.Instance.isSaveGame)
        {
            //인벤토리와 grip null로 초기화
            playerInventory.NewGame_Init_Inventory();

            condition = 100;
            focus = 100;
        }
    }    
        
    public void OnMove(InputAction.CallbackContext context)
    {
        InputMoveDir = context.ReadValue<Vector2>();

        if (context.performed)
        {
            if (context.interaction is MultiTapInteraction)
            {
                if (!isRunning && isForward)
                {
                    StartCoroutine(Running());
                }
            }
        }
        
    }

    private IEnumerator Running()
    {
        if (!isRunning)
        {
            isRunning = true;
            bodyAnimator.SetBool("Running", true);
            yield return new WaitForSeconds(1f);
            bodyAnimator.SetBool("Running", false);

            yield return new WaitForSeconds(1f);
            isRunning = false;
            yield break;
        }
    }

    private void Run()
    {
        if (isRunning)
        {
            MaxSpeed = 6f;
            MoveSpeed = 15f;
        }
        else
        {
            MaxSpeed = 4f;
            MoveSpeed = 10f;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die) return;
        if (bodyAnimator.GetBool("PickUp")) return;
        if (context.performed)
        {
            if (isBleeding) isBleeding = false;            
            Crouch();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Die) return;
        if (context.performed)
        {
            RightArmAimator.SetBool("Aiming", true);
            LeftArmAimator.SetBool("Aiming", true);

            mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            InputRotation = new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.GetChild(0).position.y, 0);
            Debug.DrawLine(new Vector3(transform.position.x, transform.GetChild(0).position.y, 0), InputRotation * 8f, Color.red);

            InputRotation.Normalize();
            float rotationZ = Mathf.Atan2(InputRotation.y, InputRotation.x) * Mathf.Rad2Deg;
            //ArmsTransform.rotation = Quaternion.Euler(0f,0f,rotationZ);

            
            if (InputRotation.x < 0)
            {
                PlayerRotation = Vector2.left;
                ArmsTransform.localRotation = Quaternion.Euler(180f, 180f, -rotationZ);
            }
            else if (InputRotation.x > 0)
            {
                PlayerRotation = Vector2.right;
                ArmsTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            }

            //Decide_Target();
        }        

        if (context.canceled)
        {
            //state = PlayerState.Idle;
            RightArmAimator.SetBool("Aiming", false);
            LeftArmAimator.SetBool("Aiming", false);
        }
    }

    private void Decide_Target()
    {
        Transform target = null;        

        if (Lights_target != null)
        {
            target = Lights_target.transform;
        }
        else if (Monster_target != null)
        {
            target = Monster_target.transform;
        }

        if (target != null)
        {
            if (target.position.x - transform.position.x > 0f)
            {
                ArmsTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (target.position.x - transform.position.x < 0f)
            {
                ArmsTransform.localRotation = Quaternion.Euler(180f, 180f, 180f);
            }
        }     
    }

    public void OnHeadAiming(InputAction.CallbackContext context)
    {
        if (isLegAiming || isLightAiming) return;
        isHeadAiming = context.ReadValue<float>() > 0f;
    }

    public void OnLegAiming(InputAction.CallbackContext context)
    {
        if (isHeadAiming || isLightAiming) return;
        isLegAiming = context.ReadValue<float>() > 0f;
    }

    public void OnLightAiming(InputAction.CallbackContext context)
    {
        if (isHeadAiming || isLegAiming) return;
        isLightAiming = context.ReadValue<float>() > 0f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (state == PlayerState.PickUping || state == PlayerState.Die || state == PlayerState.Attack || state == PlayerState.Hurt) return;
            if (!playerGun.equipedBullet) return;
            if (isGunMalfunction) return;

            isBattle = true;
            isWaveStart = true;

            isRunning = false;
            bodyAnimator.SetBool("Running", false);

            lastAttackTime = Time.time;

            state = PlayerState.Attack;

            //bodyAnimator.SetTrigger("Attack");
            RightArmAimator.SetTrigger("Attack");
            LeftArmAimator.SetTrigger("Attack");
            //bodyAnimator.SetBool("Attack", true);
            //RightArmAimator.SetBool("Attack", true);
            //LeftArmAimator.SetBool("Attack", true);
            //

            //if (target != null)
            //{
            //    target.Hurt(AttackDamage);
            //    Debug.Log("공격 ");
            //}

            //bullet 감소
            //playerGun.BackSlide();

            if(Monster_target != null)
            {
                if (isHeadAiming) focus -= 15;
                else if (isLegAiming) focus -= 10;
                else { focus += 10; }
            }
            else if(Lights_target != null)
            {
                focus -= 15;
            }
            else
            {
                focus -= 10;
            }
            

            focus = Mathf.Clamp(focus, 0f, 100f);

            isGunMalfunction = Random.Range(0f, 100f) < 30f? true: false;
            Game_UI_Manager.Instance.BackSlide_Animation(isGunMalfunction, playerGun.equipedBullet);
        }        
    }    
    
    public void AttackBackSlide()
    {
        playerGun.BackSlide();
    }

    public void OnCheckedEquipedBullet(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die || state == PlayerState.Attack) return;
        if (state == PlayerState.BackSlide) return;
            isCheckingBullet = context.ReadValue<float>() > 0f;             
    }

    public void OnChangeItemSlot(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Die || state == PlayerState.Attack) return;
        if (context.performed)
        {
            //플레이어 손에 탄창이 있으면 누른 번호의 slot으로 이동
            //만약 플레이어 손에 탄창이 없고 해당 slot에 탄창이 있으면 가져온다.
            //둘다 있거나 없으면 실행하지 않는다.

            int index = (int)context.ReadValue<float>();
          
            playerInventory.ChangeMagazineSlot(index);
            
        }
    }

    public void OnEquip(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die || state == PlayerState.PickUping) return;
        if (context.started)
        {
            state = PlayerState.Equiped;
            
        }

        if(context.performed)
        {
            if (playerInventory.grip == null) return;
            if (playerGun.equipedMagazine != null) return;
            Game_UI_Manager.Instance.Drop_UI_Aimation();
            playerGun.Equip(playerInventory.grip);
            playerInventory.Equip();
            LeftArmAimator.SetBool("Equip", true);
            RightArmAimator.SetBool("Equip", true);
        }

        if (context.canceled)
        {
            state = PlayerState.Idle;
            LeftArmAimator.SetBool("Equip", false);
            RightArmAimator.SetBool("Equip", false);
        }
    }

    public void OnUnEquip(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die || state == PlayerState.Attack) return;
        if (context.started)
        {
            state = PlayerState.UnEquiped;
        }

        if (context.performed)
        {
            //손에 탄창이 있으면 해당 탄창을 내려 놓는다.
            //그러나 총에 탄창이 있는 상황에서 손에 탄창이 있으면 총의 탄창을 분리하고 내려 놓는다.
            //손에 탄창이 없으면 장착되어 있는 총의 탄창을 손에 쥔다.
            //손, 총 모두 탄창이 없으면 실행되는 것이 없다.

            if (playerInventory.grip != null)
            {
                if (playerGun.equipedMagazine != null)
                {
                    if(playerGun.equipedMagazine.bulletCount > 0)
                    {
                        ObjectPoolManager.Instance.Drop(playerGun.equipedMagazine, this.gameObject);
                    }                    
                    playerGun.UnEquip();
                }
                else
                {
                    if(playerInventory.grip.bulletCount > 0)
                    {
                        ObjectPoolManager.Instance.Drop(playerInventory.grip, this.gameObject);
                    }                
                    playerInventory.Equip();
                    //GameManager.Instance.Print_Player_Grip_Ainimation();
                    //GameManager.Instance.Drop_UI_Aimation();
                }
            }
            else
            {
                if (playerGun.equipedMagazine == null) return;
                playerInventory.UnEquip(playerGun.equipedMagazine);
                playerGun.UnEquip();
                Game_UI_Manager.Instance.Print_Player_Grip_Ainimation();
            }
        }

        if(context.canceled)
        {
            state = PlayerState.Idle;
        }
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die) return;
        //아래에 총탄이 있으면 3초동안 홀드하여 줍는다.
        //없으면 잠시 멈춰 땅을 짚고 곧바로 일어선다.
        //플레이어의 손에 총탄이 있으면 버린다.
        if (context.started)
        {
            if (playerInventory.grip != null)
            {
                if(playerInventory.grip.bulletCount > 0)
                    ObjectPoolManager.Instance.Drop(playerInventory.grip, this.gameObject);
                playerInventory.Equip();
                return;
            }
            playerRigid.velocity = Vector3.zero;
            state = PlayerState.PickUping;            
        }

        if (context.performed)
        {
            if (playerInventory.grip != null) return;            
            if (PickUpItemObject != null)
            {
                if(context.interaction is HoldInteraction)
                {
                    playerInventory.UnEquip(PickUpItemObject.GetComponent<Magazine>().bullet);
                    PickUpItemObject.GetComponent<Magazine>().DestoryMagazine();

                    bodyAnimator.SetTrigger("PickUping");
                    //Game_UI_Manager.Instance.Print_Player_Grip_Ainimation();

                    state = PlayerState.Idle;
                }else if(context.interaction is PressInteraction)
                {
                    state = PlayerState.Idle;
                    bodyAnimator.SetBool("PickUp", false);
                    return;
                }
                
            }
            else
            {
                if (context.interaction is HoldInteraction)
                {
                    ReCharge_Inventory_Magazine();
                }
                
                state = PlayerState.Idle;
                bodyAnimator.SetBool("PickUp", false);
                return;
            }
        }

        if (context.canceled)
        {
            state = PlayerState.Idle;
        }
    }

    private void PickUpAimation()
    {
        if (state == PlayerState.PickUping)
        {
            bodyAnimator.SetBool("PickUp", true);
            bodyAnimator.SetBool("Move", false);
            bodyAnimator.SetBool("Crouch", false);
            
        }
        //else
        //{
        //    bodyAnimator.SetBool("PickUp", false);
        //}

        if (playerInventory.grip != null)
        {
            LeftArmAimator.SetBool("PickUp", true);
        }
        else
        {
            LeftArmAimator.SetBool("PickUp", false);
        }
    }

    void Finish() 
    {
        LeftArmAimator.gameObject.SetActive(true);
        RightArmAimator.gameObject.SetActive(true);
    }
    public void OnBackSlide(InputAction.CallbackContext context)
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die) return;
        if (context.started)
        {
            state = PlayerState.BackSlide;
        }

        if (context.performed)
        {
            if (playerInventory.grip != null) return;

            if (isGunMalfunction)
            {
                isGunMalfunction = false;
            }

            Game_UI_Manager.Instance.BackSlide_Animation(isGunMalfunction, playerGun.equipedBullet);

            if (playerGun.equipedMagazine != null)
            {
                playerGun.BackSlide();
            }            

            LeftArmAimator.SetTrigger("BackSlide");
            RightArmAimator.SetTrigger("BackSlide");            
        }

        if (context.canceled)
        {
            state = PlayerState.Idle;
        }
    }

    private void AttackDamageDecide()
    {
        if (isHeadAiming)
        {
            AttackDamage = Damage * 3f;
        }else if (isLegAiming)
        {
            AttackDamage = Damage * 0.5f;
        }
        else
        {
            AttackDamage = Damage;
        }
    }

    private void Crouch()
    {
        if (state == PlayerState.Hurt || state == PlayerState.Die) return;
        if(state != PlayerState.Crouch) 
        { 
            state = PlayerState.Crouch;
            bodyAnimator.SetBool("Crouch", true);
            playerRigid.velocity = new Vector2(0, playerRigid.velocity.y);
            capsuleCollider.transform.parent.localScale = new Vector2(capsuleCollider.transform.localScale.x, CrouchHeiht);
            capsuleCollider.transform.parent.localPosition = new Vector2(0.16f, 0);
            normalArmPos = ArmsTransform.position;
            ArmsTransform.localPosition = Vector2.zero;
        }
        else if(state == PlayerState.Crouch) 
        {
            state = PlayerState.Idle;
            bodyAnimator.SetBool("Crouch", false);
            capsuleCollider.transform.parent.localScale = new Vector2(capsuleCollider.transform.localScale.x, 1);
            capsuleCollider.transform.parent.localPosition = new Vector2(0, 0);
            ArmsTransform.position = normalArmPos;
        }
    }

    private void SpriteRotate()
    {
        if(PlayerRotation.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if(PlayerRotation.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (state == PlayerState.Die || state == PlayerState.Crouch) return;
        if (InputMoveDir == Vector2.zero)
        {
            playerRigid.velocity = new Vector2(0, playerRigid.velocity.y);
            bodyAnimator.SetBool("Move", false);
            //state = PlayerState.Idle;
        }
    }

    private void SpriteMove()
    {
        if(PlayerRotation.x * playerRigid.velocity.x > 0f)
        {
            isForward = true;
            bodyAnimator.SetBool("Forward", true);
        }else if (PlayerRotation.x * playerRigid.velocity.x < 0f)
        {
            isForward = false;
            bodyAnimator.SetBool("Forward", false);
        }
    }

    private void Move()
    {
        if (state == PlayerState.Die || bodyAnimator.GetBool("PickUp") || state == PlayerState.PickUping || bodyAnimator.GetBool("Crouch")) return;
        if (InputMoveDir.x != 0 && playerRigid.velocity.x < MaxSpeed && playerRigid.velocity.x > -MaxSpeed)
        {            
            playerRigid.AddForce(Vector2.right * InputMoveDir.x * MoveSpeed);
            bodyAnimator.SetBool("Move", true);
            //Move_Requirement();            
        }
    }

    private void Move_Requirement()
    {
        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);
        if(worldpos.x < 0f)
        {
            worldpos.x = 0f;
            playerRigid.velocity = Vector2.zero;
            bodyAnimator.SetBool("Move", false);
        }
        else if (worldpos.x > 1f)
        {
            worldpos.x = 1f;
            playerRigid.velocity = Vector2.zero;
            bodyAnimator.SetBool("Move", false);
        }
            this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }

    private void Decide_Aiming_Target_Layer()
    {
        if (isLightAiming)
        {
            layer = 2048;
        }
        else
        {
            layer = 64;
        }
    }

    private void LightsAiming(RaycastHit2D[] hit)
    {
        if (Monster_target != null)
        {
            Game_UI_Manager.Instance.Default_MonsterAiming_Color();
            Monster_target.SetTargeted();
            Monster_target = null;
        }
        
        if (hit.Length != 0)
        {
            float closetDistance = float.MaxValue;
            foreach (var hit2 in hit)
            {
                MyLight targets = hit2.transform.GetComponent<MyLight>();

                if (targets != null)
                {
                    float distance = Mathf.Abs(Vector2.Distance(hit2.transform.position, mousePosition));
                    if (distance < closetDistance)
                    {
                        closetDistance = distance;
                        if (Lights_target != null)
                        {
                            Lights_target.OffTargeting();
                        }
                        Lights_target = targets;
                    }
                }
            }
            if (Lights_target != null)
            {
                Lights_target.OnTargeting();
            }
        }
        else
        {
            if (Lights_target != null)
            {
                Lights_target.OffTargeting();
            }
            Lights_target = null;
        }
    }

    private void MonsterAiming(RaycastHit2D[] hit)
    {
        if (Lights_target != null)
        {
            Game_UI_Manager.Instance.Default_LightAiming_Color();
            Lights_target.OffTargeting();
            Lights_target = null;
        }
        if (hit.Length != 0)
        {
            Game_UI_Manager.Instance.Change_MonsterAiming_Color();

            float closetDistance = float.MaxValue;
            foreach (var hit2 in hit)
            {
                Monster_LongRange targets = hit2.transform.GetComponent<Monster_LongRange>();

                if (targets != null)
                {
                    float distance = Mathf.Abs(Vector2.Distance(hit2.transform.position, mousePosition));
                    if (distance < closetDistance)
                    {
                        closetDistance = distance;
                        if (Monster_target != null)
                        {
                            Monster_target.SetTargeted();
                        }
                        Monster_target = targets;
                    }
                }
            }
            if (Monster_target != null)
            {
                Monster_target.SetTargeted(isHeadAiming, isLegAiming);
            }
        }
        else
        {
            if (Monster_target != null)
            {
                Monster_target.SetTargeted();
            }
            Monster_target = null;
        }
    }

    private void Aiming()
    {
        Decide_Aiming_Target_Layer();
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D[] hit = Physics2D.RaycastAll(new Vector2(startPos.x, startPos.y + 0.1f), PlayerRotation, 8f, layer);
        Debug.DrawRay(startPos, PlayerRotation * 8f, Color.green);

        if (!isLightAiming)
        {
            MonsterAiming(hit);
        }
        else
        {
            LightsAiming(hit);
        }
        
    }
   
    private void PrintSlots()
    {
        if(playerInventory.grip != null)        
        {
            Game_UI_Manager.Instance.Print_Player_Grip(playerInventory.grip);           
        }
    }

    public void Hurt(float ATKdamage, Transform attackMonster)
    {
        if (condition <= 0f) return;
        if(Random.Range(0,100f)  >= EvasionPer) 
        {
            if(bodyAnimator.GetBool("Crouch"))
            {
                bodyAnimator.SetBool("Crouch", false);
                capsuleCollider.transform.parent.localScale = new Vector2(capsuleCollider.transform.localScale.x, 1);
                capsuleCollider.transform.parent.localPosition = new Vector2(0, 0);
                ArmsTransform.position = normalArmPos;
            }

            state = PlayerState.Hurt;
            playerRigid.velocity = Vector3.zero;

            isRunning = false;
            bodyAnimator.SetBool("Running", false);

            if (PlayerRotation.x * (transform.position.x - attackMonster.position.x) > 0f)
            {
                condition = 0;
            }
            else
            {
                condition -= ATKdamage;
                isBleeding = true;
                StartCoroutine(Bleeding());
            }

            if (condition <= 0)
            {
                Dead();
            }
            else  bodyAnimator.SetTrigger("Hurt");
        }        
    }

    private IEnumerator Bleeding()
    {
        while (isBleeding)
        {
            // 주기적으로 피해를 입히는 간격(예: 1초)을 기다립니다.
            yield return new WaitForSeconds(1f);

            // 피해를 입히는 부분
            condition -= 5f;

            if (condition <= 0) Dead();
            if (state == PlayerState.Die) yield break;
        }

        yield break;
    }

    private void Dead()
    {
        isWaveStart = false;
        state = PlayerState.Die;
        playerRigid.velocity = Vector3.zero;
        
        //사망 애니메이션 실행
        bodyAnimator.SetTrigger("Dead");

        //1.5초 후에 해당 라운드 재시작
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.GameLoad();
        if (GameManager.Instance.SaveSceneName != null)
            Loading_Bar_Controller.LoadScene(GameManager.Instance.SaveSceneName);
        else
            Loading_Bar_Controller.LoadScene(SceneManager.GetActiveScene().name);
        yield break;
    }

    private void EvasionPer_Up(float value)
    {
        if (!isEvasionPer_change)
        {
            EvasionPer += value;
            isEvasionPer_change = true;
        }         
    }

    private void EvasionPer_Down(float value)
    {
        if (isEvasionPer_change)
        {
            EvasionPer -= value;
            isEvasionPer_change = false;
        }

    }

    private void FixedUpdate()
    {
        if (state == PlayerState.None) return;        
        Move();        
        SpriteRotate();
        SpriteMove();
        Run();
        //Decide_Target();

        Decide_Target();
        ContactCover();
        Aiming();
        AttackDamageDecide();
        PickUpAimation();

        //PrintSlots();

        playerInventory.Print_Inventory_Slot();
        BattleModeEnd();

        CheckBullet();
    }

    private void ContactCover()
    {        
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.1f, 0), PlayerRotation, 2.5f, Cover);
        if(hit.transform != null)
        {
            isContactCover = true;
            if (state == PlayerState.Crouch)
            {
                if(hit.transform.position.x - transform.position.x > 0f)
                {
                    transform.position = this.transform.position + new Vector3(hit.distance - 1.4f, 0f, 0f);
                }
                else if(hit.transform.position.x - transform.position.x < 0f)
                {
                    transform.position = this.transform.position - new Vector3(hit.distance - 1.4f, 0f, 0f);
                }
                
            }
        }
        else
        {
            isContactCover = false;
        }
    }       
    
    public void ReCharge_Inventory_Magazine()
    {
        if (isContectDepot)
        {
            playerInventory.RechargeAll();
            for(int i  = 1; i < 5; i++)
            {
                Game_UI_Manager.Instance.Print_Player_InvenSlot_Up_Animation(i);
            }            
        }
    }

    private void BattleModeEnd()
    {
        if (isBattle && Time.time - lastAttackTime >= 3f)
        {
            isBattle = false;            
        }        

        if (!isBattle && !isHealing)
        {
            StartCoroutine(RecoveryFocus());
        }               
    }

    private IEnumerator RecoveryFocus()
    {
        isHealing = true;

        focus = Mathf.Clamp(focus + 5, 0f, 100f);
        yield return new WaitForSeconds(3f);

        isHealing = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Magazine"))
        {
            PickUpItemObject = collision.transform.parent.gameObject;
            Game_UI_Manager.Instance.isContactMagazine = true;
        }

        if (collision.gameObject.layer == 10)
        {
            isLightUnder = true;
            EvasionPer_Down(40);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magazine"))
        {
            PickUpItemObject = null;
            Game_UI_Manager.Instance.isContactMagazine = false;
        }

        if(collision.gameObject.layer == 10)
        {
            isLightUnder = false;
            EvasionPer_Up(40);
        }
    }

    public void LoadData(float Condition, int gripBullet, int[] inven_Bullet, int EquipedMagazine, bool EquipedBullet)
    {
        this.condition = Condition;       

        playerInventory.Load_Grip(gripBullet);

        playerInventory.Load_Inven(inven_Bullet);

        playerGun.Load_Gun(EquipedMagazine, EquipedBullet);
    }  

    private void CheckBullet()
    {
        if (isCheckingBullet)
        {
            Game_UI_Manager.Instance.Check_EquipedBullet_Animation(playerGun.equipedBullet);
        }
        else
        {
            Game_UI_Manager.Instance.End_Check_EquipedBullet_Animation();
        }
    }
}
