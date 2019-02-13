using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX, gridY;

    public int gCost, hCost;

    public Node(bool _walkable, Vector3 _wPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _wPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
