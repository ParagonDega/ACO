using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Nest home;
    public Grid grid;
    private BoxCollider agentCollider;
    public GameObject workerPrefab;
    GameObject workerPrefabClone;
    List<Vector3> traveledNodes = new List<Vector3>();
    float speed = 2.0f;
    Vector3 test = Vector3.zero, targetPos, checkPosition, currentWaypoint,homeNest;
    Vector3[] path;
    const float minPathUpdateTime = .2f;
    bool returnToNest = false, moving = false, foundResource = false;
    int targetIndex, storage=0;
    public int energy;
    public int baseEnergy, storageLimit, numberOfAnts= 1, spawnCost;

    private void Start()
    {
        agentCollider = GetComponent<BoxCollider>();
        targetPos = transform.position;
        StartCoroutine(UpdateMove());
        baseEnergy = GetEnergy();
        storageLimit = baseEnergy;
        homeNest = home.GetPosition();
        Physics.IgnoreLayerCollision(13, 13);

        spawnCost = baseEnergy + (numberOfAnts * numberOfAnts);
    }

    private void Update()
    {
        if(traveledNodes.Count >= (baseEnergy/2) && !returnToNest)
        {
            ReturnToNest();
        }else if (energy <= 2 && !returnToNest)
        {
            KillUnit();
        }
    }

    IEnumerator UpdateMove()                                        //Forward pathfinding manager call
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if (!moving)
            {
                energy--;
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
                CheckFood();
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
                        home.ChangeFood(storage);
                    }
                    CheckFood();
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

    private void CheckFood()
    {
        int missing = (baseEnergy - energy) / 10;
        if (missing > 0)
        {
            FillEnergy(missing);
        }
    }

    private void FillEnergy(int missing)
    {
        if (home.FoodLow() && energy <= 0)
        {
            KillUnit();
        }
        else
        {
            int foodGained = -home.ChangeFood(-missing) * 10;
            energy += foodGained;
        }
    }

    private int GetEnergy()
    {
        return energy;
    }

    private void KillUnit()
    {
        numberOfAnts--;
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public void SpawnUnit()
    {
        if (!home.FoodLow() && spawnCost < home.food)
        {
            workerPrefabClone = Instantiate(workerPrefab, homeNest, Quaternion.identity) as GameObject;
            numberOfAnts++;
            home.ChangeFood(-spawnCost);
        }
        spawnCost = baseEnergy + (numberOfAnts * numberOfAnts);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Ant")

        {
            //Debug.Log(collision.collider.name);
            /*if (!returnToNest)
            {
                moving = false;
            }*/


        }

        if (collision.collider.name == "foodHoney(Clone)" && !returnToNest)
        {
            GameObject hitObject = collision.gameObject;


            grid.updateNodeWeight(transform.position);
            //Debug.Log(collision.collider.name);
            foundResource = true;
            int fill = ((baseEnergy - energy) % 10);
            float gathered = collision.gameObject.GetComponent<ResourceManager>().ResourceGather(-(fill + storageLimit));
            energy += (int)gathered;
            if(energy > baseEnergy)
            {
                energy = baseEnergy;
            }

            storage = ((int)gathered-fill);

            ReturnToNest();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;
        if(hitObject.tag == "Resource")
        {
            int fill = -(((baseEnergy - energy) % 10) + storage);
            hitObject.GetComponent<ResourceManager>().ResourceGather(fill);
        }
    }

    public void OnDrawGizmos()
    {
        if (targetPos != test)
        {
            Gizmos.color = new Vector4(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.4f);
            Gizmos.DrawCube(targetPos, Vector3.one);
        }
        if (path != null)
        {
            if (foundResource)
            {
                Gizmos.color = new Vector4(Color.red.r, Color.red.g, Color.red.b, 0.4f);//Colour successful food trail
            }
            else
            {
                Gizmos.color = new Vector4(Color.green.r, Color.green.g, Color.green.b, 0.4f);//Colour failed search trail
            }
            for (int i = targetIndex; i < path.Length; i++)
            {                
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

