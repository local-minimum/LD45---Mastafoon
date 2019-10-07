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
            FindObjectOfType<WorldClock>().GiveTurnTo(Turn.None);            
            session.ReportLevelCompleted();
            return 1;
        }
        return 0;
    }
}
