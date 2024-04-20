using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class NewInventory
{
    public Item grip {  get; private set; }
    public Item[] Inven {  get; private set; }

    private Vector2 dropPos = Vector2.zero;

    public NewInventory()
    {
        //grip = new Item();
        Inven = new Item[4];

        //Inven[0] = new Item();
        //Inven[1] = new Item();
        //Inven[2] = new Item();
        //Inven[3] = new Item();
    }

    public void NewGame_Init_Inventory()
    {
        grip = null;
        for(int i = 0; i < Inven.Length; i++)
        {
            Inven[i] = null;
        }
    }

    public void Equip()
    {
        grip = null;
        Game_UI_Manager.Instance.Drop_UI_Aimation();
    }

    public void UnEquip(Item item)
    {
        grip = item;
    }    
    
    public void ChangeMagazineSlot(int slotIndex)
    {
        if (Inven[slotIndex-1]  != null && grip == null)
        {
            grip = Inven[slotIndex-1];
            Inven[slotIndex - 1] = null;
            Game_UI_Manager.Instance.Print_Player_Grip_Ainimation();
            Game_UI_Manager.Instance.Print_Player_InvenSlot_Up_Animation(slotIndex);
        }
        else if (Inven[slotIndex - 1] == null && grip != null)
        {
            Inven[slotIndex-1] = grip;
            grip = null;
            Game_UI_Manager.Instance.Drop_UI_Aimation();
            Game_UI_Manager.Instance.Print_Player_InvenSlot_Up_Animation(slotIndex);
        }        
    }

    public void Print_Inventory_Grip()
    {
        if (grip != null)
        {
            Game_UI_Manager.Instance.Print_Player_Grip(grip);
        }
        else
        {
            Game_UI_Manager.Instance.Print_Player_Grip();
        }
    }

    public void Print_Inventory_Slot()
    {
        int index = 0;
        foreach (Item item in Inven)
        {
            if (item != null)
            {
                Game_UI_Manager.Instance.Print_Player_InvenSlot(item, index);
            }
            else
            {
                Game_UI_Manager.Instance.Print_Player_InvenSlot(index);
            }
            index++;
        }
    }

    public void RechargeAll()
    {
        foreach (Item item in Inven)
        {
            if (item != null)
            {
                item.bulletCount = 15;
            }
        }

        if(grip != null)
        {
            grip.bulletCount = 15;
        }
    }

    public void Load_Grip(int gripBullet)
    {
        if(this.grip == null)
        {
            this.grip = new Item();
        }

        if(gripBullet > 0)
            this.grip.bulletCount = gripBullet;
        else
            this.grip = null;
    }

    public void Load_Inven(int[] invenBullet)
    {
        for (int i = 0; i < 4; i++)
        {
            Inven[i] = new Item();

            if (invenBullet[i] == -1)
            {
                Inven[i] = null;
            }
            else
            {
                Inven[i].bulletCount = invenBullet[i];
            }
        }
    }
}
