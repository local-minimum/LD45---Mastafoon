using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] WorldClock worldClock;
    [SerializeField] Location start;
    ActionsInventory actionsInventory;
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
        location = start;
        location.PlaceCharacter(this);
    }

    private void OnEnable()
    {
        worldClock.OnTick += WorldClock_OnTick;
    }

    private void OnDisable()
    {
        worldClock.OnTick -= WorldClock_OnTick;
    }

    Turn lastTurn = Turn.Player;
    private void WorldClock_OnTick(Turn turn)
    {
        if (lastTurn != turn)
        {
            if (turn == Turn.Player) actionsInventory.NewTurn();
            lastTurn = turn;            
        } else if (turn == Turn.Player)
        {
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
