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
        if(this.equipedMagazine == null || this.equipedMagazine.bulletCount <=0)
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

    public void Load_Gun(int EquipedMagazine, bool EquipedBullet)
    {
        this.equipedMagazine = new Item();

        if (EquipedMagazine > 0)
            this.equipedMagazine.bulletCount = EquipedMagazine;
        else
            this.equipedMagazine = null;

        this.equipedBullet = EquipedBullet;
    }
}
