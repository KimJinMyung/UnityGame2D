using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Pool;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public enum PlayerState 
{
    Idle,
    Move,
    Crouch,
    NormalAiming,
    PickUping,
    Equiped,
    UnEquiped,
    BackSlide,
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
    public float MoveSpeed = 10f;
    public float MaxSpeed = 4f;

    public float CrouchHeiht = 0.7f;
    private Vector2 normalArmPos;

    private Vector2 mousePosition;
    private Vector2 InputRotation;
    private Vector2 PlayerRotation;

    private bool isHeadAiming;
    private bool isLegAiming;
    public LayerMask layer;
    private Monster target;

    private PlayerState state;

    private newInventory playerInventory;
    private Gun playerGun;

    private float Damage = 5;
    private float AttackDamage;

    //[SerializeField]
    //private GameObject magazine;
    private GameObject PickUpItemObject;

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        capsuleCollider = transform.GetChild(1).GetChild(0).GetComponent<CapsuleCollider2D>();
        bodyAnimator = transform.GetChild(0).GetComponent<Animator>();
        RightArmAimator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        LeftArmAimator = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Animator>();
        ArmsTransform = transform.GetChild(0).GetChild(0).transform;
        state = PlayerState.Idle;
        playerInventory = new newInventory();
        playerGun = new Gun();        
    }
        
    public void OnMove(InputAction.CallbackContext context)
    {        
        InputMoveDir = context.ReadValue<Vector2>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Crouch();
        }
    }

    public void OnLoock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            state = PlayerState.NormalAiming;
            RightArmAimator.SetBool("Aiming", true);
            LeftArmAimator.SetBool("Aiming", true);
        }

        if (context.performed)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            InputRotation = new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.GetChild(0).position.y, 0);
            Debug.DrawLine(new Vector3(transform.position.x, transform.GetChild(0).position.y, 0), InputRotation * 8f, Color.red);
            
            InputRotation.Normalize();
            float rotationZ = Mathf.Atan2(InputRotation.y, InputRotation.x) * Mathf.Rad2Deg;
            //ArmsTransform.rotation = Quaternion.Euler(0f,0f,rotationZ);

            if (InputRotation.x < 0)
            {
                PlayerRotation = Vector2.left;
                ArmsTransform.localRotation = Quaternion.Euler(180f,180f,-rotationZ);
            }
            else if (InputRotation.x > 0)
            {
                PlayerRotation = Vector2.right;
                ArmsTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            }            
        }        

        if (context.canceled)
        {
            state = PlayerState.Idle;
            RightArmAimator.SetBool("Aiming", false);
            LeftArmAimator.SetBool("Aiming", false);
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (state == PlayerState.PickUping || state == PlayerState.Die) return;
        if (context.performed)
        {
            if (!playerGun.equipedBullet) return;
            
            if(target != null)
            {
                target.Hurt(AttackDamage);                
            }
            //bullet ����
            playerGun.BackSlide();
        }
    }

    public void OnEquip(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            state = PlayerState.Equiped;
        }

        if(context.performed)
        {
            if (playerInventory.grip == null) return;
            if (playerGun.equipedMagazine != null) return;
            playerGun.Equip(playerInventory.grip);
            playerInventory.Equip();
        }

        if (context.canceled)
        {
            state = PlayerState.Idle;
        }
    }

    public void OnUnEquip(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            state = PlayerState.UnEquiped;
        }

        if (context.performed)
        {
            //�տ� źâ�� ������ �ش� źâ�� ���� ���´�.
            //�׷��� �ѿ� źâ�� �ִ� ��Ȳ���� �տ� źâ�� ������ ���� źâ�� �и��ϰ� ���� ���´�.
            //�տ� źâ�� ������ �����Ǿ� �ִ� ���� źâ�� �տ� ���.
            //��, �� ��� źâ�� ������ ����Ǵ� ���� ����.

            if (playerInventory.grip != null)
            {
                if (playerGun.equipedMagazine != null)
                {
                    ObjectPoolManager.Instance.Drop(playerGun.equipedMagazine,this.gameObject);
                    playerGun.UnEquip();
                    Debug.Log("playerGun�� źâ drop");
                }
                else
                {
                    ObjectPoolManager.Instance.Drop(playerInventory.grip, this.gameObject);
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

        if(context.canceled)
        {
            state = PlayerState.Idle;
        }
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        //�Ʒ��� ��ź�� ������ 3�ʵ��� Ȧ���Ͽ� �ݴ´�.
        //������ ��� ���� ���� ¤�� ��ٷ� �Ͼ��.
        //�÷��̾��� �տ� ��ź�� ������ ������.
        if (context.started)
        {
            if (playerInventory.grip != null)
            {
                ObjectPoolManager.Instance.Drop(playerInventory.grip, this.gameObject);
                playerInventory.Equip();
                return;
            }
            state = PlayerState.PickUping;
            bodyAnimator.SetBool("PickUp", true);
        }

        if (context.performed)
        {
            if(PickUpItemObject != null)
            {
                if(context.interaction is HoldInteraction)
                {
                    playerInventory.UnEquip(PickUpItemObject.GetComponent<Magazine>().bullet);
                    PickUpItemObject.GetComponent<Magazine>().DestoryMagazine();
                    Debug.Log("�ݱ� ����");
                }
                else if(context.interaction is PressInteraction)
                {
                    state = PlayerState.Idle;
                    bodyAnimator.SetBool("PickUp", false);
                    return;
                }
            }
            else
            {
                state = PlayerState.Idle;
                bodyAnimator.SetBool("PickUp", false);
                return;
            }
        }

        if (context.canceled)
        {
            state = PlayerState.Idle;
            bodyAnimator.SetBool("PickUp", false);
        }
    }

    public void OnBackSlide(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            state = PlayerState.BackSlide;
        }

        if (context.performed)
        {
            if (playerInventory.grip != null || playerGun.equipedMagazine == null) return;
            playerGun.BackSlide();
            Debug.Log("BackSlide");
            Debug.Log(playerGun.equipedMagazine.bulletCount);
        }

        if (context.canceled)
        {
            state = PlayerState.Idle;
        }
    }

    //private void Drop(Item item)
    //{
    //    Vector2 dropPos = transform.position;
    //    //GameObject newMagazine = Instantiate(magazine);
    //    var newMagazine = _pool.Get();
    //    newMagazine.GetComponent<Magazine>().SetMagazine(item, true);
    //    newMagazine.transform.position = dropPos;
    //}

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
            state = PlayerState.Idle;
        }
    }

    private void SpriteMove()
    {
        if(PlayerRotation.x * playerRigid.velocity.x > 0f)
        {
            bodyAnimator.SetBool("Forward", true);
        }else if (PlayerRotation.x * playerRigid.velocity.x < 0f)
        {
            bodyAnimator.SetBool("Forward", false);
        }
    }

    private void Move()
    {
        if (state == PlayerState.Die || state == PlayerState.Crouch || state == PlayerState.PickUping) return;
        if (InputMoveDir.x != 0 && playerRigid.velocity.x < MaxSpeed && playerRigid.velocity.x > -MaxSpeed)
        {
            playerRigid.AddForce(Vector2.right * InputMoveDir.x * MoveSpeed);
            state = PlayerState.Move;
            bodyAnimator.SetBool("Move", true);
        }
    }

    private void Aiming()
    {
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D[] hit = Physics2D.RaycastAll(new Vector2(startPos.x, startPos.y + 0.1f), PlayerRotation, 8f, layer);
        Debug.DrawRay(startPos, PlayerRotation * 8f, Color.green);
        Debug.Log(hit.Length);
        if (hit.Length != 0)
        {

            float closetDistance = float.MaxValue;
            foreach (var hit2 in hit)
            {
                Monster targets = hit2.transform.GetComponent<Monster>();
                if (targets != null)
                {
                    float distance = Mathf.Abs(Vector2.Distance(hit2.transform.position, mousePosition));
                    if (distance < closetDistance)
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
   

    private void FixedUpdate()
    {
        Move();
        SpriteRotate();
        SpriteMove();
        Aiming();
        AttackDamageDecide();
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
