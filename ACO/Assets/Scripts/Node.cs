using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX, gridY;
    public const float pheremoneInit = 4.0f;
    public float alpha = 5.0f, pheremone, probability;

    public Node(bool _walkable, Vector3 _wPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _wPos;
        gridX = _gridX;
        gridY = _gridY;
        pheremone = pheremoneInit;
    }
    public float GetInitialPheremone()
    {
        return pheremoneInit;
    }

    public void changeWeight(float newWeight)
    {
        if( pheremone > 1 || pheremone < pheremoneInit)
        {
            pheremone += newWeight;
        }
    }

    public float fCost
    {
        get
        {
            return alpha + pheremone;
        }
    }
}
