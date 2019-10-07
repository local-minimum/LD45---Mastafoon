using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LiveUpdate { LocationToPosition, PositionToLocation, None}
[ExecuteInEditMode]
public class Location : MonoBehaviour
{
    WorldClock worldClock;    

    [SerializeField] LiveUpdate updateMode;
    Item item;

    [SerializeField]
    Vector3Int gridPosition;
    Grid grid;

    [SerializeField]
    List<Location> neighbours = new List<Location>();

    private void OnEnable()
    {
        worldClock = FindObjectOfType<WorldClock>();
        item = GetComponentInChildren<Item>();
        if (item) PlaceItem(item);
        Character character = GetComponentInChildren<Character>();
        if (character) PlaceCharacter(character);
        Enemy enemy = GetComponentInChildren<Enemy>();
        if (enemy) PlaceEnemy(enemy, true);
        grid = GetComponentInParent<Grid>();
    }


    void SetWorldFromGridPosition()
    {
        transform.position = grid.GetCellCenterWorld (gridPosition) + grid.cellSize * 0.5f;
    }

    void SetGridPositionFromWorld()
    {
        gridPosition = grid.WorldToCell(transform.position - grid.cellSize * 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);
        bool missingNeighbour = false;
        for (int i = 0, l = neighbours.Count; i < l; i++)
        {
            if (neighbours[i] == null)
            {
                missingNeighbour = true;
            } else
            {
                Gizmos.color = neighbours[i].neighbours.Contains(this) ? Color.white : Color.cyan;
                Gizmos.DrawLine(transform.position, neighbours[i].transform.position);
            }
        }
        if (missingNeighbour)
        {
            neighbours.RemoveAll(neighbour => neighbour == null);
        }
    }

    public void AddNeighbour(Location neighbour, bool bidirectional = true)
    {
        if (!neighbours.Contains(neighbour))
        {
            neighbours.Add(neighbour);
            if (bidirectional)
            {
                if (!neighbour.neighbours.Contains(this)) neighbour.neighbours.Add(this);
            }
        }
    }

    public void ClearNeighbours()
    {
        neighbours.Clear();
    }

    public bool IsProximate(Location neighbour)
    {
        Vector3Int offset = neighbour.gridPosition - gridPosition;
        return Mathf.Abs(offset.x) + Mathf.Abs(offset.y) == 1;
    }

    public Location GetNeighbour(Vector2Int offset)
    {
        if (offset == Vector2Int.zero) return null;
        for (int i = 0, l = neighbours.Count; i < l; i++)
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

    public Interactable interactable
    {
        get
        {
            return GetComponentInChildren<Interactable>();
        }
    }

    public int Interact(Vector2Int offset)
    {
        if (interactable && offset.magnitude > 0)
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

    public bool PlaceEnemy(Enemy enemy, bool noTransition = false, bool force = false)
    {
        if (force || !hasEnemyOrCharacter(enemy))
        {
            enemy.transform.SetParent(transform);
            if (noTransition)
            {
                enemy.transform.localPosition = Vector3.zero;
            } else
            {
                StartCoroutine(_AnimateEnemyTransition(enemy.transform));
            }
            
            return true;
        }
        return false;
    }

    IEnumerator<WaitForSeconds> _AnimateEnemyTransition(Transform enemyTrans){
        float start = Time.timeSinceLevelLoad;
        float duration = worldClock.enemyMoveDuration * worldClock.tickDuration;
        if (duration > 0)
        {
            float progress = 0;
            Vector3 startPos = enemyTrans.position;
            while (progress < 1f)
            {
                yield return new WaitForSeconds(0.02f);
                progress = Mathf.Min((Time.timeSinceLevelLoad - start) / duration, 1);
                enemyTrans.position = Vector3.Lerp(startPos, transform.position, worldClock.enemyMoveEase.Evaluate(progress));
            }
        }
        enemyTrans.localPosition = Vector3.zero;
    }

    bool hasEnemyOrCharacter(Enemy enemy)
    {
        Enemy existing = GetComponentInChildren<Enemy>();
        return hasCharacter || existing != null && existing != enemy;        
    }

    bool hasEnemyOrCharacter(Character character)
    {
        Character existing = GetComponentInChildren<Character>();
        return hasEnemy || existing != null && existing != character;
    }


    public bool hasCharacter
    {
        get
        {
            return GetComponentInChildren<Character>();
        }
    }

    public bool hasEnemy
    {
        get
        {
            return GetComponentInChildren<Enemy>();
        }
    }

    public bool PlaceCharacter(Character character)
    {
        if (!hasEnemyOrCharacter(character))
        {
            character.transform.SetParent(transform);
            character.transform.localPosition = Vector3.zero;
            CheckEnvironmentalCapture();
            return true;
        }
        return false;
    }

    bool hasCaptured = false;
    public bool CheckEnvironmentalCapture()
    {
        if (hasCaptured) return true;
        Environmentals[] envs = GetComponentsInChildren<Environmentals>();
        for (int i = 0; i < envs.Length; i++)
        {
            if (envs[i].isDangerous)
            {
                hasCaptured = true;
                CaptureCharacter();
                return true;
            }
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

    public void CaptureCharacter()
    {
        WorldClock worldClock = FindObjectOfType<WorldClock>();
        worldClock.GiveTurnTo(Turn.None);
        FindObjectOfType<SessionManager>().ImprisonCharacter();
    }

    public void Position()
    {
        if (updateMode == LiveUpdate.LocationToPosition)
        {
            SetWorldFromGridPosition();
        }
        else if (updateMode == LiveUpdate.PositionToLocation)
        {
            SetGridPositionFromWorld();
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        Position();
        Interactable interactable = this.interactable;
        if (GetComponentsInChildren<Interactable>().Length > 1)
        {
            Debug.LogError(string.Format("{0} has more than one interactable", name));
        } else if (interactable != null)
        {
            interactable.transform.localPosition = Vector3.zero;
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

    public List<Location> FindPathToCharacter(int maxDistance)
    {
        return FindPathTo(loc => loc.hasCharacter, maxDistance);
    }

    public List<Location> FindPathTo(System.Func<Location, bool> targetCheck, int maxDistance)
    {
        List<Location> seen = new List<Location>();
        seen.Add(this);
        Queue<List<Location>> paths = new Queue<List<Location>>();
        Location currentLocation = this;
        List<Location> currentPath = new List<Location>();
        while (true)
        {
            foreach(List<Location> path in currentLocation.Neighbours(currentPath, seen))
            {
                Location possibleTarget = path[path.Count - 1];
                if (targetCheck(possibleTarget))
                {
                    return path;
                }
                seen.Add(possibleTarget);
                if (maxDistance < 0 || path.Count < maxDistance) paths.Enqueue(path);
            }            
            if (paths.Count == 0) break;
            currentPath = paths.Dequeue();
            currentLocation = currentPath[currentPath.Count - 1];
        }
        return null;
    }

    IEnumerable<List<Location>> Neighbours(List<Location> path, List<Location> seen)
    {
        for (int i=0, l=neighbours.Count; i<l; i++)
        {
            Location neighbour = neighbours[i];
            if (seen.Contains(neighbour)) continue;
            List<Location> currentPath = new List<Location>(path);
            currentPath.Add(neighbour);
            yield return currentPath;
        }
    }

    public Vector2Int HeadingTo(Location other)
    {
        Vector3Int offset = other.gridPosition - gridPosition;
        return new Vector2Int(offset.x, offset.y);
    }
}
