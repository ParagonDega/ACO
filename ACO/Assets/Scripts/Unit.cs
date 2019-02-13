using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    List<Vector3> traveledNodes = new List<Vector3>();
    float speed = 10;
    Vector3 targetPos;
    Vector3[] path;
    const float minPathUpdateTime = .2f;
    bool foundFood = false;
    int targetIndex;

    private void Start()
    {
        targetPos = transform.position;
        StartCoroutine(UpdateMove());
    }

    public void OnReturn(Vector3 moveLocation, bool moveSuccesful)
    {
        if (moveSuccesful &&!foundFood)
        {
            StopCoroutine("UnitSearch");
            targetPos = moveLocation;
            StartCoroutine("UnitSearch");
        }
    }
    public void OnPathFound(Vector3[] newPath, bool moveSuccesful)
    {

        if (moveSuccesful && foundFood)
        {
            StopCoroutine("FollowPath");
            path = newPath;
            targetIndex = 0;
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdateMove()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        MoveRequestManager.RequestMove(transform.position, targetPos, OnReturn);

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
                traveledNodes.Add(transform.position);
                MoveRequestManager.RequestMove(transform.position, targetPos, OnReturn);
        }
    }

    IEnumerator UnitSearch()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
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
        Debug.Log(collision.collider.name);

        if (collision.collider.name == "Honey" && !foundFood)
        {
            StopCoroutine("UnitSearch");
            foundFood = true;
            PathRequestManager.RequestPath(traveledNodes.ToArray(), OnPathFound);

        }
    }

    //public void OnDrawGizmos()
    //{
    //    if(targetPos != null)
    //    {
    //        Gizmos.color = Color.black;
    //        Gizmos.DrawCube(targetPos, Vector3.one);
    //    }
    //}
}
