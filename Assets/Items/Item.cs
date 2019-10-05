using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] string _firstPickup;

    public string firstPickup { get { return _firstPickup; } }
    public bool firstPickupEmitted { get; set; }

    public Sprite sprite
    {
        get
        {
            return GetComponent<SpriteRenderer>().sprite;
        }
    }

    public Location location
    {
        get
        {
            return GetComponentInParent<Location>();
        }
    }
}
