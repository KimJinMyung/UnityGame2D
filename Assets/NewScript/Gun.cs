using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Gun
{
    public Item equipedMagazine { get; private set; }
    public bool equipedBullet {  get; private set; }

    public void Equip(Item item)
    {
       this.equipedMagazine = item;     
    }

    public void UnEquip()
    {
        this.equipedMagazine = null;
    }

    public void BackSlide()
    {
        if(this.equipedMagazine.bulletCount <=0 )
        {
            equipedBullet = false;
        }
        else
        {
            equipedMagazine.bulletCount--;
            equipedBullet = true;
        }       
    }

    public bool ChedkEquipedBullet()
    {
        return equipedBullet;
    }

    public void Drop()
    {

    }
}
