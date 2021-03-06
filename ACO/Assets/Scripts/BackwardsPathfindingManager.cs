﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardsPathfindingManager : MonoBehaviour //Backwards pathfinding
{

    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;
    static BackwardsPathfindingManager instance;
    private  Pathfinding pathfinding;
    private bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3[] path, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(path, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.path);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3[] path;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3[] _path, Action<Vector3[],bool> _callback)
        {
            path = _path;
            callback = _callback;
        }
    }
}
