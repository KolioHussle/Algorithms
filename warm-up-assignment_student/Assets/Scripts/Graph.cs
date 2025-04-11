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


    public void AddEdge(T node1,T node2)
    {
        if (!adjacencyList.ContainsKey(node1) || !adjacencyList.ContainsKey(node2))
        {
            Debug.Log("One or both nodes do not exist in the graph.");
            return;
        }

        if (!adjacencyList[node1].Contains(node2))
        {
            adjacencyList[node1].Add(node2);
        }

        if (!adjacencyList[node1].Contains(node2))
        {
            adjacencyList[node1].Add(node2);
        }
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

    public HashSet<T> BFS(T startNode)
    {
        //List<T> queue = new List<T>();
        Queue<T> qeue2 = new Queue<T>();
        HashSet<T> discovered = new HashSet<T>();


        T v = startNode;
        //queue.Add(v);

        qeue2.Enqueue(v);

        discovered.Add(v);
        //Debug.Log(v + " was discovered");

        while (/*queue.Count*/ qeue2.Count > 0)
        {
            //v = queue[0];
            //queue.RemoveAt(0);
            v = qeue2.Dequeue();
            Debug.Log(v);
            foreach (var w in GetNeighbors(v))
            {
                if (!discovered.Contains(w))
                {
                    //queue.Add(w);
                    qeue2.Enqueue(w);
                    discovered.Add(w);
                }
            }
        }
        return discovered;
    }

    public void DFS(T startNode)
    {
        Stack<T> S = new Stack<T>();
        List<T> discovered = new List<T>();

        T v = startNode;
        S.Push(v);

        while (S.Count > 0)
        {
            v = S.Pop();

            if (!discovered.Contains(v))
            {
                discovered.Add(v);
                Debug.Log(v);

                foreach (var w in GetNeighbors(v))
                {
                    S.Push(w);
                }
            }
        }
    }
}
