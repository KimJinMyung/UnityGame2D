using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public enum PlayerState 
{
    Idle,
    Move,
    Crouch,
    NormalAiming,
    precisionAiming,
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

    private void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        capsuleCollider = transform.GetChild(1).GetChild(0).GetComponent<CapsuleCollider2D>();
        bodyAnimator = transform.GetChild(0).GetComponent<Animator>();
        RightArmAimator = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        LeftArmAimator = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Animator>();
        ArmsTransform = transform.GetChild(0).GetChild(0).transform;
        state = PlayerState.Idle;
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
        if (state == PlayerState.Die || state == PlayerState.Crouch) return;
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
    }

}
