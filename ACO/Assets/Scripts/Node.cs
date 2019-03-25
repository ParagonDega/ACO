using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX, gridY;
    public const float pheremoneInit = 8.0f;
    public float alpha = 10.0f, pheremone, probability;

    public GameObject food;

    public Node(bool _walkable, Vector3 _wPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _wPos;
        gridX = _gridX;
        gridY = _gridY;
        pheremone = pheremoneInit;
    }
    public float GetInitialPheremone(){return pheremoneInit;}


    public void changeWeight(float newWeight)
    {
        if( pheremone > 1 && pheremone <= pheremoneInit)
        {
            pheremone += newWeight;
            if (pheremone < 1)
            {
                pheremone = 1.0f;
            }else if (pheremone>pheremoneInit){
                pheremone = pheremoneInit;
            }
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
