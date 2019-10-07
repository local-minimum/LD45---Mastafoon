using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environmentals : MonoBehaviour
{
    [SerializeField]
    bool[] OnOffSequence;
    int index = -1;
    [SerializeField]
    SpriteRenderer sr;
    Location location;
    protected AudioSource speakers;

    private void Start()
    {
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        FindObjectOfType<EnvironmentManager>().Add(this);
        location = GetComponentInParent<Location>();
        speakers = GetComponentInChildren<AudioSource>();
    }

    public void Tick()
    {
        index++;
        if (index == OnOffSequence.Length) index = 0;
        sr.enabled = location.hasEnemy ? false : OnOffSequence[index];
        if (sr.enabled)
        {
            OnEffect();
            if (location.hasCharacter)
            {
                location.CaptureCharacter();
            }
        }
    }

    protected virtual void OnEffect()
    {

    }

    public bool isDangerous
    {
        get
        {
            return OnOffSequence[index];
        }
    }
}
