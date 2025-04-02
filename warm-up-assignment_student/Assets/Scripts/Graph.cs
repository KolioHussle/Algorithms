using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph<T>
{
    private Dictionary<T, List<T>> adjacencyList;

    public Graph()
    {
        adjacencyList = new Dictionary<T, List<T>>();
    }

    public void AddNode(T node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<T>();
        }
    }


    public void AddEdge(T fromNode, T toNode)
    {
        if (!adjacencyList.ContainsKey(fromNode) || !adjacencyList.ContainsKey(toNode))
        {
            Debug.Log("One or both nodes do not exist in the graph.");
            return;
        }
        adjacencyList[fromNode].Add(toNode);
        adjacencyList[toNode].Add(fromNode);
    }

    public List<T> GetNeighbors(T node)
    {
        if (adjacencyList.ContainsKey(node))
        {
            return adjacencyList[node];
        }
        return new List<T>(); // Return an empty list instead of null
    }

    public List<T> GetNodes()
    {
        return adjacencyList.Keys.ToList();
    }

    public void PrintGraph()
    {
        /*foreach (var node in adjacencyList)
        {
            Debug.Log($"{node.Key}: {string.Join(", ", node.Value)}");
        }*/
        foreach (var node in adjacencyList)
        {
            if (node.Key is RectInt room)
            {
                // Visualizing the current node and its neighbors
                Debug.Log($"Room at ({room.xMin}, {room.yMin}) with size ({room.width}, {room.height}) has neighbors:");

                if (node.Value.Count > 0)
                {
                    // Displaying the list of neighbors' positions using xMin and yMin
                    foreach (var neighbor in node.Value)
                    {
                        if (neighbor is RectInt neighborRoom)
                        {
                            Debug.Log($" - ({neighborRoom.xMin}, {neighborRoom.yMin}) with size ({neighborRoom.width}, {neighborRoom.height})");
                        }
                    }
                }
                else
                {
                    Debug.Log(" No neighbors.");
                }
            }
        }
    }
}
