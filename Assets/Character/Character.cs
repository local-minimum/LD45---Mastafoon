﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] WorldClock worldClock;
    [SerializeField] Location start;
    [SerializeField] string messageOnLoad;
    ActionsInventory actionsInventory;
    Narrator narrator;
    Location location;
    int steps = 0;

    public int stepsTaken
    {
        get
        {
            return steps;
        }
    }

    public Location GetLocation()
    {
        return location;
    }

    void Start()
    {
        actionsInventory = FindObjectOfType<ActionsInventory>();
        narrator = FindObjectOfType<Narrator>();
        location = start;
        location.PlaceCharacter(this);
        if (!string.IsNullOrEmpty(messageOnLoad)) narrator.ShowPieceByKey(messageOnLoad);
    }

    private void OnEnable()
    {
        worldClock.OnTick += WorldClock_OnTick;
    }

    private void OnDisable()
    {
        worldClock.OnTick -= WorldClock_OnTick;
    }


    private void WorldClock_OnTick(Turn turn, bool changeOfTurn)
    {

        if (turn == Turn.Player)
        {
            if (changeOfTurn) actionsInventory.NewTurn();
            if (actionsInventory.remainingActions == 0)
            {
                worldClock.GiveTurnTo(Turn.Enemies);
                actionsInventory.EnemyTurn();
            }
        }
    }
    
    void GetMove()
    {
        int actionPoints = actionsInventory.remainingActions;
        Vector2Int offset = Vector2Int.zero;
        if (actionPoints <= 0) return;
        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                offset = Vector2Int.up;
            } else
            {
                offset = Vector2Int.down;
            }                        
            
        } else if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                offset = Vector2Int.right;
            } else
            {
                offset = Vector2Int.left;
            }            
        }
        Location nextLocation = location.GetNeighbour(offset);
        if (nextLocation)
        {
            if (nextLocation.PlaceCharacter(this))
            {
                location = nextLocation;
                actionsInventory.UseAction();
                steps++;
                Item item = location.GetItem();
                if (item && actionsInventory.canPickUp)
                {
                    actionsInventory.PickUp(item, location);
                }
            }
        } else
        {
            int points = location.Interact(offset);
            if (points > 0) actionsInventory.UseAction();            
        }
        if (actionsInventory.remainingActions == 0)
        {
            worldClock.GiveTurnTo(Turn.Enemies);
            actionsInventory.EnemyTurn();
        }
    }

    void Update()
    {
        GetMove();
    }
}
