using Assets.Student.Scripts.Grid_Scripts;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PathfindingAlgorithm
{
    // TODO: Implement the pathfinding assignment here.
    // Find the lowest-cost path from start to goal using the map dimensions, wall costs and directed jumps exposed by IMapData.


    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        if (mapData == null)
        {
            Debug.LogError("Map data is null.");
            return null;
        }

        // Build the graph from the map data.
        Graph graph = new Graph(mapData);

        // Check that start and goal exist in the graph.
        if (!graph.ContainsVertex(start) || !graph.ContainsVertex(goal))
        {
            Debug.LogError("Start or goal is outside the map.");
            return null;
        }

        // If start and goal are the same, no movement is needed.
        if (start == goal)
            return new List<Vector2Int> { start };

        // Cheapest known distance from start to each node.
        Dictionary<Vector2Int, float> distances = new Dictionary<Vector2Int, float>(); // space complexity: O(V), where V is the number of vertices in the graph

        // Previous node in the cheapest known path.
        Dictionary<Vector2Int, Vector2Int> previous = new Dictionary<Vector2Int, Vector2Int>(); // space complexity: O(V), where V is the number of vertices in the graph

        // Priority queue: lowest distance gets processed first.
        MinPriorityQueue priorityQueue = new MinPriorityQueue(); // space complexity: O(V), where V is the number of vertices in the graph

        // Initialize every node with infinite distance.
        foreach (Vector2Int vertex in graph.Vertices()) 
        {
            distances[vertex] = float.PositiveInfinity;
        }

        // Starting node has distance zero.
        distances[start] = 0f;

        priorityQueue.Enqueue(start, 0f); // Enqueue the starting position with priority 0.

        // Dijkstra's main loop.
        while (priorityQueue.Count > 0) // time complexity: O((V + E) log V), where V is the number of vertices and E is the number of edges
        {
            // The time complexity is O((V + E) log V) because each vertex is processed once, and each edge is examined once. The priority queue operations (enqueue and dequeue) take O(log V) time.

            // Get the node with the lowest priority (distance).
            QueueItem currentItem = priorityQueue.Dequeue();

            Vector2Int current = currentItem.Position; 
            float currentDistance = currentItem.Priority; 

            // Ignore outdated queue entries.
            if (currentDistance > distances[current])
                continue;

            // The goal has been reached with its cheapest distance.
            if (current == goal)
                break;

            // Examine every outgoing connection.
            foreach (Edge edge in graph.Adjacent(current))
            {
                Vector2Int neighbour = edge.Destination;
                float movementCost = edge.Weight;

                // Dijkstra requires non-negative edge weights.
                if (movementCost < 0f || float.IsNaN(movementCost) || float.IsInfinity(movementCost)) // Ignore invalid weights.
                {
                    continue;
                }

                // Calculate the total cost to reach this neighbour.
                float newDistance = currentDistance + movementCost;

                // Check whether this route is cheaper.
                if (newDistance < distances[neighbour])
                {
                    distances[neighbour] = newDistance;
                    previous[neighbour] = current;

                    // Add the neighbour with its updated priority.
                    priorityQueue.Enqueue(neighbour, newDistance);
                }
            }
        }

        // If the goal still has infinite distance, no path exists.
        if (float.IsPositiveInfinity(distances[goal]))
        {
            Debug.LogWarning("No path found.");
            return null;
        }

        // Reconstruct the path from goal back to start.
        return ReconstructPath(start, goal, previous);
    }

    public static bool IsMovementBlocked(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        if (mapData.TryGetJumpCost(from, to, out _))
            return false;

        int deltaX = to.x - from.x;
        int deltaY = to.y - from.y;

        if (Mathf.Abs(deltaX) + Mathf.Abs(deltaY) != 1)
            return true;

        if (deltaX != 0)
        {
            int wallX = deltaX > 0 ? to.x : from.x;
            float wallCost = mapData.GetVerticalWallCost(wallX, from.y);
            return float.IsPositiveInfinity(wallCost) || wallCost >= float.MaxValue;
        }

        int wallY = deltaY > 0 ? to.y : from.y;
        float horizontalWallCost = mapData.GetHorizontalWallCost(from.x, wallY);
        return float.IsPositiveInfinity(horizontalWallCost) || horizontalWallCost >= float.MaxValue;
    }

    private static List<Vector2Int> ReconstructPath(Vector2Int start, Vector2Int goal, Dictionary<Vector2Int, Vector2Int> previous)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        Vector2Int current = goal;

        // Work backwards from goal to start.
        while (current != start)
        {
            path.Add(current);

            // If there is no previous node, reconstruction failed.
            if (!previous.ContainsKey(current))
                return null;

            current = previous[current];
        }

        // Include the starting position.
        path.Add(start);

        // Reverse so the path goes from start to goal.
        path.Reverse();

        return path;
    }
}
