using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCellDoor : Interactable
{
    [SerializeField] Location openFrom;
    [SerializeField] Location openTo;
    bool isClosed = true;
    [SerializeField] GameObject shownWhenOpen;
    [SerializeField] GameObject worldDoor;
    [SerializeField] GameObject[] roomsToActivate;
    private void Awake()
    {
        shownWhenOpen.SetActive(!isClosed);
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
            openFrom.AddNeighbour(openTo);
            shownWhenOpen.SetActive(true);
            isClosed = false;
            for (int i = 0; i < roomsToActivate.Length; i++)
            {
                roomsToActivate[i].SetActive(true);
            }
            worldDoor.SetActive(false);
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
