using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeaconSequence { Bounce, Wrap}
public class Beaconer : Enemy
{
    [SerializeField]
    Item[] beaconSequence;
    [SerializeField]
    int nextBeaconIndex = 0;
    int nextBeaconStep = 1;
    [SerializeField] BeaconSequence sequence;

    public override void Act()
    {
        Location neighbour = FindNextLocation();
        if (neighbour)
        {
            MoveTo(neighbour);
        }
    }

    Location FindNextLocation()
    {
        const int lookAllMap = -1;
        const int maxCaptureDistance = 1;

        // First look around for character
        Location target = AttemptCapture(maxCaptureDistance);

        // Second attempt finding path to next beacon
        if (target == null)
        {
            Item beacon = beaconSequence[nextBeaconIndex];
            List<Location> trail = location.FindPathTo(loc => loc.GetItem() == beacon, lookAllMap);
            if (trail != null)
            {
                target = trail[0];
                if (trail.Count == 1)
                {                    
                    nextBeaconIndex += nextBeaconStep;                    
                    if (nextBeaconIndex >= beaconSequence.Length)
                    {
                        if (sequence == BeaconSequence.Bounce)
                        {
                            nextBeaconStep = -1;
                            nextBeaconIndex = beaconSequence.Length - 2;
                        } else
                        {
                            nextBeaconIndex = 0;
                        }
                    }
                    if (nextBeaconIndex < 0)
                    {
                        if (sequence == BeaconSequence.Bounce)
                        {
                            nextBeaconStep = 1;
                            nextBeaconIndex = 1;
                        } else
                        {
                            nextBeaconIndex = beaconSequence.Length - 1;
                        }
                    }
                }
            }
        }

        //Third, if beacon is missing, player has it
        if (target == null)
        {
            Debug.Log("Hunting player");
            target = AttemptCapture(lookAllMap);
        }
        return target;
    }
}
