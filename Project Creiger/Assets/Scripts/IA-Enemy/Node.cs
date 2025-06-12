using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // custo do nó desde o início
    public int hCost; // heurística para o fim
    public Node parent;

    public Node(bool walkable, Vector3 worldPos, int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = x;
        this.gridY = y;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
