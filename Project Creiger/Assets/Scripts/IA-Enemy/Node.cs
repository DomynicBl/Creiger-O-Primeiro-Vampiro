using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Node : IHeapItem<Node> {
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX, gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;

    int heapIndex;
    public int HeapIndex {
        get => heapIndex;
        set => heapIndex = value;
    }

    public int CompareTo(Node other) {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(other.hCost);
        }
        return compare;
    }

    public Node(bool walkable, Vector2 worldPos, int x, int y) {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = x;
        this.gridY = y;
    }
}

