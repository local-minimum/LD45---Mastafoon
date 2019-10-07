using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionInventorySlot : MonoBehaviour
{
    [SerializeField]
    Image inventorySlot;
    [SerializeField]
    Image inventoryItem;
    Item item;

    [SerializeField]
    Image actionPoint;

    [SerializeField]
    Color actionPointAvailable = Color.green;
    [SerializeField]
    Color actionPointUsed = Color.red;
    [SerializeField]
    Color actionPointUnavailbale = Color.gray;

    [SerializeField]
    bool slotEnabled = true;
    
    public bool slotAvailable
    {
        get
        {
            return actionPoint.color == actionPointAvailable;
        }
    }

    public bool hasItem
    {
        get
        {
            return item != null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryItem.gameObject.SetActive(false);        
        ResetActionPoint();
    }

    public bool UseActionPoint()
    {
        if (slotAvailable)
        {
            actionPoint.color = actionPointUsed;
            return true;
        }
        return false;
    }

    public void UpdateIcon(Item item, Sprite sprite)
    {
        if (item != null && this.item == item) inventoryItem.sprite = sprite;
    }

    public void PickUp(Item item, Location location)
    {
        this.item = item;
        if (location) location.RemoveItem();
        if (item)
        {
            inventoryItem.sprite = item.sprite;
            inventoryItem.gameObject.SetActive(true);
            item.transform.SetParent(transform);
            item.gameObject.SetActive(false);
        }
        ResetActionPoint();
    }

    public void ClearItem()
    {
        if (item)
        {
            inventoryItem.sprite = null;
            inventoryItem.gameObject.SetActive(false);
            Destroy(item.gameObject);
            item = null;
        }
    }

    public Item DropItem(bool defaultToAvailable=true)
    {
        Item item = this.item;
        this.item = null;        
        inventoryItem.gameObject.SetActive(false);
        ResetActionPoint(defaultToAvailable || slotAvailable || item != null);
        return item;
    }

    public void ResetActionPoint(bool defaultToAvailable=true)
    {
        actionPoint.color = item == null && slotEnabled ? (defaultToAvailable ? actionPointAvailable : actionPointUsed) : actionPointUnavailbale;
    }

    public void SetActionPointUnavailable()
    {
        actionPoint.color = actionPointUnavailbale;
    }
}
