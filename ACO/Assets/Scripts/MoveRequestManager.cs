using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRequestManager : MonoBehaviour
{

    Queue<MoveRequest> moveRequestQueue = new Queue<MoveRequest>();
    MoveRequest currentMoveRequest;

    static MoveRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestMove(Vector3 currentPosition, Vector3 oldPosition, Action<Vector3, bool> callback)
    {
        MoveRequest newRequest = new MoveRequest(currentPosition, oldPosition, callback);
        instance.moveRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && moveRequestQueue.Count >0)
        {
            currentMoveRequest = moveRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.RequestMove(currentMoveRequest.currentPosition, currentMoveRequest.oldPosition);
        }
    }

    public void FinishedProcessingMove(Vector3 pos, bool success)
    {
        currentMoveRequest.callback(pos, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct MoveRequest
    {
        public Vector3 currentPosition, oldPosition;
        public Action<Vector3, bool> callback;

        public MoveRequest(Vector3 _start, Vector3 _old, Action<Vector3, bool> _callback)
        {
            currentPosition = _start;
            oldPosition = _old;
            callback = _callback;
        }
    }
}
