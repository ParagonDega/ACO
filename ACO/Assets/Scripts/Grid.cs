using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Grid : MonoBehaviour
{
    const float minGridUpdateTime = 120.0f;
    public Camera gamecamera;
    public Transform player;
    public LayerMask unwalkableMask, foodMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    public Nest nest;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    float randomLimit= 1.0f;
    //private float offset = 1.2f;
    string path, content;
    bool textStart = false;
    int run = 0;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        StartCoroutine(UpdateGrid());

        path = Application.dataPath + "/log.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }

    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridSizeY / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));                                                                                     //Mostly be used for simulated formicarium
                bool food = !(Physics.CheckSphere(worldPoint, nodeRadius, foodMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NextNode(Node n, Node l, int size)
    {
        Node nextNode;
        List<Node> neighbours = GetNeighbours(n, l);

        //if (!textStart)
        //{
        //    content = "Settings: pheremoneInit: " + n.GetInitialPheremone() + ", foodInit:" + n.GetFoodPheremone() + ", alpha:" + n.alpha + ", randomLimit:" + randomLimit + "\n";
        //    File.AppendAllText(path, content);
        //    textStart = true;
        //}

        nextNode = CalculateProbabilities(neighbours);//Change for graph weights

        return nextNode;
    }

    private Node CalculateProbabilities(List<Node> l)
    {
        //content = "Run: " + run + "= ";
        Node next = l[0];
        float pheremone = 0.0f;
        float check = 0, random = RandomNumber(randomLimit);

        for (int i = 0; i < l.Count; i++)                                   //calculate neighbour probabilities
        {
            pheremone += l[i].alpha * (1 / l[i].pheremone);                 //Sets total pheremone
        }

        //content += random + ", " + pheremone + ", ";

        for (int i = 0; i < l.Count; i++)                                   //calculate neighbour probabilities
        {
            l[i].probability = (l[i].alpha * (1 / l[i].pheremone)) / pheremone;
            //content += i + ": " + l[i].probability + ", ";
        }
        for (int i = 0; i < l.Count; i++)
        {
            check += l[i].probability;
            if (check >= random)
            {
                //content += "Next: " + i + "\n";
                //File.AppendAllText(path, content);
                run++;
                next = l[i];
                break;
            }
        }
        return next;
    }

    public List<Node> GetNeighbours(Node node, Node last)//For forward pathfinding
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (grid[checkX, checkY].walkable && (checkX != last.worldPos.x && checkY != last.worldPos.z))  //If position is walkable and not the last node
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }

        return neighbours;
    }
    public List<Node> GetNeighbours(Node node)//For pheremone update
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (grid[checkX, checkY].walkable)  //If position is walkable and not the last node
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }

        return neighbours;
    }

    public void updateNodeWeight(Vector3 position)
    {
        NodeFromWorldPoint(position).changeWeight(-1.0f);
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];

    }

    private void OnDrawGizmos()
    {
        float color;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0, gridWorldSize.y));
        if (grid != null)
        {
            //Node playerNode = NodeFromWorldPoint(player.position);

            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.clear : Color.red;                                                                          //Display unwalkable
                if (n.pheremone < n.GetInitialPheremone() && n.pheremone >= 1.0f)
                {
                    color = n.pheremone / n.GetInitialPheremone();
                    Gizmos.color = new Vector4(color, color, color, 1.0f);
                }else if(n.pheremone == 0.0f){
                    Gizmos.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 0.2f);
                }
                else if(n.pheremone == 0.9f){
                    Gizmos.color = new Vector4(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.2f);
                }
                //if (playerNode == n)
                //{
                //    Gizmos.color = Color.cyan;                                                                                                  //Playernode
                //}
                Gizmos.DrawCube(n.worldPos, new Vector3(1.0f, 0.2f, 1.0f) * (nodeDiameter - .1f));
            }
        }
    }

    private float RandomNumber(float num)
    {
        float value = (UnityEngine.Random.Range(0.0f, (float)num));
        return value;
    }

    void OnMouseDown()
    {
        Ray ray = gamecamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Node spawnNode = null;
        Vector3 spawnPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hitInfo))
        {
            spawnNode = NodeFromWorldPoint(hitInfo.point);//get spawn location
            spawnPosition = spawnNode.worldPos;
            Debug.Log("Hit " + spawnNode.gridX + " " + spawnNode.gridY);

        }
        if (spawnNode.food != null)//Check if food already spawned
        {
            Debug.Log("Already food here - TODO: Display on screen");
            return;
        }
        else//Spawn new food item
        {
            if (spawnPosition != Vector3.zero)
            {
                GameObject foodToSpawn = SpawnManager.spawnManager.GetFoodToSpawn();
                float dist = Vector3.Distance(spawnPosition, nest.GetPosition());
                if (dist > 4.0f) {
                    spawnNode.food = (GameObject)Instantiate(foodToSpawn, spawnPosition, Quaternion.identity);
                }
                    //List<Node> neighbours = GetNeighbours(spawnNode);
                //float foodPhere=spawnNode.GetFoodPheremone();
                //for (int i = 0; i < neighbours.Count; i++)
                //{
                //    neighbours[i].pheremone = foodPhere;                                                             //Set all neighbour pheremones high to simulate food awareness
                //}

            }
        }

    }

    public void RemoveFood(Vector3 position)
    {
        Node spawnNode = NodeFromWorldPoint(position);
        List<Node> neighbours = GetNeighbours(spawnNode);
        float initPhere = spawnNode.GetInitialPheremone();
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].pheremone = initPhere;                                                             //Set all neighbour pheremones high to simulate food awareness
        }
    }

    IEnumerator UpdateGrid()
    {
        if (Time.timeSinceLevelLoad < 120.0f)
        {
            yield return new WaitForSeconds(30.0f);
        }
        while (true)
        {
            yield return new WaitForSeconds(minGridUpdateTime);
            pheremoneDispersal();
        }

    }

    void pheremoneDispersal()
    {
        Debug.Log("Updated pheremone");
        foreach(Node n in grid)
        {
            if(n.pheremone != n.GetInitialPheremone() && n.pheremone != n.GetFoodPheremone()){
                n.changeWeight(0.25f);
            }
        }
    }
}