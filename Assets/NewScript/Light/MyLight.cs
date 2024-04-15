using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLight : MonoBehaviour
{
    private bool isDead = false;

    private Rigidbody2D Light_Rigid;
    private SpriteRenderer Aiming;

    //private IObjectPool<Magazine> _pool;

    private void Awake()
    {
        Light_Rigid = GetComponent<Rigidbody2D>();
        Aiming = transform.GetChild(1).GetComponent<SpriteRenderer>();

        OffTargeting();
    }

    public void Hurt()
    {
        transform.GetChild(0).gameObject.SetActive(false);      
        isDead = true;
    }    

    public void OnTargeting()
    {
        if (isDead) return;
        Aiming.enabled = true;        
    }

    public void OffTargeting()
    {
        Aiming.enabled = false;        
    }    
}
