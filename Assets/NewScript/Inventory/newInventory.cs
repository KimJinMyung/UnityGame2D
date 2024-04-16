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
        Inven = new Item[4];
    }

    public void Equip()
    {
        grip = null;
        GameManager.Instance.Drop_UI_Aimation();
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
            GameManager.Instance.Print_Player_Grip_Ainimation();
            GameManager.Instance.Print_Player_InvenSlot_Up_Animation(slotIndex);
        }
        else if (Inven[slotIndex - 1] == null && grip != null)
        {
            Inven[slotIndex-1] = grip;
            grip = null;
            GameManager.Instance.Drop_UI_Aimation();
            GameManager.Instance.Print_Player_InvenSlot_Up_Animation(slotIndex);
        }        
    }

    public void Print_Inventory_Slot()
    {
        int index = 0;
        foreach (Item item in Inven)
        {
            if (item != null)
            {
                GameManager.Instance.Print_Player_InvenSlot(item, index);
            }
            else
            {
                GameManager.Instance.Print_Player_InvenSlot(index);
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
}
