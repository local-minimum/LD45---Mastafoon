using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatrollerPattern { TurnLeft, TurnRight, Bounce};

public class Patroller : Enemy
{
    [SerializeField]
    Vector2Int startHeading;

    [SerializeField]
    Vector2Int heading;

    [SerializeField] List<string> afraidOf = new List<string>();
    [SerializeField] List<string> afraidMsg = new List<string>();

    [SerializeField] PatrollerPattern pattern;
    [SerializeField] string wakupMessage;
    bool emittedMesasge;
    bool wasWalking = false;
    private void Start()
    {
        heading = startHeading;
    }

    public override void Rest()
    {
        anim.SetTrigger("Rest");
        wasWalking = false;
    }

    public override void Act()
    {
        if (!string.IsNullOrEmpty(wakupMessage))
        {
            if (remainingActionPoints == actionsPerTurn)
            {
                if (!emittedMesasge)
                {
                    Narrator.ShowPieceByKey(wakupMessage);
                    emittedMesasge = true;
                }
                else
                {
                    Narrator.ClearDisplay(wakupMessage);
                    wakupMessage = "";
                }
            }            
        }
        for (int i=0; i<4; i++)
        {
            if (Walk()) break;
        }
    }

    bool Walk()
    {
        int maxSearchDistance = 1;
        Location characterTrail = AttemptCapture(maxSearchDistance);
        if (characterTrail)
        {
            heading = location.HeadingTo(characterTrail);
        }
        Location neighbour = location.GetNeighbour(heading);
        if (neighbour)
        {
            Item item = neighbour.GetItem();
            bool afraid = item != null && afraidOf.Contains(item.tag);
            if (afraid) EmitAfraidMessage(item.tag);
            if (afraid && !CaptureCharacter(neighbour) || !MoveTo(neighbour))
            {
                Turn();
                return false;
            }
            if (!wasWalking)
            {
                wasWalking = true;
                SetWalkAnimFromHeading(heading);
            }
            return true;
        }
        Turn();
        return false;
    }

    void EmitAfraidMessage(string tag)
    {
        int pos = afraidOf.IndexOf(tag);
        if (pos >= afraidMsg.Count) return;
        string msg = afraidMsg[pos];
        if (!string.IsNullOrEmpty(msg)) Narrator.ShowPieceByKey(msg);
    }

    Vector2Int GetTurnHeading(PatrollerPattern pattern)
    {
        switch (pattern)
        {
            case PatrollerPattern.TurnLeft:
                return new Vector2Int(-heading.y, heading.x);
            case PatrollerPattern.TurnRight:
                return new Vector2Int(heading.y, -heading.x);
            case PatrollerPattern.Bounce:
                return new Vector2Int(-heading.x, -heading.y);
        }
        throw new System.Exception("Illegal turn pattern");
    }

    bool SetHeadingIfExists(PatrollerPattern pattern)
    {
        Vector2Int nextHeading = GetTurnHeading(pattern);
        if (location.GetNeighbour(nextHeading))
        {
            if (heading != nextHeading)
            {
                SetWalkAnimFromHeading(nextHeading);
            }
            heading = nextHeading;
            return true;
        }
        return false;
    }

    void Turn()
    {
        if (SetHeadingIfExists(pattern)) return;
        if (pattern != PatrollerPattern.TurnLeft)
        {
            if (SetHeadingIfExists(PatrollerPattern.TurnLeft))
            {
                pattern = PatrollerPattern.TurnLeft;
                return;
            }
        }
        if (pattern != PatrollerPattern.TurnRight)
        {
            if (SetHeadingIfExists(PatrollerPattern.TurnRight)) {
                pattern = PatrollerPattern.TurnRight;
                return;
            }
        }
        if (pattern != PatrollerPattern.Bounce)
        {
            if (SetHeadingIfExists(PatrollerPattern.Bounce)) return;
        }
    }
}
