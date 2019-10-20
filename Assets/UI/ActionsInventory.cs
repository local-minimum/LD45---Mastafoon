using System.Collections;
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
    [SerializeField] Button endTurnButton;

    void Start()
    {        
        slots = GetComponentsInChildren<ActionInventorySlot>();
        NewTurn();
    }

    private void Update()
    {
        UpdateEndTurnStatus();
        if (endTurnButton.interactable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EndTurnStart();
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                EndTurnStop();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                AttemptDrop(slots[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                AttemptDrop(slots[1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                AttemptDrop(slots[2]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                AttemptDrop(slots[3]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                AttemptDrop(slots[4]);
            }
        }
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
                if (!item.firstPickupEmitted && !string.IsNullOrEmpty(item.firstPickup))
                {
                    Narrator.ShowPieceByKey(item.firstPickup);
                    item.firstPickupEmitted = true;
                }
                break;
            }
        }
    }

    public void AttemptDrop(ActionInventorySlot slot)
    {
        if (remainingActions > 0 && slot.hasItem)
        {
            Location location = character.GetLocation();
            if (location.canPlaceItem)
            {
                location.PlaceItem(slot.DropItem());
                character.PlayDrop();
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
        if (slots == null) return;
        for (int action = 0; action < slots.Length; action++)
        {
            slots[action].ResetActionPoint();
        }
        foreground.color = activeColor;
        endTurnButton.interactable = true;
    }

    public void SetNotMyTurn()
    {
        foreground.color = inactiveColor;
    }

    public void EndTurn()
    {
        for (int action = 0; action < slots.Length; action++)
        {
            slots[action].UseActionPoint();
        }
        endTurnButton.interactable = false;
        endingTurn = false;
        endTurnStart = endTurnEnd;
    }

    bool endingTurn;
    float endTurnStart;
    float endTurnEnd;
    [SerializeField, Range(0, 5)] float timeToEndTurn = 1.5f;
    [SerializeField] Image endProgress;

    public void EndTurnStart()
    {        
        if (endTurnButton.interactable)
        {
            endTurnStart = Time.timeSinceLevelLoad - Mathf.Max(0f, ((endTurnEnd - endTurnStart) - (Time.timeSinceLevelLoad - endTurnEnd)) / timeToEndTurn);
            endingTurn = true;
        }
    }

    public void EndTurnStop()
    {
        if (endTurnButton.interactable)
        {
            endTurnEnd = Time.timeSinceLevelLoad;
            endingTurn = false;
        }
    }

    public void UpdateEndTurnStatus()
    {
        float progress = 0f;
        if (endingTurn)
        {
            progress = Mathf.Min(1f, (Time.timeSinceLevelLoad - endTurnStart) / timeToEndTurn);
        }
        else
        {
            progress = Mathf.Max(0f, ((endTurnEnd - endTurnStart) - (Time.timeSinceLevelLoad - endTurnEnd)) / timeToEndTurn);
        }
        endProgress.fillAmount = progress;
        if (progress == 1f)
        {
            EndTurn();
        }
    }

    public int itemCount
    {
        get
        {
            int items = 0;
            for (int action = 0; action < slots.Length; action++)
            {
                items += slots[action].hasItem ? 1 : 0;
            }
            return items;
        }
    }
}
