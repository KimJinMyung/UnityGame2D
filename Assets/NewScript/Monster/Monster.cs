using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

enum Monster_State
{
    Idle,
    Down,
    Attack,
    Dead
}
public abstract class Monster : MonoBehaviour
{
    //몬스터 작업 예정

    [SerializeField] protected float Hp = 12;
    [SerializeField] protected float ATK = 30;
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected int Parts = 1;
    [SerializeField] protected float MoveSpeed = 3f;  //이동 속도
    [SerializeField] protected float EvasionPer;  //회피율
    [SerializeField] protected float AttackRange; //공격 거리
    [SerializeField] protected float SightRange;    //시야 범위

    protected Rigidbody2D monster_Rigid;
    protected SpriteRenderer[] Aiming;
    protected bool isBleeding = false;

    //public IObjectPool<GameObject> _pool;

    protected GameObject Target_Player;

    protected Vector2 MoveDir = Vector2.zero;
    protected Vector2 RaycastDir = Vector2.right;
    protected Vector3 monster_Rotation = Vector3.zero;

    [SerializeField]
    protected LayerMask LayerMask;
    [SerializeField]
    protected LayerMask AttackLayerMask;

    protected bool isEvasionPer_change = false;

    protected GameObject target;

    private Monster_State monster_State;

    protected bool isDontMove;

    public int Monster_Type_ID { get; protected set; }
    // 근거리 1/ 원거리 2

    protected abstract void State_DownMonster();
    protected abstract void Monster_HurtAnimation();
    protected abstract void Monster_DeadAnimation();
    protected abstract void Monster_MoveAnimation();

    private void OnEnable()
    {
        monster_State = Monster_State.Idle;
    }

    protected void OffTargeting()
    {
        Aiming[0].enabled = false;
        Aiming[1].enabled = false;
        Aiming[2].enabled = false;
    }

    ////오버 로딩
    public void SetTargeted()
    {
        if (isDead) return;
        OffTargeting();
    }

    public void SetTargeted(bool isHeadAiming, bool isLegAiming)
    {
        if (isDead) return;
        OnTargeting(isHeadAiming, isLegAiming);
    }

    //히트 박스 위치 변경
    protected abstract void HitBox_Position();
    //{
    //    if (monster_Animation.GetBool("Down"))
    //    {
    //        transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.63f, -0.38f, 0);
    //        transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.32f, -0.68f, 0);
    //        transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.32f, -0.77f, 0);
    //    }
    //    else
    //    {
    //        transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0f, 0.56f, 0);
    //        transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0f, -0.04f, 0);
    //        transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0f, -0.6f, 0);
    //    }
    //}

    public abstract void SetMonster(float HP, float ATK, float AttackRange, bool isDead);
    
    //{
    //    this.Hp = HP;
    //    this.ATK = ATK;
    //    this.isDead = isDead;
    //    isEvasionPer_change = false;

    //    monster_State = Monster_State.Idle;
    //    //monster_Animation.SetBool("Down", false);
    //    HitBox_Position();
    //    isDontMove = false;
    //    monster_Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    //    transform.rotation = Quaternion.identity;
    //    gameObject.layer = 6;
    //}

    protected void OnTargeting(bool isHeadAiming, bool isLegAiming)   //공격받는 부위 표시
    {
        if (isHeadAiming && !isLegAiming)
        {
            Aiming[0].enabled = true;
            Aiming[1].enabled = false;
            Aiming[2].enabled = false;
        }
        else if (!isHeadAiming && isLegAiming)
        {
            Aiming[0].enabled = false;
            Aiming[1].enabled = false;
            Aiming[2].enabled = true;
        }
        else
        {
            Aiming[0].enabled = false;
            Aiming[1].enabled = true;
            Aiming[2].enabled = false;
        }
    }

    protected void HitParts() //공격받은 부위 판별
    {
        int line = 0;
        foreach (var i in Aiming)
        {
            if (i.enabled)
            {
                Parts = line;
            }
            line++;
        }
    }
    
    

    public virtual void Hurt(float damage)
    {
        HitParts();

        if (Random.Range(0, 100f) >= EvasionPer)
        {
            if (Parts == 2)
            {
                //if(rb == null) rb = GetComponent<Rigidbody2D>();
                if (transform.rotation.z == 0)
                {
                    monster_State = Monster_State.Down;
                    isDontMove = true;
                    State_DownMonster();
                    //monster_Animation.SetBool("Down", true);
                    HitBox_Position();

                    // 스프라이트 없을 당시 넘어짐 표현
                    //monster_Rigid.constraints = RigidbodyConstraints2D.None;
                    //float RotationdDir = transform.position.x - GameManager.Instance.GetPlayer.transform.position.x < 0f ? -60f : 60f;
                    //transform.Rotate(0, 0, RotationdDir);
                }
            }
            Hp -= damage;

            if (Hp <= 0)
            {
                isDead = true;
                Die();
            }
            else
            {
                //monster_Animation.SetTrigger("Hurt");
                Monster_HurtAnimation();
            }

            if (!isBleeding)
            {
                isBleeding = true;
                StartCoroutine(Bleeding());
            }
        }
    }

