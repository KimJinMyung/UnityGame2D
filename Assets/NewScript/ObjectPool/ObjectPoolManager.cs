using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField]
    private GameObject magazinePrefab;

    public IObjectPool<GameObject> pool;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this.gameObject);

        Init();
    }

    private void Init()
    {
        pool = new ObjectPool<GameObject>(CreateMagazine, OnGetMagazine, OnReleaseMagazine, OnDestroyMagazine, maxSize:10);
        
        for(int i = 0; i < 3; i++)
        {
            Magazine magazine = CreateMagazine().GetComponent<Magazine>();
            magazine.Pool.Release(magazine.gameObject);
        }
    }

    private GameObject CreateMagazine()
    {
        GameObject magazine = Instantiate(magazinePrefab);
        magazine.GetComponent<Magazine>().Pool = this.pool;
        return magazine;
    }

    private void OnGetMagazine(GameObject magazine)
    {
        magazine.SetActive(true);
    }

    private void OnReleaseMagazine(GameObject magazine)
    {
        magazine.SetActive(false);
    }

    private void OnDestroyMagazine(GameObject magazine)
    {
        Destroy(magazine);
    }

    public void Drop(Item item, GameObject gameObject)
    {
        Vector2 dropPos = gameObject.transform.position;
        var newMagazine = Instance.pool.Get();
        newMagazine.GetComponent<Magazine>().SetMagazine(item, true);
        newMagazine.transform.position = dropPos;
        StartCoroutine(ReleaseMagazine(newMagazine));
    }

    private IEnumerator ReleaseMagazine(GameObject magazine)
    {
        yield return new WaitForSeconds(30f);
        OnReleaseMagazine(magazine);
        yield break;
    }
}
