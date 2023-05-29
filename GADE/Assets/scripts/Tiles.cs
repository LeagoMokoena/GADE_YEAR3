using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Tiles
{
    public int width;
    public int hieght;
    public map map;

    public List<Node> currentPath = null;

    void Update()
    {
        if (currentPath != null)
        {
            int currentNode = 0;

            while (currentNode < currentPath.Count - 1)
            {
                Vector3 beg = map.TileCoordToWorldCoord(currentPath[currentNode].w, currentPath[currentNode].h);
                Vector3 fin = map.TileCoordToWorldCoord(currentPath[currentNode + 1].w, currentPath[currentNode + 1].h); ;

                beg += Vector3.forward * -1f;
                fin += Vector3.forward * -1f;

                Debug.DrawLine(beg, fin, Color.red);
                currentNode++;
            }
        }
    }

}