    protected virtual void Die()
    {
        isBleeding = false;
        monster_State = Monster_State.Dead;

        //애니메이션
        //monster_Animation.SetTrigger("Dead");
        Monster_DeadAnimation();

        StopAllCoroutines();
        MoveDir = Vector2.zero;        
     
        OffTargeting();
        gameObject.layer = 7;
        if(Random.Range(0f,100f) < 80f)
        {
            Item item = new Item();
            item.bulletCount = Random.Range(0, 16);
            ObjectPoolManager.Instance.Drop(item, this.gameObject);
        }

        StartCoroutine(ReleaseMonster());
    }

    protected IEnumerator ReleaseMonster()    // 풀링으로 반환
    {
        yield return new WaitForSeconds(5f);
        //_pool.Release(this.gameObject);
        Monster_Spawner.instance.OnRelease(this.gameObject);
        yield break;
    }
    protected IEnumerator Bleeding()  //출혈
    {
        while (!isDead)
        {
            // 주기적으로 피해를 입히는 간격(예: 1초)을 기다립니다.
            yield return new WaitForSeconds(2f);

            // 피해를 입히는 부분
            Hp -= 1;
            if (Hp <= 0) Die();
        }
    }    

    protected void Monster_Move() //움직임
    {
        if (GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().state == PlayerState.Die) return;
        if (isDontMove) return;
        if (monster_State == Monster_State.Dead || monster_State == Monster_State.Down || monster_State == Monster_State.Attack) return;
        else if (Target_Player.GetComponent<Player_Controller>().isWaveStart)
        {
            if (Target_Player.transform.position.x - transform.position.x < 0f)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (Target_Player.transform.position.x - transform.position.x > 0f)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (Mathf.Abs(Target_Player.transform.position.x - transform.position.x) >= AttackRange)
            {
                //monster_Animation.SetBool("Move", true);
                MoveDir = Vector2.right;                
            }
            else
            {
                //monster_Animation.SetBool("Move", false);
                MoveDir = Vector2.zero;
            }

            Monster_MoveAnimation();

            transform.Translate(MoveDir * MoveSpeed * Time.deltaTime);
        }
    }

    protected void EvasionPer_Up(float value) //회피율 상승
    {
        if (!isEvasionPer_change)
        {
            EvasionPer += value;
            isEvasionPer_change = true;
        }
    }

    protected void EvasionPer_Down(float value)   //회피율 감소
    {
        if (isEvasionPer_change)
        {
            EvasionPer -= value;
            isEvasionPer_change = false;
        }
    }
    protected void OnTriggerStay2D(Collider2D collision)  
    {
        if (collision.gameObject.layer == 10)   //조명
        {
            EvasionPer_Down(20);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)   //조명
        {
            EvasionPer_Up(20);
        }
    }
    protected void Raycast_Dir()
    {
        if (isDontMove) return;
        if (monster_State == Monster_State.Down || monster_State == Monster_State.Dead) return;
        if (transform.eulerAngles.y == 0)
        {
            RaycastDir = Vector2.right;
        }
        else
        {
            RaycastDir = Vector2.left;
        }
    }

    protected void Look()
    {
        if (GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().state == PlayerState.Die) return;
        if (monster_State == Monster_State.Dead) return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, RaycastDir, SightRange, LayerMask);
        //Debug.DrawLine(transform.position, (Vector2)transform.position + RaycastDir * SightRange, Color.red);

        if (hit.transform != null)
        {
            Target_Player.GetComponent<Player_Controller>().isWaveStart = true;
            //monster_State = State.Attack;

            if (monster_State != Monster_State.Attack)
            {
                if(Mathf.Abs(hit.distance) < AttackRange)
                {
                    target = hit.transform.gameObject;
                    Attack_Target();
                }                
            }
        }
    }

    protected abstract void Attack_Target();

    protected void Attack_Start()
    {
        monster_State = Monster_State.Attack;
    }

    protected void Attack_End()
    {
        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1f);
        monster_State = Monster_State.Idle;
        yield break;
    }

    protected virtual void Attack()
    {
        if (target == null) return;
        target.GetComponent<Player_Controller>().Hurt(ATK, this.transform);
    }    
}
