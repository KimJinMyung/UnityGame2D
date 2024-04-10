using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    public Item bullet {  get; private set; }
    public bool isPickUped = false;

    public Magazine()
    {
        if(bullet == null)
        {            
            bullet = new Item();
            bullet.bulletCount = 15;
        }
    }

    public void SetMagazine(Item item, bool isPickuped)
    {
        this.bullet = item;
        this.isPickUped = isPickuped;
    }    
}
