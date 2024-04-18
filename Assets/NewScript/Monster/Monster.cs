using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

enum Monster_State
{
    Idle,
    Down,
    Dead
}
public class Monster : MonoBehaviour
{
    //몬스터 작업 예정

    //[SerializeField] private float Hp = 12;
    //[SerializeField] private float ATK = 30;
    //[SerializeField] private bool isDead = false;
    //[SerializeField] private int Parts = 1;
    //[SerializeField] private float MoveSpeed = 3f;  //이동 속도
    //[SerializeField] private float EvasionPer;  //회피율

    //private Rigidbody2D monster_Rigid;
    //private SpriteRenderer[] Aiming;
    //private bool isBleeding = false;

    //private Monster_State monster_State;

    //public IObjectPool<GameObject> _pool;

    //private GameObject Target_Player;

    //private Vector2 MoveDir = Vector2.zero;
    //private Vector2 RaycastDir = Vector2.right;
    //private Vector3 monster_Rotation = Vector3.zero;

    //[SerializeField]
    //private LayerMask LayerMask;
    //[SerializeField]
    //private LayerMask AttackLayerMask;

    //private bool isEvasionPer_change = false;

    //private GameObject target;


    //private bool isDontMove;

    //private void Awake()
    //{
    //    monster_Rigid = GetComponent<Rigidbody2D>();
    //    Aiming = new SpriteRenderer[3];
    //}

    //private void Start()
    //{
    //    Aiming[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
    //    Aiming[1] = transform.GetChild(1).GetComponent<SpriteRenderer>();
    //    Aiming[2] = transform.GetChild(2).GetComponent<SpriteRenderer>();

    //    if (_pool == null)
    //    {
    //        _pool = Monster_Spawner.instance.MonsterPool;
    //    }

    //    OffTargeting();

    //    Target_Player = GameManager.Instance.GetPlayer;

    //    monster_State = Monster_State.Idle;
    //    LayerMask = 512;
    //    AttackLayerMask = 4608;
    //    EvasionPer = 0;

    //    isDontMove = false;
    //}

    //private void OffTargeting()
    //{
    //    Aiming[0].enabled = false;
    //    Aiming[1].enabled = false;
    //    Aiming[2].enabled = false;
    //}

    //////오버 로딩
    //public void SetTargeted()
    //{
    //    if (isDead) return;
    //    OffTargeting();
    //}

    //public void SetTargeted(bool isHeadAiming, bool isLegAiming)
    //{
    //    if (isDead) return;
    //    OnTargeting(isHeadAiming, isLegAiming);
    //}

    ////히트 박스 위치 변경
    ////private void HitBox_Position()
    ////{
    ////    if (monster_Animation.GetBool("Down"))
    ////    {
    ////        transform.GetChild(0).gameObject.transform.localPosition = new Vector3(-0.63f, -0.38f, 0);
    ////        transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.32f, -0.68f, 0);
    ////        transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0.32f, -0.77f, 0);
    ////    }
    ////    else
    ////    {
    ////        transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0f, 0.56f, 0);
    ////        transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0f, -0.04f, 0);
    ////        transform.GetChild(2).gameObject.transform.localPosition = new Vector3(0f, -0.6f, 0);
    ////    }
    ////}

    //public void SetMonster(float HP, float ATK, bool isDead)
    //{
    //    this.Hp = HP;
    //    this.ATK = ATK;
    //    this.isDead = isDead;
    //    isEvasionPer_change = false;

    //    monster_State = Monster_State.Idle;
    //    //monster_Animation.SetBool("Down", false);
    //    //HitBox_Position();
    //    isDontMove = false;
    //    monster_Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    //    transform.rotation = Quaternion.identity;
    //    gameObject.layer = 6;
    //}

    //public void Hurt(float damage)
    //{
    //    HitParts();

    //    if (Random.Range(0, 100f) >= EvasionPer)
    //    {
    //        if (Parts == 2)
    //        {
    //            //if(rb == null) rb = GetComponent<Rigidbody2D>();
    //            if (transform.rotation.z == 0)
    //            {
    //                monster_State = Monster_State.Down;
    //                isDontMove = true;
    //                //monster_Animation.SetBool("Down", true);
    //                //HitBox_Position();
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
    //            //monster_Animation.SetTrigger("Hurt");
    //        }

    //        if (!isBleeding)
    //        {
    //            isBleeding = true;
    //            StartCoroutine(Bleeding());
    //        }
    //    }


    //}

    //public void Die()
    //{
    //    isBleeding = false;
    //    monster_State = Monster_State.Dead;
    //    //monster_Animation.SetTrigger("Dead");
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
}
