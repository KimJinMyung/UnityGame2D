using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Monster_Spawner : MonoBehaviour
{
    public static Monster_Spawner instance {  get; private set; }

    [SerializeField]
    private GameObject[] monsterPrefab = new GameObject[2];
    [SerializeField]
    private Transform[] spawnPoints;

    private Vector2 MinPos;
    private Vector2 MaxPos;

    [SerializeField]
    private GameObject Ground;

    private List<GameObject> monsterList;

    private float ReleaseCount=0f;
    private float spawnTime = 0f;

    [SerializeField]
    private float MaxMonsterCount = 5f;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this.gameObject);       

        Init();
    }

    private void Start()
    {
        MinPos = new Vector2(-(Ground.transform.localScale.x / 2) + Ground.transform.position.x /*+ Mathf.Abs(Camera.main.transform.GetChild(1).localPosition.x)*/, 0);
        MaxPos = new Vector2((Ground.transform.localScale.x / 2) + Ground.transform.position.x /*- Mathf.Abs(Camera.main.transform.GetChild(0).localPosition.x)*/,  0);
    }

    private void Init()
    {
        monsterList = new List<GameObject>();

        ReleaseCount = MaxMonsterCount;

       for (int i = 0; i < MaxMonsterCount; i++)
       {
            ///*Monster_LongRange*/Monster monster = CreateMonster().GetComponent<Monster>();
            //monster._pool.Release(monster.gameObject);

            Monster newMonster = OnCreate().GetComponent<Monster>();
            OnRelease(newMonster.gameObject);
       }
    }

    private void FixedUpdate()
    {
        if (ReleaseCount > 0f && GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().isWaveStart)
        {
            if (Time.time - spawnTime > Random.Range(5f, 10f))
            {
                for(int i = 0; i< Random.Range(1f, 1f); i++)
                {
                    MonsterRespawn();
                }
            }
        }

        if(ReleaseCount > MaxMonsterCount)
        {
            int destroyMonsterIndex = Random.Range(0, monsterList.Count - 1);
            var destroyMonster = monsterList[destroyMonsterIndex];
            monsterList.RemoveAt(destroyMonsterIndex);
            ReleaseCount--;
        }
        else if(ReleaseCount < MaxMonsterCount)
        {
            Monster newMonster = OnCreate().GetComponent<Monster>();
            OnRelease(newMonster.gameObject);
            ReleaseCount++;
        }
    }

    public void MonsterRespawn()
    {
        spawnTime = Time.time;

        Vector2 spwanPoint;
        if (spawnPoints[1].position.x < MinPos.x)
        {
            spwanPoint = spawnPoints[0].position;
        }
        else if(spawnPoints[0].position.x > MaxPos.x)
        {
            spwanPoint = spawnPoints[1].position;
        }
        else
        {
            if (Random.Range(0, 100f) < 50f)
            {
                spwanPoint = spawnPoints[0].position;
            }
            else
            {
                spwanPoint = spawnPoints[1].position;
            }
        }

        //var newMonster = instance.MonsterPool.Get();
        var newMonster = instance.OnGet();

        if (newMonster.GetComponent<Monster>().Monster_Type_ID == 1)
        {
            newMonster.GetComponent<Monster_ShortRange>().SetMonster(12f, 100f, 1.5f, false);
        }else if(newMonster.GetComponent<Monster>().Monster_Type_ID == 2)
        {
            newMonster.GetComponent<Monster_LongRange>().SetMonster(10f, 30f, 5.5f, false);
        }
        //newMonster.GetComponent<Monster>().SetMonster(12, 30, 4.5f, false);
        newMonster.transform.position = spwanPoint;
    }
    

    //private GameObject CreateMonster()
    //{
    //    ReleaseCount--;

    //    GameObject monster;
    //    if (Random.Range(0f,100f) >= 50f)
    //    {
    //        monster = Instantiate(monsterPrefab[0]);
    //        //monster.GetComponent<Monster_ShortRange>()._pool = this.MonsterPool;
    //    }
    //    else
    //    {
    //        monster = Instantiate(monsterPrefab[1]);            
    //       // monster.GetComponent<Monster_LongRange>()._pool = this.MonsterPool;
    //    }
        
    //    return monster;
    //}

    //private void OnGetMonster(GameObject monster)
    //{
    //    ReleaseCount--;
    //    monster.SetActive(true);
    //}

    //private void OnReleaseMonster(GameObject monster)
    //{
    //    ReleaseCount++;
    //    monster.SetActive(false);
    //}

    //private void OnDestroyMonster(GameObject monster)
    //{
    //    Destroy(monster);
    //}

    private GameObject OnCreate()
    {
        ReleaseCount--;

        GameObject monster;
        if (Random.Range(0f, 100f) >= 50f)
        {
            monster = Instantiate(monsterPrefab[0]);           
        }
        else
        {
            monster = Instantiate(monsterPrefab[1]);          
        }
        OnRelease(monster);

        return monster;
    }

    public GameObject OnGet()
    {
        var monster = monsterList[(int)Random.Range(0, ReleaseCount-1)];
        monsterList.Remove(monster);
        ReleaseCount--;
        monster.SetActive(true);
        return monster;
    }

    public void OnRelease(GameObject monster)
    {
        monster.SetActive(false);
        monsterList.Add(monster);
        ReleaseCount++;
    }
}
