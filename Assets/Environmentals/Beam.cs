using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Environmentals
{
    [SerializeField] AudioClip beam;

    protected override void OnEffect()
    {
        StartCoroutine(Spark());
    }

    IEnumerator<WaitForSeconds> Spark()
    {
        yield return new WaitForSeconds(Random.value * 0.1f);
        speakers.PlayOneShot(beam);
    }
}
