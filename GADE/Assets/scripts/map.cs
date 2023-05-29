using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class map : MonoBehaviour
{
    public GameObject selectedUnit;

    public types[] tileTypes;

    int[,] tiles;
    Node[,] nodes;

    int width = 10;
    int height = 10;

    void Start()
    {

        selectedUnit.GetComponent<Tiles>().width = (int)selectedUnit.transform.position.x;
        selectedUnit.GetComponent<Tiles>().hieght = (int)selectedUnit.transform.position.z;
        selectedUnit.GetComponent<Tiles>().map = this;

        GenerateMapData();
        GeneratePathfindingGraph();
        GenerateMapVisual();
    }

    void GenerateMapData()
    {
        // Allocate our map tiles
        tiles = new int[width, height];

        // Initialize our map tiles to be grass
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = 0;
            }
        }

 

        tiles[4, 5] = 2;
        tiles[4, 6] = 2;
        tiles[8, 5] = 2;
        tiles[8, 6] = 2;

    }

    float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {
        types tt = tileTypes[tiles[targetX, targetY]];
        float cost = tt.movementCost;

        if (sourceX != targetX && sourceY != targetY)
        {
            // we are moving diagonaly Fudge the cost for tie breaking
            cost += 0.001f;
        }

        return cost;
    }

    void GeneratePathfindingGraph()
    {
        //initilize the graph
        nodes = new Node[width, height];

        //Initilize the node for each sopt in the array/
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                nodes[i, j] = new Node();
                nodes[i, j].w = i;
                nodes[i, j].h = j;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (x > 0)
                {
                    nodes[x, y].children.Add(nodes[x - 1, y]);
                    if (y > 0)
                        nodes[x, y].children.Add(nodes[x - 1, y - 1]);
                    if (y < height - 1)
                        nodes[x, y].children.Add(nodes[x - 1, y + 1]);
                }
                if (x < width - 1)
                {
                    nodes[x, y].children.Add(nodes[x + 1, y]);
                    if (y > 0)
                        nodes[x, y].children.Add(nodes[x + 1, y - 1]);
                    if (y < height - 1)
                        nodes[x, y].children.Add(nodes[x + 1, y + 1]);
                }
                if (y > 0)
                    nodes[x, y].children.Add(nodes[x, y - 1]);
                if (y < height - 1)
                    nodes[x, y].children.Add(nodes[x, y + 1]);
            }
        }
    }

    void GenerateMapVisual()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                types tt = tileTypes[tiles[x, y]];
                GameObject go = (GameObject)Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity);

                click ct = go.GetComponent<click>();
                ct.width = x;
                ct.height = y;
                ct.tile = this;
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    public void GeneratePathTo(int x, int y)
    {
        selectedUnit.GetComponent<Tiles>().currentPath = null;

        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> voius = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node sou = nodes[ selectedUnit.GetComponent<Tiles>().width,selectedUnit.GetComponent<Tiles>().hieght];

        Node tar = nodes[x,
                            y
                            ];

        distance[sou] = 0;
        voius[sou] = null;

        foreach (Node v in nodes)
        {
            if (v != sou)
            {
                distance[v] = Mathf.Infinity;
                voius[v] = null;
            }

            unvisited.Add(v);
        }

        while (unvisited.Count > 0)
        {
            // "u" is going to be the unvisited node with the smallest distance.
            Node u = null;

            foreach (Node possibleU in unvisited)
            {
                if (u == null || distance[possibleU] < distance[u])
                {
                    u = possibleU;
                }
            }

            if (u == tar)
            {
                break;  // Exit the while loop!
            }

            unvisited.Remove(u);

            foreach (Node v in u.children)
            {

                float alt = distance[u] + CostToEnterTile(u.w, u.h, v.w, v.h);
                if (alt < distance[v])
                {
                    distance[v] = alt;
                    voius[v] = u;
                }
            }
        }


        if (voius[tar] == null)
        {
            return;
        }

        List<Node> currentPath = new List<Node>();

        Node curr = tar;

        // Step through the "prev" chain and add it to our path
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = voius[curr];
        }

        // Right now, currentPath describes a route from out target to our source
        // So we need to invert it!

        currentPath.Reverse();

        selectedUnit.GetComponent<Tiles>().currentPath = currentPath;
    }

}
