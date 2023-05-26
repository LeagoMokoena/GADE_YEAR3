using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> children;
    public int w;
    public int h;
    public Node()
    {
        children = new List<Node>();
    }

    public float distance(Node no)
    {
        return Vector2.Distance(new Vector2(w, h), new Vector2(no.w, no.h));
    }
}
