using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePoolManager : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefabs;

    private Queue<GameObject> IMG_Pool = new Queue<GameObject>();

    public static ImagePoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for(int i = 0; i< 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefabs);
            instanceToAdd.transform.SetParent(transform);
            OnRelease(afterImagePrefabs);
        }
    }

    public void OnRelease(GameObject prefab)
    {
        prefab.SetActive(false);
        IMG_Pool.Enqueue(prefab);
    }

    public GameObject CreateObject()
    {
        if(IMG_Pool.Count == 0)
        {
            GrowPool();
        }

        var instance = IMG_Pool.Dequeue();
        instance.SetActive(true);
        return instance;
    }

}
