using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width;
    public int height;
    private RectInt startRoom;
    private List<RectInt> newRooms = new List<RectInt>();
    private List<RectInt> newDoors = new List<RectInt>();
    public int roomsToCreate;
    public float timeToGenerate;
    private bool hasGenerated = false;

    private Graph<RectInt> dungeonGraph;
    HashSet<(Vector2Int, Vector2Int)> usedRoomPairs = new();

    void Start()
    {
        dungeonGraph = new Graph<RectInt>();

        startRoom = new RectInt(0, 0, width, height);

        //SplittingRooms(startRoom, roomsToCreate);
        SplittingRooms1(startRoom, roomsToCreate);
        MakingDoors();
        //MakingDoors1();

        CreateNodes();
        CreatdEdges();

        //Debug.Log($"Total Graph Nodes: {dungeonGraph.GetNodes().Count}");
        //dungeonGraph.PrintGraph();
        //TestMinimalGraph();
    }

    void Update()
    {
        AlgorithmsUtils.DebugRectInt(startRoom, Color.yellow);

        StartCoroutine(GenerateDungeon());
        if (hasGenerated)
        {
            Debug.Log($"Final Room Count: {newRooms.Count}");
            StartCoroutine(ShowingDoors());
        }
    }

    void TestMinimalGraph()
    {
        // Create two intersecting rooms manually
        RectInt room1 = new RectInt(0, 0, 10, 10);
        RectInt room2 = new RectInt(5, 5, 10, 10);

        dungeonGraph.AddNode(room1);
        dungeonGraph.AddNode(room2);

        // Add edge between them
        dungeonGraph.AddEdge(room1, room2);

        // Print the graph
        dungeonGraph.PrintGraph();
    }



    /*void SplittingRooms(RectInt room, int numberOfRooms)
    {
        if (numberOfRooms <= 0 || room.width < 10 || room.height < 10)
        {
            newRooms.Add(room);
            Debug.Log($"Final room count: {newRooms.Count}");
            return;
        }

        int randomNumber = Random.Range(0,2);
        bool directionDevider = false;

        if (randomNumber == 0)
        {
            directionDevider = true;
        }

        if (directionDevider)
        {       
        int WidthMinPos = room.xMin + 5;
        int WidthMaxPos = room.xMax - 5;

        if (WidthMinPos >= WidthMaxPos) return;

        int randomPointX = Random.Range(WidthMinPos, WidthMaxPos);


        RectInt leftRoom = new RectInt(room.x, room.y, randomPointX - room.x, room.height);
        RectInt rightRoom = new RectInt(randomPointX, room.y, room.xMax - randomPointX, room.height);

        newRooms.Add(leftRoom);
        newRooms.Add(rightRoom);

        //Debug.Log($" Splitting room at {room.position}, size {room.size} -> Left: {leftRoom.position} {leftRoom.size}, Right: {rightRoom.position} {rightRoom.size}");

        SplittingRooms(leftRoom, numberOfRooms - 1);
        SplittingRooms(rightRoom, numberOfRooms - 1);
        }

        else 
        {         
        int HeightMinPos = room.yMin + 5;
        int HeightMaxPos = room.yMax - 5;

        if (HeightMinPos >= HeightMaxPos) return;

        int randomPointY = Random.Range(HeightMinPos, HeightMaxPos);

        RectInt upRoom = new RectInt(room.x, randomPointY, room.width, room.yMax - randomPointY);
        RectInt downRoom = new RectInt(room.x, room.y, room.width,randomPointY - room.y);

        newRooms.Add(upRoom);
        newRooms.Add(downRoom);

        //Debug.Log($" Splitting room at {room.position}, size {room.size} -> Up: {upRoom.position} {upRoom.size}, Down: {downRoom.position} {downRoom.size}");

        SplittingRooms(upRoom, numberOfRooms - 1);
        SplittingRooms(downRoom, numberOfRooms - 1);
        }
    }*/
    void SplittingRooms1(RectInt startRoom, int maxRooms)
    {
        newRooms.Clear();
        Queue<RectInt> queue = new Queue<RectInt>();
        queue.Enqueue(startRoom);

        while (queue.Count > 0 && newRooms.Count + queue.Count < maxRooms)
        {
            RectInt currentRoom = queue.Dequeue();

            bool splitHorizontally = Random.value > 0.5f;
            if (currentRoom.width < 10 || currentRoom.height < 10)
            {
                newRooms.Add(currentRoom);
                continue;
            }

            if (splitHorizontally && currentRoom.height >= 10)
            {
                int splitY = Random.Range(currentRoom.yMin + 5, currentRoom.yMax - 5);
                RectInt bottom = new RectInt(currentRoom.x, currentRoom.y, currentRoom.width, splitY - currentRoom.y);
                RectInt top = new RectInt(currentRoom.x, splitY, currentRoom.width, currentRoom.yMax - splitY);

                queue.Enqueue(bottom);
                queue.Enqueue(top);
            }
            else if (!splitHorizontally && currentRoom.width >= 10)
            {
                int splitX = Random.Range(currentRoom.xMin + 5, currentRoom.xMax - 5);
                RectInt left = new RectInt(currentRoom.x, currentRoom.y, splitX - currentRoom.x, currentRoom.height);
                RectInt right = new RectInt(splitX, currentRoom.y, currentRoom.xMax - splitX, currentRoom.height);

                queue.Enqueue(left);
                queue.Enqueue(right);
            }
            else
            {
                newRooms.Add(currentRoom);
            }
        }

        // Add remaining rooms
        while (queue.Count > 0)
        {
            newRooms.Add(queue.Dequeue());
        }
    }

    void MakingDoors()
    {
        newDoors.Clear();
        Debug.Log("MakingDoors() is running!");
        //List<RectInt> usedDoorPositions = new List<RectInt>();
        List<Vector2Int> usedDoorPositions1 = new List<Vector2Int>();

        for (int i = 1; i < newRooms.Count; i++)
        {
            RectInt roomA = newRooms[i];
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                RectInt roomB = newRooms[j];

                if (AreRoomsAdjacent(roomA, roomB)/*AlgorithmsUtils.Intersects(roomA, roomB)*/)
                {
                    Debug.Log("hello");
                    Vector2Int doorPosition = GetSharedEdgeMidpoint(roomA, roomB);

                    /* RectInt doorArea = AlgorithmsUtils.Intersect(roomA, roomB);
                     if (doorArea.width > 0 && doorArea.height > 0)
                     {

                         Vector2Int doorPos;
                         Vector2Int doorSize;

                         if (doorArea.width > doorArea.height) // Horizontal door
                         {
                             doorPos = new Vector2Int(doorArea.x + doorArea.width / 2 - 1, doorArea.y);
                             doorSize = new Vector2Int(2, 1);
                         }
                         else // Vertical door
                         {
                             doorPos = new Vector2Int(doorArea.x, doorArea.y + doorArea.height / 2 - 1);
                             doorSize = new Vector2Int(1, 2);
                         }

                         // Ensure we do not already have a door at this position
                         bool doorExists = false;
                         foreach (var door in usedDoorPositions)
                         {
                             if (door.x == doorPos.x && door.y == doorPos.y && door.width == doorSize.x && door.height == doorSize.y)
                             {
                                 doorExists = true;
                                 break;
                             }
                         }

                         // Add the door if it doesn't already exist
                         if (!doorExists)
                         {
                             newDoors.Add(new RectInt(doorPos, doorSize));
                             usedDoorPositions.Add(new RectInt(doorPos, doorSize));
                             Debug.Log($"Door added between room {roomA.position} and {roomB.position} at {doorPos}");
                         }*/
                    if (doorPosition != Vector2Int.zero && !usedDoorPositions1.Contains(doorPosition))
                    {
                        newDoors.Add(new RectInt(doorPosition, Vector2Int.one)); // Size of the door is 1x1
                        usedDoorPositions1.Add(doorPosition); // Track the door position
                        Debug.Log($"Door added between room {roomA.position} and {roomB.position} at {doorPosition}");
                    }
                }
                
            }
        }
        Debug.Log($"Final door count: {newDoors.Count}");
    }

    void CreateNodes()
    {
         foreach (var room in newRooms)
         {
               if (room != null)
               {
                  dungeonGraph.AddNode(room);
               }
         }
    }

    void CreatdEdges()
    {
        for (int i = 1; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersects(newRooms[i], newRooms[j]))
                {
                    dungeonGraph.AddEdge(newRooms[i], newRooms[j]);
                    Debug.Log($"Connected Room {i} with Room {j}");
                }
            }
        }
    }

    IEnumerator ShowingDoors()
    {
        foreach(var door in newDoors)
        {
            yield return new WaitForSeconds(timeToGenerate);
            AlgorithmsUtils.DebugRectInt(door, Color.blue);
        }
    }

    IEnumerator GenerateDungeon()
    {
        foreach (var room in newRooms)
        {
            yield return new WaitForSeconds(timeToGenerate);
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }

        hasGenerated = true;
    }


    void MakingDoors1()
    {
        newDoors.Clear();
        usedRoomPairs.Clear();
        Debug.Log("MakingDoors() is running!");

        for (int i = 0; i < newRooms.Count; i++)
        {
            RectInt roomA = newRooms[i];

            for (int j = i + 1; j < newRooms.Count; j++)
            {
                RectInt roomB = newRooms[j];

                if (AreRoomsAdjacent(roomA, roomB))
                {
                    var pair = (roomA.position, roomB.position);
                    var reversePair = (roomB.position, roomA.position);

                    if (usedRoomPairs.Contains(pair) || usedRoomPairs.Contains(reversePair))
                        continue;

                    // Add door at midpoint of shared wall
                    Vector2Int doorPosition = GetSharedEdgeMidpoint(roomA, roomB);
                    if (doorPosition != Vector2Int.zero)
                    {
                        newDoors.Add(new RectInt(doorPosition, Vector2Int.one));
                        usedRoomPairs.Add(pair);
                        Debug.Log($"Door added between room {roomA.position} and {roomB.position} at {doorPosition}");
                    }

                }
            }
        }

        Debug.Log($"Total doors created: {newDoors.Count}");
    }
    bool AreRoomsAdjacent(RectInt a, RectInt b)
    {
        // Check if rooms are touching horizontally
        bool touchingVertically =
            (a.xMin < b.xMax && a.xMax > b.xMin) &&
            (a.yMax == b.yMin || a.yMin == b.yMax);

        // Check if rooms are touching vertically
        bool touchingHorizontally =
            (a.yMin < b.yMax && a.yMax > b.yMin) &&
            (a.xMax == b.xMin || a.xMin == b.xMax);

        return touchingVertically || touchingHorizontally;
    }

    Vector2Int GetSharedEdgeMidpoint(RectInt a, RectInt b)
    {
        // Rooms touching vertically
        if (a.yMax == b.yMin || b.yMax == a.yMin)
        {
            int minX = Mathf.Max(a.xMin, b.xMin);
            int maxX = Mathf.Min(a.xMax, b.xMax);
            int x = (minX + maxX) / 2;
            int y = a.yMax == b.yMin ? a.yMax : b.yMax; // edge between rooms
            return new Vector2Int(x, y);
        }

        // Rooms touching horizontally
        if (a.xMax == b.xMin || b.xMax == a.xMin)
        {
            int minY = Mathf.Max(a.yMin, b.yMin);
            int maxY = Mathf.Min(a.yMax, b.yMax);
            int y = (minY + maxY) / 2;
            int x = a.xMax == b.xMin ? a.xMax : b.xMax;
            return new Vector2Int(x, y);
        }

        return Vector2Int.zero; // not adjacent
    }
}
