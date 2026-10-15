using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Unity.Properties;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private static int V = 60;
    private int E = 0;
    private ConcurrentBag<int>[] adj;
    public JsonLoader jsonLoader = new JsonLoader();
    //public Graph(int V) // no edges
    //{
    //    adj = new ConcurrentBag<int>[V];
    //    for (int v = 0; v < V; v++)
    //    {
    //        adj[v] = new ConcurrentBag<int>();
    //    }
    //}

    public Graph (JsonLoader jsonLoader) // read from file
    {
        //string line = jsonLoader.GetMapData.ReadLine();
        //string[] parts = line.Split(' ');
        //V = int.Parse(parts[0]);
        //E = int.Parse(parts[1]);
        //adj = new ConcurrentBag<int>[V];
        //for (int v = 0; v < V; v++)
        //{
        //    adj[v] = new ConcurrentBag<int>();
        //}
        //for (int i = 0; i < E; i++)
        //{
        //    line = reader.ReadLine();
        //    parts = line.Split(' ');
        //    int v = int.Parse(parts[0]);
        //    int w = int.Parse(parts[1]);
        //    AddEdge(v, w);
        //}

        //this.jsonLoader = jsonLoader;

        this.jsonLoader = jsonLoader;
        jsonLoader.LoadJsonFile(jsonLoader.fileName);
    }

    void AddEdge(int v, int w)
    {
        adj[v].Add(w);
        adj[w].Add(v);
        E++;
    }

    void Vertices ()
    {
    }

    List<int> Adjacent(int v)
    {
        return new List<int>();
    }

    string ToString()
    {
        return "";
    }
}
