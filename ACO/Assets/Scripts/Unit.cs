using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Nest home;
    public Grid grid;
    List<Vector3> traveledNodes = new List<Vector3>();
    float speed = 10.0f;
    Vector3 test = Vector3.zero, targetPos, checkPosition, currentWaypoint, homeNest;
    Vector3[] path;
    const float minPathUpdateTime = .2f;
    bool returnToNest = false, moving = false, foundResource = false;
    int targetIndex, storage=0;
    public int energy;
    int baseEnergy;

    private void Start()
    {
        targetPos = transform.position;
        StartCoroutine(UpdateMove());
        baseEnergy = GetEnergy();
    }

    private void Update()
    {
        if(traveledNodes.Count >= energy && !returnToNest)
        {
            ReturnToNest();
        }
    }

    IEnumerator UpdateMove()                                        //Forward pathfinding manager call
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        if (!moving)
        {
            traveledNodes.Add(transform.position);
            ForwardMovementManager.RequestMove(transform.position, targetPos, OnReturn);
        }
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if (!moving)
            {
                traveledNodes.Add(transform.position);
                ForwardMovementManager.RequestMove(transform.position, targetPos, OnReturn);
            }
        }
    }

    public void OnReturn(Vector3 moveLocation, bool moveSuccesful)  //Forward pathfinding wrapper
    {
        if (moveSuccesful && !returnToNest && !moving)
        {
            StopCoroutine("UnitSearch");
            targetPos = moveLocation;
            moving = true;
            StartCoroutine("UnitSearch");
        }
    }

    IEnumerator UnitSearch()                                        //Forward pathfinding coroutine
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            if (dist<=0.4f)
            {
                moving = false;
            }
            float nestDist = Vector3.Distance(transform.position, homeNest);
            if (nestDist <= 1.0f)
            {
                traveledNodes.Clear();
                energy = baseEnergy;
                traveledNodes.Add(homeNest);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
  
    }

    private void ReturnToNest()                                     //Backwards pathfinding manager call
    {
        StopCoroutine("UnitSearch");
        moving = false;
        targetIndex = 0;
        targetPos = test;
        returnToNest = true;
        BackwardsPathfindingManager.RequestPath(traveledNodes.ToArray(), OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool moveSuccesful)  //Backwards pathfinding wrapper
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

    IEnumerator FollowPath()                                        //Bacwards pathfinding coroutine
    {
        while (true)
        {
            float dist = Vector3.Distance(transform.position, currentWaypoint);

            if (dist <= 0.4f)
            {
                //Debug.Log("Target Reached");

                if (foundResource)
                {
                    grid.updateNodeWeight(currentWaypoint);
                }

                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    traveledNodes.Clear();
                    if (foundResource)
                    {
                        foundResource = false;
                    }
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

    private int GetEnergy()
    {
        return energy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Ant")
        {
            //Debug.Log(collision.collider.name);
            if (!returnToNest)
            {
                moving = false;
            }

        }

        if (collision.collider.name == "Honey" && !returnToNest)
        {
            grid.updateNodeWeight(transform.position);
            Debug.Log(collision.collider.name);
            foundResource = true;
            storage++;
            ReturnToNest();
        }
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

