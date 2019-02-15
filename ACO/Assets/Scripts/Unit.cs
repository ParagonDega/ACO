﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    List<Vector3> traveledNodes = new List<Vector3>();
    float speed = 5.0f;
    Vector3 test = Vector3.zero, targetPos, checkPosition, currentWaypoint;
    Vector3[] path;
    const float minPathUpdateTime = .2f;
    bool returnToNest = false, moving = false;
    int targetIndex, energy = 30, storage=0;

    private void Start()
    {
        targetPos = transform.position;
        StartCoroutine(UpdateMove());
    }

    private void Update()
    {
        if(traveledNodes.Count >= energy && !returnToNest)
        {
            ReturnToNest();
        }
    }

    IEnumerator UpdateMove()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        if (!moving)
        {
            traveledNodes.Add(transform.position);
            MoveRequestManager.RequestMove(transform.position, targetPos, OnReturn);
        }
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if (!moving)
            {
                traveledNodes.Add(transform.position);
                MoveRequestManager.RequestMove(transform.position, targetPos, OnReturn);
            }
        }
    }

    public void OnReturn(Vector3 moveLocation, bool moveSuccesful)
    {
        if (moveSuccesful && !returnToNest && !moving)
        {
            StopCoroutine("UnitSearch");
            targetPos = moveLocation;
            moving = true;
            StartCoroutine("UnitSearch");
        }
    }

    IEnumerator UnitSearch()
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            if (dist<=0.4f)
            {
                moving = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

    }

    public void OnPathFound(Vector3[] newPath, bool moveSuccesful)
    {

        if (moveSuccesful && returnToNest && !moving)
        {
            StopCoroutine("FollowPath");
            path = newPath;
            moving = true;
            currentWaypoint = path[targetIndex];
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, currentWaypoint);

            if (dist <= 0.4f)
            {
                Debug.Log("Target Reached");

                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    traveledNodes.Clear();
                    returnToNest = false;
                    moving = false;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.name == "Ant")
        {
            Debug.Log(collision.collider.name);
            if (!returnToNest)
            {
                moving = false;
            }

        }

        if (collision.collider.name == "Honey" && !returnToNest)
        {
            Debug.Log(collision.collider.name);
            storage++;
            ReturnToNest();
        }
    }

    private void ReturnToNest()
    {
        StopCoroutine("UnitSearch");
        moving = false;
        targetIndex = 0;
        targetPos = test;
        returnToNest = true;
        PathRequestManager.RequestPath(traveledNodes.ToArray(), OnPathFound);
    }

    public void OnDrawGizmos()
    {
        if (targetPos != test)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(targetPos, Vector3.one);
        }
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}

