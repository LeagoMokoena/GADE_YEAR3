using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click : MonoBehaviour
{
    public int width;
    public int height;
    public map tile;
    public GameObject unitOnTile;
    private void OnMouseUp()
    {
        tile.Move(width, height);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
