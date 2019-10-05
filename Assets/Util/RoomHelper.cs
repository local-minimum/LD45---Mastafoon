using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomHelper : MonoBehaviour
{
    [SerializeField]
    bool connectLocations = false;

    void ConnectLocations()
    {
        Location[] locations = GetComponentsInChildren<Location>();
        for (int i=0; i<locations.Length; i++)
        {
            Location location = locations[i];
            for (int j=i+1; j<locations.Length; j++)
            {
                Location neighbour = locations[j];
                if (location.IsProximate(neighbour))
                {
                    location.AddNeighbour(neighbour);
                }
            }
        }
    }

    void Update()
    {
        if (connectLocations)
        {
            connectLocations = false;
            ConnectLocations();
        }
    }
}
