using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Gridded : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] Vector3Int gridPosition;
    [SerializeField] bool snap;

    Grid grid;

    private void Start()
    {
        grid = GetComponentInParent<Grid>();
    }

    void Update()
    {
        if (snap)
        {
            transform.position = grid.GetCellCenterWorld(gridPosition) + grid.cellSize * 0.5f;
        }

    }
#endif
}
