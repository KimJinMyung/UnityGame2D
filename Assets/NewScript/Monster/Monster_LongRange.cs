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
    //[SerializeField] private float Hp = 12;
    //[SerializeField] private float ATK = 30;
    //[SerializeField] private bool isDead = false;
    //[SerializeField] private int Parts = 1;
    //[SerializeField] private float MoveSpeed = 3f;  //이동 속도
    //[SerializeField] private float EvasionPer;  //회피율
    //[SerializeField] private float AttackRange; //공격 거리
    //[SerializeField] private float SightRange;  //시야 범위
    ////[SerializeField] private float MaxSpeed = 5f;

    //private Rigidbody2D monster_Rigid;
    //private SpriteRenderer[] Aiming;
    //private bool isBleeding = false;

    private Gun MonsterGun;

    [SerializeField]
    private GameObject dropMagazine;

    //public IObjectPool<GameObject> _pool;

    //private GameObject Target_Player;

    //private Vector2 MoveDir = Vector2.zero;
    //private Vector2 RaycastDir = Vector2.right;
    //private Vector3 monster_Rotation = Vector3.zero;  

    //private State monster_State;
    //[SerializeField]
    //private LayerMask LayerMask;
    //[SerializeField]
    //private LayerMask AttackLayerMask;
    //private bool isEvasionPer_change = false;

    private Animator monster_Animation;
    //private GameObject target;

    //private bool isDontMove;

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

        if (_pool == null)
        {
            _pool = Monster_Spawner.instance.MonsterPool;
        }

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

    //private void OnEnable()
    //{
    //    SetMonster(10f, 30f, 5.5f, false);
    //}

    private void FixedUpdate()
    {        
        Monster_Move();
        Raycast_Dir();
        Look();
    }

    //private void HitBox_Position()
    //{
    //    if(monster_Animation.GetBool("Down"))
    //    {
    //        transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.63f, -0.38f,0);
    //        transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.32f, -0.68f, 0);
    //        transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.32f, -0.77f,0);
    //    }
    //    else
    //    {
    //        transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0f, 0.56f,0);
    //        transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0f, -0.04f,0);
    //        transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0f, -0.6f, 0);
    //    }
    //}

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

    //public void Hurt(float damage)
    //{
    //    HitParts();

    //    if (Random.Range(0,100f) >= EvasionPer)
    //    {
    //        if (Parts == 2)
    //        {
    //            //if(rb == null) rb = GetComponent<Rigidbody2D>();
    //            if (transform.rotation.z == 0)
    //            {
    //                monster_State = State.Down;
    //                isDontMove = true;
    //                monster_Animation.SetBool("Down", true);
    //                HitBox_Position();
    //                //monster_Rigid.constraints = RigidbodyConstraints2D.None;
    //                //float RotationdDir = transform.position.x - GameManager.Instance.GetPlayer.transform.position.x < 0f ? -60f : 60f;
    //                //transform.Rotate(0, 0, RotationdDir);
    //            }
    //        }
    //        Debug.Log($"{damage}만큼의 공격을 받았습니다.");
    //        Hp -= damage;

    //        if (Hp <= 0)
    //        {
    //            isDead = true;
    //            Die();
    //        }
    //        else
    //        {
    //            monster_Animation.SetTrigger("Hurt");
    //        }

    //        if (!isBleeding)
    //        {
    //            isBleeding = true;
    //            StartCoroutine(Bleeding());
    //        }
    //    }


    //}
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

    //public void Die()
    //{
    //    isBleeding = false;
    //    monster_State = State.Dead;
    //    monster_Animation.SetTrigger("Dead");
    //    StopAllCoroutines();
    //    MoveDir = Vector2.zero;
    //    //transform.gameObject.SetActive(false);
    //    //if (transform.rotation.z == 0)
    //    //{
    //    //    monster_Rigid.constraints = RigidbodyConstraints2D.None;
    //    //    float RotationdDir = transform.position.x - GameManager.Instance.GetPlayer.transform.position.x < 0f ? -60f : 60f;
    //    //    transform.Rotate(0, 0, RotationdDir);
    //    //}        
    //    OffTargeting();
    //    gameObject.layer = 7;
    //    Item item = new Item();
    //    item.bulletCount = Random.Range(0, 16);
    //    ObjectPoolManager.Instance.Drop(item, this.gameObject);

    //    StartCoroutine(ReleaseMonster());
    //}    

    //private IEnumerator ReleaseMonster()
    //{
    //    yield return new WaitForSeconds(5f);
    //    _pool.Release(this.gameObject);
    //    yield break;
    //}

    //private void OnTargeting(bool isHeadAiming, bool isLegAiming)
    //{
    //    if (isHeadAiming && !isLegAiming)
    //    {
    //        Aiming[0].enabled = true;
    //        Aiming[1].enabled = false;
    //        Aiming[2].enabled = false;
    //    }
    //    else if (!isHeadAiming && isLegAiming)
    //    {
    //        Aiming[0].enabled = false;
    //        Aiming[1].enabled = false;
    //        Aiming[2].enabled = true;
    //    }
    //    else
    //    {
    //        Aiming[0].enabled = false;
    //        Aiming[1].enabled = true;
    //        Aiming[2].enabled = false;
    //    }
    //}

    //private void HitParts()
    //{
    //    int line = 0;
    //    foreach (var i in Aiming)
    //    {
    //        if (i.enabled)
    //        {
    //            Parts = line;
    //        }
    //        line++;
    //    }
    //}

    //private void OffTargeting()
    //{
    //    Aiming[0].enabled = false;
    //    Aiming[1].enabled = false;
    //    Aiming[2].enabled = false;
    //}

    ////오버 로딩
    //public void SetTargeted()
    //{
    //    if(isDead) return;
    //    OffTargeting();
    //}

    //public void SetTargeted(bool isHeadAiming, bool isLegAiming)
    //{
    //    if(isDead) return;
    //    OnTargeting(isHeadAiming, isLegAiming);
    //}

    //private IEnumerator Bleeding()
    //{
    //    while (!isDead)
    //    {            
    //        // 주기적으로 피해를 입히는 간격(예: 1초)을 기다립니다.
    //        yield return new WaitForSeconds(2f);

    //        // 피해를 입히는 부분
    //        Hp -= 1;
    //        if (Hp <= 0) Die();
    //    }
    //}

    //private void Monster_Move()
    //{
    //    if (GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().state == PlayerState.Die) return;
    //    if(isDontMove) return;
    //    if(monster_State == State.Dead || monster_State == State.Down || monster_State == State.Attack) return;
    //    else if (Target_Player.GetComponent<Player_Controller>().isWaveStart)
    //    {            
    //        if (Target_Player.transform.position.x - transform.position.x < 0f)
    //        {
    //            transform.eulerAngles = new Vector3(0,180,0);           
    //        }
    //        else if (Target_Player.transform.position.x - transform.position.x > 0f)
    //        {
    //            transform.eulerAngles = new Vector3(0, 0, 0);
    //        }

    //        if (Mathf.Abs(Target_Player.transform.position.x - transform.position.x) >= 4.5f)
    //        {
    //            monster_Animation.SetBool("Move", true);
    //            MoveDir = Vector2.right;
    //        }
    //        else
    //        {
    //            monster_Animation.SetBool("Move", false);
    //            MoveDir = Vector2.zero;
    //        }

    //        transform.Translate(MoveDir * MoveSpeed * Time.deltaTime);            
    //    }
    //}    

    //private void Raycast_Dir()
    //{
    //    if (isDontMove) return;
    //    if (monster_State == State.Down || monster_State == State.Dead) return;
    //    if (transform.eulerAngles.y == 0)
    //    {
    //        RaycastDir = Vector2.right;
    //    }
    //    else
    //    {
    //        RaycastDir = Vector2.left;
    //    }
    //}

    //private void Look()
    //{
    //    if (GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().state == PlayerState.Die) return;
    //    if (monster_State == State.Dead) return;
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, RaycastDir, 4.5f, LayerMask);
    //    Debug.DrawLine(transform.position, (Vector2)transform.position + RaycastDir * 4.5f, Color.red);

    //    if (hit.transform != null)
    //    {
    //        Target_Player.GetComponent<Player_Controller>().isWaveStart = true;
    //        //monster_State = State.Attack;

    //        if (monster_State != State.Attack)
    //        {
    //            Attack_Target();
    //        }            
    //    }
    //}

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
        Debug.DrawLine(startPoint, startPoint + RandomAngle * AttackRange, Color.black);       

        if (attackTarget.transform != null)
        {
            if (attackTarget.transform.gameObject.layer == 9)
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

    //private void Attack_Start()
    //{
    //    monster_State = State.Attack;
    //}

    //private void Attack_End()
    //{
    //    StartCoroutine(AttackDelay());
    //}

    //private IEnumerator AttackDelay()
    //{
    //    yield return new WaitForSeconds(1f);
    //    monster_State = State.Idle;
    //    yield break;
    //}

    //private void Attack()
    //{
    //    if (target == null) return;
    //    target.GetComponent<Player_Controller>().Hurt(ATK, this.transform);
    //    MonsterGun.BackSlide();
    //}    

    //private void EvasionPer_Up(float value)
    //{
    //    if (!isEvasionPer_change)
    //    {
    //        EvasionPer += value;
    //        isEvasionPer_change = true;
    //    }
    //}

    //private void EvasionPer_Down(float value)
    //{
    //    if (isEvasionPer_change)
    //    {
    //        EvasionPer -= value;
    //        isEvasionPer_change = false;
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{        
    //    if (collision.gameObject.layer == 10)
    //    {
    //        EvasionPer_Down(20);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == 10)
    //    {
    //        EvasionPer_Up(20);
    //    }
    //}
}
