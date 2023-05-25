using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class map : MonoBehaviour
{
    public GameObject selected;
    public Tiles[] types;

    int[,] tiles;

    int width = 10;
    int height = 10;
    // Start is called before the first frame update
    void Start()
    {
        tiles = new int[width, height];

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tiles[i, j] = 0;
            }
        }

        tiles[4, 4] = 1;
        tiles[5, 4] = 1;
        tiles[6,4] = 1;
        tiles[7,4] = 1;
        tiles[8,4] = 1;
        build();
    }

    class Node
    {
        public List<Node> children;

        public Node()
        {
            children = new List<Node>();
        }
    }

    Node[,] nodes;
    void pathGraph()
    {
        nodes = new Node[width, height];

        for( int i = 0; i < width; ++i)
        {
            for( int j = 0; j < height; ++j)
            {
                if(i > 0)
                    nodes[i, j].children.Add(nodes[i - 1, j]); 
                if(i < width - 1)
                    nodes[i, j].children.Add(nodes[i + 1, j]); 
                if(j > 0)
                    nodes[i, j].children.Add(nodes[i, j-1]); 
                if(j < height - 1)
                    nodes[i, j].children.Add(nodes[i, j + 1]);


            }
        }
    }

    void build()
    {
        for(int i = 0; i < width; ++i)
        {
            for( int j = 0; j < height; ++j)
            {
                Tiles t = types[tiles[i, j]];

                GameObject ob = Instantiate(t.ob, new Vector3(i,0,j), Quaternion.identity);

                click cl = ob.GetComponent<click>();
                cl.width = i; cl.height = j;
                cl.tile = this;
            }
        }
    }

    public void Move(int wid, int hei)
    {

        Dictionary<Node, float> dis = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> notReached = new List<Node>();
        Node sour = nodes[selected.GetComponent<prop>().width,
            selected.GetComponent<prop>().height];

        dis[sour] = 0;
        prev[sour] = null;

        foreach( Node node in nodes )
        {
            if( node != sour) 
            {
                dis[node] = Mathf.Infinity; prev[node] = null;
                
            }

            notReached.Add( node );
        }

        while (notReached.Count > 0) 
        {
            Node q = notReached.OrderBy(n  => dis[n]).First();
            notReached.Remove( q );
        }
        
    }
    public Vector3 tilecoord(int wid, int hei)
    {
        return new Vector3(wid,0,hei);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
