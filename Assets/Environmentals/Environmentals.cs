using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environmentals : MonoBehaviour
{
    [SerializeField]
    bool[] OnOffSequence;
    int index = -1;
    SpriteRenderer sr;
    Location location;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        FindObjectOfType<EnvironmentManager>().Add(this);
        location = GetComponentInParent<Location>();
    }

    public void Tick()
    {
        index++;
        if (index == OnOffSequence.Length) index = 0;
        sr.enabled = OnOffSequence[index];
        if (sr.enabled)
        {
            if (location.hasCharacter)
            {
                location.CaptureCharacter();
            }
        }
    }

    public bool isDangerous
    {
        get
        {
            return OnOffSequence[index];
        }
    }
}
