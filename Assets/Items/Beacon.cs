using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : Item
{

    [SerializeField] Sprite inactiveSprite;
    [SerializeField] Sprite activeSprite;

    SpriteRenderer _sr;

    SpriteRenderer sr
    {
        get
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            return _sr;
        }
    }

    public void SetBeaconActive(bool active)
    {
        sr.sprite = active ? activeSprite : inactiveSprite;
    }
}
