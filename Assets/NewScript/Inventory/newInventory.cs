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
        }else if (Inven[slotIndex - 1] == null && grip != null)
        {
            Inven[slotIndex-1] = grip;
            grip = null;
        }
    }
}
