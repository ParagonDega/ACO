using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid worldGrid;
    ForwardMovementManager forwardMovementManager;
    BackwardsPathfindingManager pathRequestManager;

    int maskSize = 5;

    void Awake()
    {
        worldGrid = GetComponent<Grid>();
        forwardMovementManager = GetComponent<ForwardMovementManager>();
        pathRequestManager = GetComponent<BackwardsPathfindingManager>();
    }

    public void RequestMove(Vector3 currentPos, Vector3 oldPos)
    {
        StartCoroutine(FindNext(currentPos, oldPos));
    }

    public void StartFindPath(Vector3[] path)
    {
        StartCoroutine(RetracePath(path));
    }

    IEnumerator FindNext(Vector3 currentPos, Vector3 oldPos)
    {
        Node currentNode = worldGrid.NodeFromWorldPoint(currentPos);
        Node lastNode = worldGrid.NodeFromWorldPoint(oldPos);
        Node targetNode;
        //Select neighbour
        targetNode = worldGrid.NextNode(currentNode, lastNode, 1);
        yield return null;
        forwardMovementManager.FinishedProcessingMove(targetNode.worldPos, true);
    }

    IEnumerator RetracePath(Vector3[] path)
    {
        bool pathSucces = false;
        List<Node> newPath = new List<Node>();
        for (int i = 0; i < path.Length; i++)
        {
            newPath.Add(worldGrid.NodeFromWorldPoint(path[i]));
        }
        List<Node> returnedPath = DetectLoop(newPath);
        Vector3[] returnPath = SimplifyPath(returnedPath);

        Array.Reverse(returnPath);
        pathSucces = true;
        yield return null;
        pathRequestManager.FinishedProcessingPath(returnPath,pathSucces);

    }

    List<Node> DetectLoop(List<Node> path)                  //detects loops in path and removes them
    {
        for(int i = 1; i < path.Count; i++)                 //Starting from the second element, loop through all elements
        {
            for (int j = i + 1; j - i <= maskSize && j < path.Count; j++)     //Starting from the element after i, check the next x elements up to the size of mask
            {
                if(path[i].worldPos == path[j].worldPos)    //if the world location of i is the same as the world location of j then there is a loop
                {
                    for(int k = i+1; k <= j && k < path.Count; k++)
                    {
                        path.RemoveAt(k);                   //Remove all elements after i, up to and including duplicate
                    }
                    //path.RemoveAll(x => x == null);         //Remove all null elements from path
                    j = i + 1;                                  //Reset j to continue checking for loops
                }
            }
        }
        return path;
    }

    Vector3[] SimplifyPath(List<Node> path)                 //Converts node list to vec3 array
    {
        List<Vector3> waypoints = new List<Vector3>();

        for (int i = 0; i < path.Count-1; i++)
        {
                waypoints.Add(path[i].worldPos);
        }
        return waypoints.ToArray();
    }

}

