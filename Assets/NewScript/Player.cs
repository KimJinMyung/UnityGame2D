using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Interactions;

public class Player : MonoBehaviour
{
    private Vector2 InputMovement = Vector2.zero;
    private Vector2 playerMovement = Vector2.zero;
    private Vector2 InputRotation = Vector2.zero;
    private Vector2 mousePosition = Vector2.zero;
    private Vector2 PlayerRotation = Vector2.zero;

    [SerializeField] private float MoveSpeed = 10.0f;
    [SerializeField] private float damage = 3;
    private float AttackDamage;

    [SerializeField] private LayerMask layer;

    private Monster target;

    [SerializeField] private bool isCrouch = false;
    [SerializeField] private bool isHeadAiming = false;
    [SerializeField] private bool isLegAiming = false;
    [SerializeField] private bool isCheckingReloadBullet = false;
    [SerializeField] private bool isPickUping = false;
    [SerializeField] private bool isBleeding = false;
    [SerializeField] private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private GameObject PlayerCollider;
    private Vector2 normalHeight;
    private Sprite normalSprite;
    public Sprite changeSprite;
    private Rigidbody2D playerRigid;

    public GameObject magazine;
    public Vector2 dropPos;

    private GameObject PickUpItemObject;

    public float CrouchHeiht = 0.7f;

    private newInventory playerInventory;
    private Gun playerGun;
    private float HP = 100f;

    Vector2 MOVEvec;

    public float PlayerHP 
    { 
        set {  HP = value; }
        get { return HP; }
    }

