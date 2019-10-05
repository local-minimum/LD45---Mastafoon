using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
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
