using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.Progress;

enum State
{
    Idle,
    Attack,
    Down,
    Dead
}

public class Monster : MonoBehaviour
{
    [SerializeField] private float Hp = 12;
    [SerializeField] private float ATK = 30;
    [SerializeField] private bool isDead = false;
    [SerializeField] private int Parts = 1;
    [SerializeField] private float MoveSpeed = 3f;  //이동 속도
    [SerializeField] private float EvasionPer;  //회피율
    [SerializeField] private float Accuracy;    //명중률
    //[SerializeField] private float MaxSpeed = 5f;

    private Rigidbody2D monster_Rigid;
    private SpriteRenderer[] Aiming;
    private bool isBleeding = false;
    private bool isAttacking = false;

    private Gun MonsterGun;

    [SerializeField]
    private GameObject dropMagazine;

    public IObjectPool<GameObject> _pool;

    private GameObject Target_Player;

    private Vector2 MoveDir = Vector2.zero;
    private Vector2 RaycastDir = Vector2.right;
    private Vector3 monster_Rotation = Vector3.zero;  

    private float Velocity;

    private State monster_State;
    private LayerMask LayerMask = 512;
    private bool isEvasionPer_change = false;

    private void Awake()
    {
        monster_Rigid= GetComponent<Rigidbody2D>();
        Aiming = new SpriteRenderer[3];           
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
        Velocity = monster_Rigid.velocity.x;

        monster_State = State.Idle;
        LayerMask = 512;
        Accuracy = 40;
        EvasionPer = 0;
    }

    public void SetMonster(float HP, float ATK, bool isDead)
    {
        this.Hp = HP;
        this.ATK = ATK;
        this.isDead = isDead;
        isEvasionPer_change = false;
        
        monster_State = State.Idle;
        monster_Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.identity;
        gameObject.layer = 6;
    }

    public void Hurt(float damage)
    {
        HitParts();

        if (Random.Range(0,100f) > EvasionPer)
        {
            if (Parts == 2)
            {
                //if(rb == null) rb = GetComponent<Rigidbody2D>();
                if (transform.rotation.z == 0)
                {
                    isAttacking = Random.Range(0,100f) < 30f ? false : true;
                    monster_State = State.Down;
                    monster_Rigid.constraints = RigidbodyConstraints2D.None;
                    float RotationdDir = transform.position.x - GameManager.Instance.GetPlayer.transform.position.x < 0f ? -60f : 60f;
                    transform.Rotate(0, 0, RotationdDir);
                }
            }
            Debug.Log($"{damage}만큼의 공격을 받았습니다.");
            Hp -= damage;

            if (!isBleeding)
            {
                isBleeding = true;
                StartCoroutine(Bleeding());
            }
        }

        if (Hp <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        monster_State = State.Dead;
        StopAllCoroutines();
        MoveDir = Vector2.zero;
        //transform.gameObject.SetActive(false);
        if (transform.rotation.z == 0)
        {
            monster_Rigid.constraints = RigidbodyConstraints2D.None;
            float RotationdDir = transform.position.x - GameManager.Instance.GetPlayer.transform.position.x < 0f ? -60f : 60f;
            transform.Rotate(0, 0, RotationdDir);
        }
        OffTargeting();
        gameObject.layer = 7;
        Item item = new Item();
        item.bulletCount = Random.Range(0, 16);
        ObjectPoolManager.Instance.Drop(item, this.gameObject);

        StartCoroutine(ReleaseMonster());
    }    

    private IEnumerator ReleaseMonster()
    {
        yield return new WaitForSeconds(5f);
        _pool.Release(this.gameObject);
        yield break;
    }

    private void OnTargeting(bool isHeadAiming, bool isLegAiming)
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

    private void HitParts()
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

    private void OffTargeting()
    {
        Aiming[0].enabled = false;
        Aiming[1].enabled = false;
        Aiming[2].enabled = false;
    }

    ////오버 로딩
    public void SetTargeted()
    {
        if(isDead) return;
        OffTargeting();
    }

    public void SetTargeted(bool isHeadAiming, bool isLegAiming)
    {
        if(isDead) return;
        OnTargeting(isHeadAiming, isLegAiming);
    }

    private IEnumerator Bleeding()
    {
        while (!isDead)
        {            
            // 주기적으로 피해를 입히는 간격(예: 1초)을 기다립니다.
            yield return new WaitForSeconds(2f);

            // 피해를 입히는 부분
            Hurt(1);
            Debug.Log("Bleeding...");
        }
    }

    private void Monster_Move()
    {
        if(monster_State == State.Dead || monster_State == State.Down) return;
        if (Target_Player.GetComponent<Player_Controller>().isWaveStart)
        {            
            if (Target_Player.transform.position.x - transform.position.x < 0f)
            {
                transform.eulerAngles = new Vector3(0,180,0);           
            }
            else if (Target_Player.transform.position.x - transform.position.x > 0f)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if(Mathf.Abs(Target_Player.transform.position.x - transform.position.x) >= 4.5f) MoveDir = Vector2.right;
            else MoveDir = Vector2.zero;

            transform.Translate(MoveDir * MoveSpeed * Time.deltaTime);
        }
    }    

    private void Raycast_Dir()
    {
        if (transform.eulerAngles.y == 0)
        {
            RaycastDir = Vector2.right;
        }
        else
        {
            RaycastDir = Vector2.left;
        }
    }

    private void Look()
    {       
        if(monster_State == State.Dead) return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, RaycastDir, 4.5f, LayerMask);
        Debug.DrawLine(transform.position, (Vector2)transform.position + RaycastDir * 4.5f, Color.red);

        if (hit.transform != null)
        {
            Target_Player.GetComponent<Player_Controller>().isWaveStart = true;
            //monster_State = State.Attack;

            if (!isAttacking)
            {
                if (MonsterGun.equipedBullet)
                {
                    StartCoroutine(Attack(hit.transform.gameObject));
                }
                else
                {
                    if(MonsterGun.equipedMagazine.bulletCount < 0)
                    {
                        //재장전 애니메이션
                        isAttacking = true;
                        Item newMagazine = new Item();
                        newMagazine.bulletCount = 15;
                        MonsterGun.Equip(newMagazine);
                        isAttacking = false;
                    }else
                    MonsterGun.BackSlide();
                }
                
            }            
        }
    }

    private IEnumerator Attack(GameObject target)
    {               
        isAttacking = true;
        if(Random.Range(0,100f) < Accuracy)
            target.GetComponent<Player_Controller>().Hurt(ATK, this.transform);
        MonsterGun.BackSlide();
        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
        yield break;
    }

    private void FixedUpdate()
    {
        Monster_Move();
        Raycast_Dir();
        Look();
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

    private void OnTriggerStay2D(Collider2D collision)
    {        
        if (collision.gameObject.layer == 10)
        {
            EvasionPer_Down(20);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            EvasionPer_Up(20);
        }
    }
}
