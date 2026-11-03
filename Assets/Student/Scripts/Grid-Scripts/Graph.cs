using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Unity.Properties;
using UnityEngine;

public class Graph
{
    private Dictionary<Vector2Int, List<Edge>> adjacencyList;
    private int width;
    private int height;

    public Graph(IMapData mapData)
    {
        if (mapData == null)
            throw new ArgumentNullException(nameof(mapData));

        width = mapData.Width;
        height = mapData.Height;

        adjacencyList = new Dictionary<Vector2Int, List<Edge>>();

        BuildGraph(mapData);
    }

    private void BuildGraph(IMapData mapData)
    {
        // Create all nodes
        // Time complexity: O(N^2) where N is the width and height of the map
        for (int x = 0; x < mapData.Width; x++) // Loop through the width of the map. Time complexity: O(N) where N is the width of the map
        {
            for (int y = 0; y < mapData.Height; y++) // Loop through the height of the map. Time complexity: O(N) where N is the height of the map
            {
                Vector2Int position = new Vector2Int(x, y); // Create a new Vector2Int for the current position

                adjacencyList[position] = new List<Edge>();  // Initialize the adjacency list for the current position
            }
        }

        // Connect the nodes
        // Time complexity: O(N^2) where N is the width and height of the map
        for (int x = 0; x < mapData.Width; x++) // time complexity: O(N) where N is the width of the map
        {
            for (int y = 0; y < mapData.Height; y++) // time complexity: O(N) where N is the height of the map
            {
                Vector2Int current = new Vector2Int(x, y);

                ConnectNeighbours(current, mapData);
            }
        }

        // Add jump connections
        AddJumps(mapData);
    }


    private void ConnectNeighbours(Vector2Int current, IMapData mapData)
    {
        // Define the directions
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int direction in directions) 
        {
            Vector2Int neighbour = current + direction; // Calculate the neighbour's position

            // Check that the neighbour is inside the map
            if (neighbour.x < 0 || neighbour.x >= mapData.Width ||
                neighbour.y < 0 || neighbour.y >= mapData.Height)
            {
                continue; // Skip this neighbour if it's outside the map
            }

            // Check whether movement is blocked by a wall
            if (PathfindingAlgorithm.IsMovementBlocked(current, neighbour, mapData))
            {
                continue; // Skip this neighbour if movement is blocked
            }

            // Determine the movement cost
            float weight = GetMovementCost(current, neighbour, mapData);

            adjacencyList[current].Add(new Edge(neighbour, weight));
        }
    }
    private float GetMovementCost(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        float wallCost;

        if (from.x != to.x)
        {
            // Moving horizontally: check vertical wall
            int wallX = Mathf.Max(from.x, to.x);

            wallCost = mapData.GetVerticalWallCost(wallX, from.y);  
        }
        else
        {
            // Moving vertically: check horizontal wall
            int wallY = Mathf.Max(from.y, to.y);

            wallCost = mapData.GetHorizontalWallCost(from.x, wallY);
        }

        if (float.IsPositiveInfinity(wallCost) || // If the wall cost is infinite, it means movement is blocked
            wallCost >= float.MaxValue) // If the wall cost is greater than or equal to the maximum float value, it means movement is blocked
        {
            return float.PositiveInfinity; // Movement is blocked by a wall
        }
        return wallCost; // Return the wall cost as the movement cost
    }

    private void AddJumps(IMapData mapData) // Time complexity: O(N^2 * M) where N is the width and height of the map, and M is the average number of jump connections per node
    {

        for (int x = 0; x < mapData.Width; x++) // Time complexity: O(N) where N is the width of the map
        {
            for (int y = 0; y < mapData.Height; y++) // Time complexity: O(N) where N is the height of the map
            {
                Vector2Int current = new Vector2Int(x, y);

                foreach (JumpConnection jump in mapData.GetJumpsFrom(current)) // Iterate through all jump connections from the current position. Time complexity: O(M) where M is the number of jump connections from the current position
                {
                    adjacencyList[current].Add(new Edge(jump.To, jump.Cost)); // Add the jump connection to the adjacency list
                }
            }
        }
    }

    // Dijkstra's algorithm to find the shortest path from start to end
    
     public IEnumerable<Vector2Int> Vertices()
     {
        return adjacencyList.Keys; // Return all the vertices in the graph
     }

    public IEnumerable<Edge> Adjacent(Vector2Int position)
    {
        if (!adjacencyList.ContainsKey(position))
            return new List<Edge>(); // Return an empty list if the position is not in the graph

        return adjacencyList[position]; // Return the list of edges for the given position
    }

    public float Weight(Vector2Int from, Vector2Int to)
    {
        if (!adjacencyList.ContainsKey(from))
            return float.PositiveInfinity;

        float lowestWeight = float.PositiveInfinity;

        foreach (Edge edge in adjacencyList[from])
        {
            if (edge.Destination == to && edge.Weight < lowestWeight)
            {
                lowestWeight = edge.Weight;
            }
        }

        return lowestWeight; // Return the lowest weight found, or PositiveInfinity if no edge exists
    }

    public bool ContainsVertex(Vector2Int position)
    {
        return adjacencyList.ContainsKey(position); // Return true if the position is in the graph, false otherwise
    }
}
