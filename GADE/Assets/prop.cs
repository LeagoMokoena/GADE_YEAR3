using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class prop : MonoBehaviour
{
    public int width;
    public int height;
    public map floor;

    public List<Node> curr = null;
    public void setFloor(int w, int h)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(curr != null)
        {
            int cun = 0;

            while(cun < curr.Count - 1)
            {
                Vector3 st = floor.coord(curr[cun].w, curr[cun].h) + new Vector3(0,0,-1f);
                Vector3 fin = floor.coord(curr[cun+1].w, curr[cun+1].h) + new Vector3(0,0,-1f);

                Debug.DrawLine(st, fin, Color.blue);

                cun++;
            }
        }
    }
}
