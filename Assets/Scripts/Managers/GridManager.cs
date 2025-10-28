using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 50;
    public int height = 50;
    public float cellSize = 1f;
    public LayerMask obstacleMask;

    private PathNode[,] grid;

    private void Awake()
    {
        Instance = this;
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new PathNode[width, height];

        Vector2 origin = transform.position;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = origin + new Vector2(x, y) * cellSize;
                bool walkable = !Physics2D.OverlapCircle(worldPos, cellSize * 0.4f, obstacleMask);
                grid[x, y] = new PathNode(new Vector2Int(x, y), walkable);
            }
        }
    }

    public PathNode GetNode(Vector2 worldPos)
    {
        Vector2 origin = transform.position;
        int x = Mathf.Clamp(Mathf.RoundToInt((worldPos.x - origin.x) / cellSize), 0, width - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((worldPos.y - origin.y) / cellSize), 0, height - 1);
        return grid[x, y];
    }

    public Vector2 GetWorldPosition(Vector2Int gridPos)
    {
        Vector2 origin = transform.position;
        return origin + new Vector2(gridPos.x, gridPos.y) * cellSize;
    }

    public PathNode[,] GetGrid() => grid;
}

