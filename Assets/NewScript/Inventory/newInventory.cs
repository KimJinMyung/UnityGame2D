using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class newInventory
{
    public Item grip {  get; private set; }
    private Item[] Inven = new Item[4];

    private Vector2 dropPos = Vector2.zero;

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
}
