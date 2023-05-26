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
        selected.GetComponent<prop>().width = (int)selected.transform.position.x;
        selected.GetComponent<prop>().height = (int)selected.transform.position.z;
        selected.GetComponent<prop>().floor = this;
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

    float entering(int widths,int tarwid,int tarhei, int heights)
    {
        Tiles tut = types[tiles[tarwid, tarhei]];

        float cos = tut.move;

        if(widths!=tarwid && heights != tarhei)
        {
            cos += 0.001f;
        }

        return cos;
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
                    nodes[i, j].children.Add(nodes[i-1, j-1]); 
                if(j < height - 1)
                    nodes[i, j].children.Add(nodes[i+1, j + 1]);


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

    public Vector3 coord(int wid,int hei)
    {
        return new Vector3(wid, 0, hei);
    }
    public void Move(int wid, int hei)
    {

        Dictionary<Node, float> dis = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        
        List<Node> notReached = new List<Node>();
        Node sour = nodes[selected.GetComponent<prop>().width,selected.GetComponent<prop>().height];

        Node tar = nodes[wid, hei];


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
            Node iu = null;

            foreach ( Node node in notReached )
            {
                if(iu == null || dis[node] < dis[iu])
                {
                    iu = node;
                }
            }

            if(iu == tar)
            {
                break;
            }

            notReached.Remove( iu );

            foreach( Node de in iu.children)
            {
                float al = dis[iu] + entering(de.w,iu.w,iu.h,de.h);
                if(al < dis[de])
                {
                    dis[de] = al;
                    prev[de] = iu;
                }
            }
        }

        if (prev[tar] == null)
        {
            return;
        }
        
        List<Node> cuur = new List<Node>();

        Node one = tar;
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
