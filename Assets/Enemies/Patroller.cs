﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatrollerPattern { TurnLeft, TurnRight, Bounce};

public class Patroller : Enemy
{
    [SerializeField]
    Vector2Int startHeading;

    [SerializeField]
    Vector2Int heading;

    [SerializeField] List<string> aftraidOf = new List<string>();
    [SerializeField] PatrollerPattern pattern;
    [SerializeField] string wakupMessage;
    bool emittedMesasge;
    private void Start()
    {
        heading = startHeading;
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
            if (item && aftraidOf.Contains(item.tag) && !CaptureCharacter(neighbour) || !MoveTo(neighbour))
            {
                Turn();
                return false;
            }
            return true;
        }
        Turn();
        return false;
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
