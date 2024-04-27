using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
//using static UnityEditor.Progress;
//using static UnityEngine.GraphicsBuffer;

enum State
{
    Idle,
    Attack,
    Down,
    Dead
}

public class Monster_LongRange : Monster
{

    private Gun MonsterGun;

    [SerializeField]
    private GameObject dropMagazine;

    private Animator monster_Animation;


    private void Awake()
    {
        monster_Rigid= GetComponent<Rigidbody2D>();
        Aiming = new SpriteRenderer[3];

        monster_Animation = GetComponent<Animator>();
    }

    private void Start()
    {
        Aiming[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Aiming[1] = transform.GetChild(1).GetComponent<SpriteRenderer>();
        Aiming[2] = transform.GetChild(2).GetComponent<SpriteRenderer>();

        //if (_pool == null)
        //{
        //    _pool = Monster_Spawner.instance.MonsterPool;
        //}

        OffTargeting();

        Target_Player = GameManager.Instance.GetPlayer;

        LayerMask = 512;
        AttackLayerMask = 4608;
        EvasionPer = 0;

        isDontMove = false;

        MonsterGun = new Gun();
        Reload();

        SetMonster(10f, 30f, 5.5f, false);
        Monster_Type_ID = 2;
    }

    private void FixedUpdate()
    {        
        Monster_Move();
        Raycast_Dir();
        Look();
    }

    public override void SetMonster(float HP, float ATK, float AttackRange, bool isDead)
    {
        this.Hp = HP;
        this.ATK = ATK;
        this.isDead = isDead;
        this.AttackRange = AttackRange;
        SightRange = AttackRange + 1f;
        this.isDead = isDead;
        isEvasionPer_change = false;        

        monster_Animation.SetBool("Down", false);
        HitBox_Position();
        isDontMove = false;
        monster_Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.layer = 6;
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
        if (MoveDir != Vector2.zero)
        {
            monster_Animation.SetBool("Move", true);
        }
        else
        {
            monster_Animation.SetBool("Move", false);
        }
    }

    private void Reload()
    {
        Item newMagazine = new Item();
        newMagazine.bulletCount = 15;
        MonsterGun.Equip(newMagazine);
    }

    private void BackSlide()
    {
        MonsterGun.BackSlide();
    }

    protected override void Attack_Target()
    {        
        Vector2 startPoint = (transform.position + new Vector3(0, 0.2f, 0));
        Vector2 RandomAngle = new Vector2(RaycastDir.x, Random.Range(-0.5f, 0.6f));
        RaycastHit2D attackTarget = Physics2D.Raycast(startPoint, RandomAngle, AttackRange, AttackLayerMask);
        Debug.DrawLine(startPoint, startPoint + RandomAngle * AttackRange, Color.yellow);       
        
        if (attackTarget.transform != null)
        {
            if (attackTarget.transform.gameObject.layer == 12 && GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().isContactCover)
            {
                target = null;
            }
            else if (attackTarget.transform.gameObject.layer == 9)
            {
                target = attackTarget.transform.gameObject;
            }
        }
        else
        {
            target = null;
        }

        Attack_Animation();
    }

    private void Attack_Animation()
    {
        if (MonsterGun.equipedBullet)
        {
            monster_Animation.SetTrigger("Attack");
        }
        else
        {
            if (MonsterGun.equipedMagazine.bulletCount < 0)
            {
                //재장전 애니메이션
                monster_Animation.SetTrigger("Reload");

                //애니메이션 메서드
                //Reload();
            }
            else
            {
                MonsterGun.BackSlide();
            }            
        }
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
}
