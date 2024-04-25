using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monster_ShortRange : Monster
{
    private Animator monster_Animation;

    //protected void Awake()
    //{

    //}  

    protected void Start()
    {
        monster_Animation = GetComponent<Animator>();

        Aiming = new SpriteRenderer[3];
        Aiming[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Aiming[1] = transform.GetChild(1).GetComponent<SpriteRenderer>();
        Aiming[2] = transform.GetChild(2).GetComponent<SpriteRenderer>();

        OffTargeting();

        monster_Rigid = GetComponent<Rigidbody2D>();

        Monster_Type_ID = 1;

        //if (_pool == null)
        //{
        //    _pool = Monster_Spawner.instance.MonsterPool;
        //}

        Target_Player = GameManager.Instance.GetPlayer;
        LayerMask = 512;
        AttackLayerMask = 4608;
        EvasionPer = 0;

        isDontMove = false;

        SetMonster(12f, 100f, 1.5f, false);
    }

    //private void OnEnable()
    //{
    //    SetMonster(12f, 100f, 1.5f, false);
    //}

    public override void SetMonster(float HP, float ATK, float AttackRange, bool isDead)
    {
        this.Hp = HP;
        this.ATK = ATK;
        this.isDead = isDead;
        this.isEvasionPer_change = false;
        this.AttackRange = AttackRange;
        this.SightRange = 8f;

        monster_Animation.SetBool("Down", false);
        HitBox_Position();
        isDontMove = false;
        monster_Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.layer = 6;
    }

    private void FixedUpdate()
    {
        Monster_Move();
        Raycast_Dir();
        Look();
        Debug.DrawLine(transform.position, (Vector2)transform.position + RaycastDir * SightRange, Color.red);
    }

    protected override void HitBox_Position()
    {
        if (monster_Animation.GetBool("Down"))
        {
            transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.63f, -0.38f, 0);
            transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.32f, -0.68f, 0);
            transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.32f, -0.77f, 0);
        }
        else
        {
            transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0f, 0.56f, 0);
            transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0f, -0.04f, 0);
            transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0f, -0.6f, 0);
        }
    }

    protected override void State_DownMonster()
    {
        monster_Animation.SetBool("Down", true);
    }

    protected override void Monster_HurtAnimation()
    {
        monster_Animation.SetTrigger("Hurt");
    }
    protected override void Monster_DeadAnimation()
    {
        monster_Animation.SetTrigger("Dead");
    }
    protected override void Monster_MoveAnimation()
    {
        if(MoveDir != Vector2.zero)
        {
            monster_Animation.SetBool("Move", true);
        }
        else
        {
            monster_Animation.SetBool("Move", false);
        }
    }

    protected override void Attack_Target()
    {
        if (isDontMove) return;

        Attack_Animation();
        //Vector2 startPoint = (transform.position + new Vector3(0, 0.2f, 0));
        //Vector2 RandomAngle = new Vector2(RaycastDir.x, 0);
        //RaycastHit2D attackTarget = Physics2D.Raycast(startPoint, RandomAngle, 1.2f, AttackLayerMask);
        //Debug.DrawLine(startPoint, startPoint + RandomAngle * 1.2f, Color.black);

        //if (attackTarget.transform != null)
        //{
        //    if (attackTarget.transform.gameObject.layer == 9)
        //    {
        //        target = attackTarget.transform.gameObject;
        //    }
        //}
        //else
        //{
        //    target = null;
        //}
    }

    private void Attack_Animation()
    {
        monster_Animation.SetTrigger("Attack");
    }

    
}
