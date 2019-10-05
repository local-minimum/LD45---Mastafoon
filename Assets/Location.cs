﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LiveUpdate { LocationToPosition, PositionToLocation, None}
[ExecuteInEditMode]
public class Location : MonoBehaviour
{
    [SerializeField] Interactable interactable;    
    [SerializeField] LiveUpdate updateMode;
    [SerializeField] Item item;

    [SerializeField]
    Vector3Int gridPosition;
    Grid grid;

    [SerializeField]
    List<Location> neighbours = new List<Location>();    

    private void OnEnable()
    {
        
        grid = GetComponentInParent<Grid>();
    }

    
    void SetWorldPosition()
    {        
        transform.position = grid.CellToWorld(gridPosition) + grid.cellSize * 0.5f;
    }

    void SetLocationFromPosition()
    {
        gridPosition = grid.WorldToCell(transform.position - grid.cellSize * 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(transform.position, Vector3.one*0.2f);
        Gizmos.color = Color.white;
        bool missingNeighbour = false;
        for (int i=0,l=neighbours.Count; i<l; i++)
        {            
            if (neighbours[i] == null)
            {
                missingNeighbour = true;
            } else
            {
                Gizmos.DrawLine(transform.position, neighbours[i].transform.position);
            }            
        }
        if (missingNeighbour)
        {
            neighbours.RemoveAll(neighbour => neighbour == null);
        }        
    }

    public void AddNeighbour(Location neighbour, bool bidirectional=true)
    {
        if (!neighbours.Contains(neighbour))
        {
            neighbours.Add(neighbour);
            if (bidirectional)
            {
                if (!neighbour.neighbours.Contains(this)) neighbour.neighbours.Add(this);
            }
        } else
        {
            Debug.LogWarning(string.Format("Attempting add already existing neighbour {0} to {1}", neighbour.name, name));
        }
    }

    public bool IsProximate(Location neighbour)
    {
        Vector3Int offset = neighbour.gridPosition - gridPosition;
        return Mathf.Abs(offset.x) + Mathf.Abs(offset.y) == 1;
    }

    public Location GetNeighbour(Vector2Int offset)
    {
        if (offset == Vector2Int.zero) return null;
        for (int i=0, l=neighbours.Count; i<l; i++)
        {
            Location neighbour = neighbours[i];
            Vector3Int neighbourOffset = neighbour.gridPosition - gridPosition;
            if (neighbourOffset.x == offset.x && neighbourOffset.y == offset.y)
            {
                return neighbour;
            }
        }
        return null;
    }

    public int Interact(Vector2Int offset)
    {
        if (interactable)
        {
            return interactable.Activate(this, offset);
        }
        return 0;
    }

    public void RemoveItem()
    {
        item = null;
    }

    public Item GetItem()
    {
        return item;
    }

    public void PlaceItem(Item item)
    {
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.gameObject.SetActive(true);
        this.item = item;
    }

    public bool PlaceEnemy(Enemy enemy)
    {
        if (!hasEnemyOrCharacter)
        {
            enemy.transform.SetParent(transform);
            enemy.transform.localPosition = Vector3.zero;
            return true;
        }
        return false;
    }

    bool hasEnemyOrCharacter
    {
        get
        {
            return GetComponentInChildren<Enemy>() || GetComponentInChildren<Character>();
        }
    }
    public bool hasCharacter
    {
        get
        {
            return GetComponentInChildren<Character>();
        }
    }

    public bool PlaceCharacter(Character character)
    {
        if (!hasEnemyOrCharacter)
        {
            character.transform.SetParent(transform);
            character.transform.localPosition = Vector3.zero;
            Environmentals[] envs = GetComponentsInChildren<Environmentals>();
            for (int i=0; i<envs.Length; i++)
            {
                if (envs[i].isDangerous)
                {
                    KillCharacter();
                }
            }
            return true;
        }
        return false;
    }

    public bool canPlaceItem
    {
        get
        {
            return item == null;
        }
    }

    public void KillCharacter()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (updateMode == LiveUpdate.LocationToPosition)
        {
            SetWorldPosition();
        } else if (updateMode == LiveUpdate.PositionToLocation)
        {
            SetLocationFromPosition();
        }
        Interactable interactable = GetComponentInChildren<Interactable>();
        if (interactable && interactable != this.interactable)
        {
            if (this.interactable)
            {
                Debug.LogError(string.Format("{0} has more than one interactable", name));
            } else
            {
                this.interactable = interactable;
                interactable.transform.localPosition = Vector3.zero;
            }
        }
        Item item = GetComponentInChildren<Item>();
        if (item && item != this.item)
        {
            if (this.item)
            {
                Debug.LogError(string.Format("{0} has more than one item!", name));
            } else
            {
                PlaceItem(item);
            }            
        }
        gameObject.name = string.Format("Node {0}, {1}", gridPosition.x, gridPosition.y);
#endif
    }
}
