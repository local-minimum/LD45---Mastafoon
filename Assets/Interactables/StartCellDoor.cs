using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCellDoor : Interactable
{
    [SerializeField] Location openFrom;
    [SerializeField] Location openTo;
    bool isClosed = true;    
    [SerializeField] GameObject worldDoor;
    [SerializeField] GameObject[] roomsToActivate;
    [SerializeField] string[] dialogues;
    int dialgueIndex = 0;

    private void Awake()
    {
        worldDoor.SetActive(isClosed);
        for (int i=0; i<roomsToActivate.Length; i++)
        {
            roomsToActivate[i].SetActive(!isClosed);
        }
    }
    public override int Activate(Location location, Vector2Int offset)
    {
        if (isClosed && location == openFrom && offset == Vector2Int.right)
        {
            if (dialgueIndex < dialogues.Length)
            {
                Narrator.ShowPieceByKey(dialogues[dialgueIndex]);
                dialgueIndex++;
            } else
            {
                Narrator.ClearDisplay();
                openFrom.AddNeighbour(openTo);
                isClosed = false;
                for (int i = 0; i < roomsToActivate.Length; i++)
                {
                    roomsToActivate[i].SetActive(true);
                }
                worldDoor.SetActive(false);
            }
            return 1;
        }
        return 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (openFrom && openTo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(openFrom.transform.position, openTo.transform.position);
        }
    }
}
