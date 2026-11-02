using UnityEngine;

public class Edge
{
    public Vector2Int Destination { get; }
    public float Weight { get; }

    public Edge(Vector2Int destination, float weight)
    {
        Destination = destination;
        Weight = weight;
    }
}
