using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Location location;

    [SerializeField]
    string narrateOnTurn;
    [SerializeField, Range(0, 1)]
    float probabilityToNarrate;

    protected Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        location = GetComponentInParent<Location>();
        FindObjectOfType<EnemyManager>().AddEnemy(this);
        speaker = GetComponent<AudioSource>();
    }

    [SerializeField, Range(0, 30)] protected int actionsPerTurn = 5;
    protected int remainingActionPoints;

    public void ResetTicks()
    {
        remainingActionPoints = actionsPerTurn;
    }
    public bool Tick() {
        if (remainingActionPoints == 0) return false;
        if (remainingActionPoints == actionsPerTurn) Narrator.ClearDisplay(narrateOnTurn);
        if (
            Random.value < probabilityToNarrate
            && !string.IsNullOrEmpty(narrateOnTurn)
            && remainingActionPoints == actionsPerTurn
        ) Narrator.ShowPieceByKey(narrateOnTurn);
        Act();
        remainingActionPoints -= 1;
        return true;
    }

    public abstract void Act();
    public abstract void Rest();

    AudioSource speaker;
    [SerializeField]
    AudioClip[] moveSounds;
    [SerializeField, Range(0, 1)]
    float moveVolume = 0.5f;
    [SerializeField]
    AudioClip[] captureSounds;
    [SerializeField, Range(0, 1)]
    float captureVolume = 1f;

    protected bool MoveTo(Location nextLocation)
    {
        if (CaptureCharacter(nextLocation))
        {
            if (captureSounds != null && captureSounds.Length > 0) speaker.PlayOneShot(captureSounds[Random.Range(0, captureSounds.Length)], captureVolume);
            nextLocation.PlaceEnemy(this, false, true);
            nextLocation.CaptureCharacter();
            return true;            
        }
        if (nextLocation.PlaceEnemy(this)) {
            if (moveSounds != null && moveSounds.Length > 0) StartCoroutine(DelayedMoveSound());
            location = nextLocation;            
            return true;
        };
        return false;
    }

    IEnumerator<WaitForSeconds> DelayedMoveSound()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.1f));
        speaker.PlayOneShot(moveSounds[Random.Range(0, moveSounds.Length)], moveVolume);
    }

    protected bool CaptureCharacter(Location nextLocation)
    {
        return nextLocation.hasCharacter;
    }

    protected Location AttemptCapture(int maxDistance)
    {
        List<Location> path = location.FindPathToCharacter(maxDistance);
        return path == null ? null : path[0];

    }
}
