using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singleTone1<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (T)FindAnyObjectByType(typeof(T));

                if(instance == null)
                {
                    GameObject newGameObject = new GameObject();
                    instance = newGameObject.AddComponent<T>();

                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }
}
