﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsInventory : MonoBehaviour
{
    Character _character;
    [SerializeField] Image foreground;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    ActionInventorySlot[] slots;    

    void Start()
    {        
        slots = GetComponentsInChildren<ActionInventorySlot>();
        NewTurn();
    }

    public void ForgetCharacter()
    {
        _character = null;
    }

    public void ClearInventory()
    {
        if (slots == null) return;
        for (int action=0; action<slots.Length; action++)
        {
            slots[action].ClearItem();
        }
    }

    Character character
    {
        get
        {
            if (_character == null)
            {
                _character = FindObjectOfType<Character>();
            }
            return _character;
        }
    }

    public bool canPickUp
    {
        get
        {
            for (int action=0; action<slots.Length; action++)
            {
                if (slots[action].slotAvailable) return true;
            }
            return false;
        }
    }

    public void PickUp(Item item, Location location)
    {
        for (int action=slots.Length - 1; action>=0; action--)
        {
            if (slots[action].slotAvailable)
            {
                slots[action].PickUp(item, location);
                break;
            }
        }
    }

    public void AttemptDrop(ActionInventorySlot slot)
    {
        if (remainingActions > 0)
        {
            Location location = character.GetLocation();
            if (location.canPlaceItem)
            {
                location.PlaceItem(slot.DropItem());
                Item currentItem = null;
                Item nextItem = null;
                for (int action = 0; action <= slots.Length; action ++)
                {
                    ActionInventorySlot currentSlot = slots[action];
                    nextItem = currentSlot.DropItem(false);
                    if (currentItem)
                    {
                        currentSlot.PickUp(currentItem, null);
                    }
                    if (currentSlot == slot) break;
                    currentItem = nextItem;
                }
                UseAction();
            }
        }
    }

    public int remainingActions
    {
        get
        {
            int available = 0;
            for (int actions=0; actions<slots.Length; actions++)
            {
                if (slots[actions].slotAvailable)
                {
                    available++;
                }
            }
            return available;
        }
    }

    public void UseAction()
    {
        for (int action=0; action < slots.Length; action++)
        {
            if (slots[action].UseActionPoint()) break;
        }
    }

    public void NewTurn()
    {
        for (int action = 0; action < slots.Length; action++)
        {
            slots[action].ResetActionPoint();
        }
        foreground.color = activeColor;
    }

    public void EnemyTurn()
    {
        foreground.color = inactiveColor;
    }

    public void EndTurn()
    {
        for (int action = 0; action < slots.Length; action++)
        {
            slots[action].UseActionPoint();
        }
    }
}
