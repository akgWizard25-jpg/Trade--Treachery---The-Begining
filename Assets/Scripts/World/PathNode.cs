using UnityEngine;

public class PathNode
{
    public Vector2Int gridPosition;
    public bool walkable;
    public int gCost, hCost;
    public int FCost => gCost + hCost;
    public PathNode parent;

    public PathNode(Vector2Int gridPosition, bool walkable)
    {
        this.gridPosition = gridPosition;
        this.walkable = walkable;
    }
}
