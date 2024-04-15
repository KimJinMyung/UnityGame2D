using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Monster_Spawner : MonoBehaviour
{
    public static Monster_Spawner instance;

    [SerializeField]
    private GameObject monsterPrefab;
    [SerializeField]
    private Transform[] spawnPoints;

    public IObjectPool<GameObject> MonsterPool;

    private float ReleaseCount=0f;
    private float spawnTime = 0f;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this.gameObject);

        Init();
    }    

    private void Init()
    {
        float MaxMonsterCount = 5f;
        MonsterPool = new ObjectPool<GameObject>(CreateMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: (int)MaxMonsterCount);
        ReleaseCount = MaxMonsterCount;

       for (int i = 0; i < 10; i++)
       {
            Monster monster = CreateMonster().GetComponent<Monster>();
            monster._pool.Release(monster.gameObject);
       }
    }

    private void FixedUpdate()
    {
        if (ReleaseCount > 0f && GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().isWaveStart)
        {
            if (Time.time - spawnTime > Random.Range(5f, 8f))
            {
                MonsterRespawn();
            }
        }        
    }

    public void MonsterRespawn()
    {
        spawnTime = Time.time;

        Vector2 spwanPoint;
        if (Random.Range(0, 100f) < 50f)
        {
            spwanPoint = spawnPoints[0].position;
        }
        else
        {
            spwanPoint = spawnPoints[1].position;
        }

        var newMonster = instance.MonsterPool.Get();
        newMonster.GetComponent<Monster>().SetMonster(12, 30, false);
        newMonster.transform.position = spwanPoint;
    }
    

    private GameObject CreateMonster()
    {
        ReleaseCount--;
        GameObject monster = Instantiate(monsterPrefab);
        monster.GetComponent<Monster>()._pool = this.MonsterPool;
        return monster;
    }

    private void OnGetMonster(GameObject monster)
    {
        ReleaseCount--;
        monster.SetActive(true);
    }

    private void OnReleaseMonster(GameObject monster)
    {
        ReleaseCount++;
        monster.SetActive(false);
    }

    private void OnDestroyMonster(GameObject monster)
    {
        Destroy(monster);
    }
}
