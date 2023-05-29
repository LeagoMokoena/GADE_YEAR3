using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hover : MonoBehaviour
{
    Color hoverColor = Color.red;
    Color hoverAlpha;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        hoverAlpha = renderer.material.color;
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
