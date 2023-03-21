using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
}

public class PathNode {
    private Vector3 position = Vector3.zero;

    public float gCost;
    public float hCost;
    public float fCost;

    public PathNode previousNode;

    PathNode(Vector3 position)
    {
        this.position = position;
    }
}

