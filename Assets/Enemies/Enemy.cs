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
    protected bool MoveTo(Location nextLocation)
    {
        if (CaptureCharacter(nextLocation))
        {            
            nextLocation.KillCharacter();
            return true;            
        }
        if (nextLocation.PlaceEnemy(this)) {
            location = nextLocation;            
            return true;
        };
        return false;
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
