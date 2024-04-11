using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Magazine : MonoBehaviour
{
    public Item bullet {  get; private set; }
    public bool isPickUped = false;

    public IObjectPool<GameObject> Pool;

    private void Start()
    {
        if(bullet == null)
        {            
            bullet = new Item();
            bullet.bulletCount = 15;
        }        
        if(Pool == null)
        {
            Pool = ObjectPoolManager.Instance.pool;
        }
    }

    //public void SetManagedPool(IObjectPool<Magazine> pool)
    //{
    //    Pool = pool;
    //}

    public void DestoryMagazine()
    {
        if (Pool != null)
        {
            Pool.Release(this.gameObject);
        }
    }

    public void SetMagazine(Item item, bool isPickuped)
    {
        this.bullet = item;
        this.isPickUped = isPickuped;
    }

    //private Magazine CreateMagazine()
    //{
    //    Magazine magazine = Instantiate(this.magazinePrefab).GetComponent<Magazine>();
    //    magazine.SetManagedPool(_ManagedPool);
    //    return magazine;
    //}

    //private void OnGetMagazine(Magazine magazine)
    //{
    //    magazine.gameObject.SetActive(true);
    //}

    //private void OnReleaseMagazine(Magazine magazine)
    //{
    //    magazine.gameObject.SetActive(false);
    //}

    //private void OnDestroyMagazine(Magazine magazine)
    //{
    //    Destroy(magazine.gameObject);
    //}
}
