using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    public static List<PathNode> FindPath(Vector2 startWorldPos, Vector2 endWorldPos)
    {
        GridManager grid = GridManager.Instance;
        PathNode startNode = grid.GetNode(startWorldPos);
        PathNode endNode = grid.GetNode(endWorldPos);

        List<PathNode> openList = new List<PathNode> { startNode };
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        foreach (PathNode node in grid.GetGrid())
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, endNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.hCost < currentNode.hCost))
                    currentNode = node;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
                return RetracePath(startNode, endNode);

            foreach (PathNode neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedList.Contains(neighbor)) continue;

                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newGCost < neighbor.gCost)
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }

    private static List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private static int GetDistance(PathNode a, PathNode b)
    {
        int dstX = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dstY = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return 10 * (dstX + dstY);
    }

    private static List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();
        GridManager grid = GridManager.Instance;
        PathNode[,] gridArray = grid.GetGrid();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridPosition.x + x;
                int checkY = node.gridPosition.y + y;

                if (checkX >= 0 && checkX < gridArray.GetLength(0) && checkY >= 0 && checkY < gridArray.GetLength(1))
                {
                    neighbors.Add(gridArray[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
}

