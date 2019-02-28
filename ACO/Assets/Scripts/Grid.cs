using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask,foodMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridSizeY / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));                                                                                     //Mostly be used for simulated formicarium
                bool food = !(Physics.CheckSphere(worldPoint, nodeRadius, foodMask));
                grid[x, y] = new Node(walkable,worldPoint,x,y);
            }
        }
    }

    public Node NextNode(Node n, Node l, int size)
    {
        Node nextNode = l;
        List<Node> neighbours = GetNeighbours(n,l);
        if (neighbours.Count > 1)
        {
            //while (nextNode == l)                                         //Ensures next node is not the last node traveled from
            //{
                //nextNode = neighbours[RandomNumber(neighbours.Count)];
                nextNode = CalculateProbabilities(neighbours);//Change for graph weights
            //}
        }
        else
        {
            nextNode = l;                                                   //If node is a dead end go back
        }
        return nextNode;
    }

    private Node CalculateProbabilities(List<Node> l)
    {
        Node next = l[0];
        float pheremone = 0.0f;
        float check = 0, random = RandomNumber(1.0f);

        for (int i = 0; i < l.Count; i++)                                   //calculate neighbour probabilities
        {
            pheremone += l[i].alpha * (1 / l[i].pheremone);                 //Sets total pheremone
        }

        for (int i = 0; i < l.Count; i++)                                   //calculate neighbour probabilities
        {
            l[i].probability = (l[i].alpha * (1 / l[i].pheremone))/pheremone;
        }
            for (int i = 0; i < l.Count; i++)
            {
                check += l[i].probability;
                if (check >= random)
                {
                    next = l[i];
                    break;
                }
            }
        return next ;
    }

    public List<Node> GetNeighbours(Node node, Node last)
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
                if(n.pheremone < n.GetInitialPheremone())
                {
                    color = n.pheremone / n.GetInitialPheremone();
                    Gizmos.color = new Vector4(color, color, color, 1.0f);
                }
                //if (playerNode == n)
                //{
                //    Gizmos.color = Color.cyan;                                                                                                  //Playernode
                //}
                Gizmos.DrawCube(n.worldPos, new Vector3(1.0f,0.2f,1.0f) * (nodeDiameter - .1f));
            }
        }
    }

    private float RandomNumber(float num)
    {
        float value = (UnityEngine.Random.Range(0.0f, (float)num));
        return value;
    }
}