using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : Interactable
{
    [SerializeField] WorldClock worldClock;

    SessionManager session;
    private void Start()
    {
        session = FindObjectOfType<SessionManager>();
    }

    public override int Activate(Location location, Vector2Int offset)
    {
        Character character = location.GetComponent<Character>();
        session.ReportLevelCompleted(character.stepsTaken, worldClock.turnsTaken);
        return 1;
    }
}
