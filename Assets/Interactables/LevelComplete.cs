using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : Interactable
{    
    [SerializeField] Vector2Int offset;

    SessionManager session;
    private void Start()
    {
        session = FindObjectOfType<SessionManager>();
    }

    public override int Activate(Location location, Vector2Int offset)
    {
        if (offset == this.offset)
        {
            Character character = location.GetComponentInChildren<Character>();
            int steps = character.stepsTaken;
            int turns = FindObjectOfType<WorldClock>().turnsTaken;
            session.ReportLevelCompleted(steps, turns);
            return 1;
        }
        return 0;
    }
}
