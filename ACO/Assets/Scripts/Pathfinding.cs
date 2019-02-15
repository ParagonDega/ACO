using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    Grid worldGrid;
    MoveRequestManager moveRequestManager;
    PathRequestManager pathRequestManager;

    void Awake()
    {
        worldGrid = GetComponent<Grid>();
        moveRequestManager = GetComponent<MoveRequestManager>();
        pathRequestManager = GetComponent<PathRequestManager>();
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
        moveRequestManager.FinishedProcessingMove(targetNode.worldPos, true);
    }

    IEnumerator RetracePath(Vector3[] path)
    {
        bool pathSucces = false;
        List<Node> newPath = new List<Node>();
        for (int i = 0; i < path.Length; i++)
        {
            newPath.Add(worldGrid.NodeFromWorldPoint(path[i]));
        }
        //List<Node> returnedPath = DetectLoop(newPath);
        //Vector3[] returnPath = SimplifyPath(returnedPath);
        Vector3[] returnPath = SimplifyPath(newPath);

        Array.Reverse(returnPath);
        pathSucces = true;
        yield return null;
        pathRequestManager.FinishedProcessingPath(returnPath,pathSucces);

    }

    List<Node> DetectLoop(List<Node> path)                                                                  //Recursively detects loops in path and removes them
    {
        List<Node> updatePath = new List<Node>();

        for (int i = 1; i < path.Count; i++)
            for (int j = 1; j < path.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                if (path[i].worldPos == path[j].worldPos)
                {
                    for (int k = 0; k < path.Count; k++)
                    {
                        if (k <= i || k > j)
                        {
                            updatePath.Add(path[i]);
                        }
                    }
                    updatePath = DetectLoop(updatePath);      //Recursion
                }
            }
        return updatePath;

    }

    Vector3[] SimplifyPath(List<Node> path)                                                                 //Converts node list to vec3 array
    {
        List<Vector3> waypoints = new List<Vector3>();

        for (int i = 0; i < path.Count-1; i++)
        {
                waypoints.Add(path[i].worldPos);
        }
        return waypoints.ToArray();
    }

}