    private void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<newInventory>();
        playerGun = new Gun();
    }

    private void Start()
    {
        PlayerCollider = transform.GetChild(1).gameObject;
        spriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        normalHeight = PlayerCollider.transform.localScale;
        normalSprite = spriteRenderer.sprite;
    }

    private void Update()
    {
        Vector2 movement = InputMovement * MoveSpeed * Time.deltaTime;
        transform.position = (Vector2)transform.position + movement;

        CharacterRotate();
        CheckedEquipedBullet();
        Crouching();
        PickUping();
    }

    public void Hurt(float damage)
    {
        Debug.Log($"{damage}��ŭ�� ������ �޾ҽ��ϴ�.");
        HP -= damage;

        if (!isBleeding)
        {
            isBleeding = true;
            StartCoroutine(Bleeding());
        }
        if (HP <= 0) isDead = true;
    }

    private void Crouching()
    {
        if (isCrouch)
        {
            InputMovement = new Vector2(0, 0);
            PlayerCollider.transform.localScale = new Vector2(PlayerCollider.transform.localScale.x, CrouchHeiht);
        }
        else
        {
            InputMovement = playerMovement;
            PlayerCollider.transform.localScale = normalHeight;
            
        }
    }

    private void PickUping()
    {
        if (isPickUping)
        {
            isCrouch = false;
            InputMovement = Vector2.zero;
        }        
    }

    private IEnumerator Bleeding()
    {
        while (!isDead)
        {
            // �ֱ������� ���ظ� ������ ����(��: 1��)�� ��ٸ��ϴ�.
            yield return new WaitForSeconds(2f);

            // ���ظ� ������ �κ�
            Hurt(1);
            Debug.Log("Bleeding...");
        }
    }

    private void Die()
    {
        //�״� ���

        
    }

    public void OnMoving(InputAction.CallbackContext context)
    {
        InputMovement = context.ReadValue<Vector2>();
        playerMovement = InputMovement;
        if (isCrouch || isDead || isPickUping) 
        {
            InputMovement = Vector2.zero;
        }      
        
        
        //Debug.Log(InputMovement.x);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetCrouch();
        }        
    }

    private void SetCrouch()
    {
        if (!isCrouch) isCrouch = true;
        else isCrouch = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        InputRotation = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        Debug.DrawLine(transform.position, InputRotation * 8f, Color.red);
        if (mousePosition.x < transform.position.x)
        {
            PlayerRotation = Vector2.left;
        }
        else if (mousePosition.x > transform.position.x)
        {
            PlayerRotation = Vector2.right;
        }
    }

    public void OnHeadAiming(InputAction.CallbackContext context)
    {
        if (isLegAiming) return;
        isHeadAiming = context.ReadValue<float>() > 0f;
    }

    public void OnLegAiming(InputAction.CallbackContext context)
    {
        if (isHeadAiming) return;
        isLegAiming = context.ReadValue<float>() > 0f;
    }

    public void OnEquip(InputAction.CallbackContext context) // : E
    {
        if (playerGun.equipedMagazine != null) return;
        if(playerInventory.grip == null) return;
        if (context.performed)
        {
            playerGun.Equip(playerInventory.grip);
            playerInventory.Equip();
            Debug.Log("������ ����");
            Debug.Log(playerGun.equipedMagazine.bulletCount);
        }
        
    }

    public void OnUnEquip(InputAction.CallbackContext context)  // : R
    {        
        if(context.performed)
        {
            //�տ� źâ�� ������ �ش� źâ�� ���� ���´�.
            //�׷��� �ѿ� źâ�� �ִ� ��Ȳ���� �տ� źâ�� ������ ���� źâ�� �и��ϰ� ���� ���´�.
            // �տ� źâ�� ������ �����Ǿ� �ִ� ���� źâ�� �տ� ���.
            //��, �� ��� źâ�� ������ ����Ǵ� ���� ����.

            if(playerInventory.grip != null)
            {
                if(playerGun.equipedMagazine != null)
                {
                    Drop(playerGun.equipedMagazine);
                    playerGun.UnEquip();
                    Debug.Log("playerGun�� źâ drop");
                }
                else
                {
                    Drop(playerInventory.grip);
                    playerInventory.Equip();
                    Debug.Log("player grip�� źâ drop");
                }
            }
            else
            {
                if (playerGun.equipedMagazine == null) return;
                playerInventory.UnEquip(playerGun.equipedMagazine);
                playerGun.UnEquip();
                Debug.Log("źâ ����");
            }            
        }
    }
    public void OnAddMagazine(InputAction.CallbackContext context)  // : Q
    {
        //�Ʒ��� ��ź�� ������ 3�ʵ��� Ȧ���Ͽ� �ݴ´�.
        //������ ��� ���� ���� ¤�� ��ٷ� �Ͼ��.
        //�÷��̾��� �տ� ��ź�� ������ ������.
        if (context.started)
        {
            if (playerInventory.grip != null)
            {
                Drop(playerInventory.grip);
                playerInventory.Equip();                
            }
            
            isPickUping = true;
        }

        if (context.performed)
        {
            if (playerInventory.grip != null) return;

            if (PickUpItemObject != null)
            {
                if (context.interaction is HoldInteraction)
                {
                    playerInventory.UnEquip(PickUpItemObject.GetComponent<Magazine>().bullet);
                    PickUpItemObject.SetActive(false);
                }
                else if (context.interaction is PressInteraction)
                {
                    isPickUping = false;
                    return;
                }
            }
            else
            {
                isPickUping = false;
                return;
            }
        }
        

        if (context.canceled)
        {
            isPickUping = false;
        }


        //if (context.started)
        //{
        //    isPickUping = true;
        //}

        //if (context.performed)
        //{
        //    if (PickUpItemObject == null || playerInventory.grip != null)
        //    {
        //        isPickUping = false;
        //        return;
        //    }
        //    if (PickUpItemObject.GetComponent<Magazine>().isPickUped == false)
        //    {
        //        if (context.interaction is HoldInteraction)
        //        {
        //            playerInventory.UnEquip(PickUpItemObject.GetComponent<Magazine>().bullet);
        //            PickUpItemObject.SetActive(false);
        //            Debug.Log("������ ȹ��");
        //        }
        //        else if (context.interaction is PressInteraction)
        //        {
        //            isPickUping = false;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (context.interaction is PressInteraction)
        //        {
        //            grip = PickUpItemObject.GetComponent<Magazine>().bullet;
        //            PickUpItemObject.SetActive(false);
        //            Debug.Log("������ ȹ��");
        //        }
        //    }
        //}

        //if (context.canceled)
        //{
        //    isPickUping = false;
        //}


    }

    private void Drop(Item item)
    {
        dropPos = transform.position;
        GameObject newMagazine = Instantiate(magazine);
        newMagazine.GetComponent<Magazine>().SetMagazine(item, true);
        newMagazine.transform.position = dropPos;
    }

    public void OnBackSlide(InputAction.CallbackContext context) // : E + T
    {
        if(context.performed)
        {
            if (playerInventory.grip != null || playerGun.equipedMagazine == null) return;
            playerGun.BackSlide();
            Debug.Log("BackSlide");
            Debug.Log(playerGun.equipedMagazine.bulletCount);
        }
    }

    public void OnCheckEquipedBullet(InputAction.CallbackContext context) // : T
    {
        if (context.performed)
        {
             isCheckingReloadBullet = context.ReadValue<float>() > 0f;  
        }        
    }

    private void CheckedEquipedBullet()
    {
        if (isCheckingReloadBullet)
        {
            Debug.Log("��� Ȯ�� : "+playerGun.ChedkEquipedBullet());
        }        
    }

    //public void OnAttack(InputAction.CallbackContext context)
    //{   
    //    if (context.performed)
    //    {
    //        if (playerInventory.grip != null || playerGun.equipedMagazine == null || playerGun.equipedBullet == false) return;
    //        Debug.Log("Attack!!");

    //        AttackDamageDecide();
    //        playerGun.BackSlide();
            
    //        if (target != null)
    //        {
    //            Debug.Log(target.gameObject);
    //            target.HitParts();
    //            target.GetComponent<Monster>().Hurt(AttackDamage);
    //            Debug.Log(playerGun.equipedMagazine.bulletCount);
    //        }
    //        else
    //        {
    //            Debug.Log(playerGun.equipedMagazine.bulletCount);
    //            return;
    //        }

            
    //    }        
    //}

    private void AttackDamageDecide()
    {
        if (isHeadAiming)
        {
            AttackDamage = damage * 2f;
        }
        else if (isLegAiming)
        {
            AttackDamage = damage * 0.5f;
        }
        else
        {
            AttackDamage = damage;
        }
    }    

    void FixedUpdate()
    {
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D[] hit = Physics2D.RaycastAll(startPos, PlayerRotation, 8f, layer);
        Debug.DrawRay(startPos, PlayerRotation * 8f, Color.green);

        if(hit.Length != 0)
        {
            float closetDistance = float.MaxValue;
            foreach(var hit2 in hit)
            {
                Monster targets = hit2.transform.GetComponent<Monster>();
                if(targets != null)
                {
                    float distance = Mathf.Abs(Vector2.Distance(hit2.transform.position, mousePosition));
                    if(distance < closetDistance)
                    {
                        closetDistance = distance;
                        if (target != null)
                        {
                            target.SetTargeted();
                        }
                        target = targets;
                    }
                }
            }
            if (target != null)
            {
                target.SetTargeted(isHeadAiming, isLegAiming);
            }
        }
        else
        {
            if (target != null)
            {
                target.SetTargeted();
            }
            target = null;
        }
    }

    private void CharacterRotate()
    {
        if (InputRotation.x > 0f)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (InputRotation.x < 0f)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Magazine"))
        {
            PickUpItemObject = collision.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magazine"))
        {
            PickUpItemObject = null;
        }
    }
}
