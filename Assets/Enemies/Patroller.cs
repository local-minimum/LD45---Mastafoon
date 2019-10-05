using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatrollerPattern { TurnLeft, TurnRight, Bounce };

public class Patroller : Enemy
{
    [SerializeField]
    Vector2Int startHeading;

    [SerializeField]
    Vector2Int heading;

    [SerializeField] List<string> aftraidOf = new List<string>();
    [SerializeField] PatrollerPattern pattern;

    private void Start()
    {
        heading = startHeading;
    }

    public override void Act()
    {
        for (int i=0; i<4; i++)
        {
            if (Walk()) break;
        }
    }

    bool Walk()
    {
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

    void Turn()
    {
        switch (pattern)
        {
            case PatrollerPattern.Bounce:
                heading = new Vector2Int(-heading.x, -heading.y);
                break;
            case PatrollerPattern.TurnLeft:
                heading = new Vector2Int(-heading.y, heading.x);
                break;
            case PatrollerPattern.TurnRight:
                heading = new Vector2Int(heading.y, -heading.x);
                break;
        }
    }
}
