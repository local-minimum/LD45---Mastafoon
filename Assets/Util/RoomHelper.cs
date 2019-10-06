﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomHelper : MonoBehaviour
{
    [SerializeField]
    bool connectLocations = false;
    [SerializeField]
    bool clearConnections = false;

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
                if (location.HeadingTo(neighbour) == Vector2Int.zero)
                {
                    Debug.LogError(string.Format("Two locations with same position {0}", neighbour));
                }
            }
        }
    }

    void ClearAllConnections()
    {
        Location[] locations = GetComponentsInChildren<Location>();
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i].ClearNeighbours();
        }

    }

    void Update()
    {
        if (clearConnections)
        {
            clearConnections = false;
            ClearAllConnections();
        }
        if (connectLocations)
        {
            connectLocations = false;
            ConnectLocations();
        }
    }
}
