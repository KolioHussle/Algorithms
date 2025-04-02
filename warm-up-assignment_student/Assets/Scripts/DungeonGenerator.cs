using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width;
    public int height;
    private RectInt startRoom;
    private List<RectInt> newRooms = new List<RectInt>();
    private List<RectInt> newDoors = new List<RectInt>();
    public int roomsToCreate;
    //private int roomCount = 0;
    public float timeToGenerate;
    private bool hasGenerated = false;

    private Graph<RectInt> dungeonGraph;

    void Start()
    {
        dungeonGraph = new Graph<RectInt>();

        startRoom = new RectInt(0, 0, width, height);
        SplittingRooms(startRoom, roomsToCreate);
        //SplittingRooms1(startRoom, roomsToCreate);
        MakingDoors();
        //MakingDoors1();

        CreateNodes();
        CreatdEdges();

       /* Debug.Log($"Total Rooms: {newRooms.Count}");
        Debug.Log($"Total Graph Nodes: {dungeonGraph.GetNodes().Count}");*/
        dungeonGraph.PrintGraph();
        //TestMinimalGraph();
    }

    void Update()
    {
        AlgorithmsUtils.DebugRectInt(startRoom, Color.yellow);

        StartCoroutine(GenerateDungeon());
        if (hasGenerated)
        {
            //Debug.Log($"Final Room Count: {newRooms.Count}");
            StartCoroutine(ShowingDoors());
        }

        foreach (var room in newRooms)
        {
            List<RectInt> neighbors = dungeonGraph.GetNeighbors(room);
            if (neighbors != null)
            {
                foreach (var neighbor in neighbors)
                {
                    Vector2 roomCenter = new Vector2(room.x + room.width / 2f, room.y + room.height / 2f);
                    Vector2 neighborCenter = new Vector2(neighbor.x + neighbor.width / 2f, neighbor.y + neighbor.height / 2f);
                    Debug.DrawLine(roomCenter, neighborCenter, Color.green);
                }
            }
        }

        /*foreach (var room in newRooms)
        {
            Debug.Log($"Room position: {room.position}, size: {room.size}");
        }*/
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



    void SplittingRooms(RectInt room, int numberOfRooms)
    {
        if (numberOfRooms <= 0 || room.width < 10 || room.height < 10)
        {
            newRooms.Add(room);
            //Debug.Log($" Added final room at {room.position} with size {room.size}");
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
    }

    void MakingDoors()
    {
        newDoors.Clear();
        Debug.Log("MakingDoors() is running!");

        for (int i = 1; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersects(newRooms[i], newRooms[j]))
                {
                    RectInt doorArea = AlgorithmsUtils.Intersect(newRooms[i], newRooms[j]);

                    if (doorArea.width >= 2 && doorArea.height >= 2) // Ensure enough space for a door
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
                        bool doorExists = newDoors.Any(door => door.x == doorPos.x && door.y == doorPos.y && door.width == doorSize.x && door.height == doorSize.y);

                        if (!doorExists)
                        {
                            newDoors.Add(new RectInt(doorPos, doorSize));
                        }
                    }
                }
            }
        }
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

    /*void SplittingRooms1(RectInt room, int numberOfRooms, Room parentRoom = null)
    {
        if (numberOfRooms <= 0 || room.width < 10 || room.height < 10)
        {
            Room newRoom = new Room(room);
            newRoom.parentRoom = parentRoom;
            if (parentRoom != null)
            {
                parentRoom.childRooms.Add(newRoom);
            }
            newRooms.Add(newRoom); // Add the Room object to newRooms
            return;
        }

        int randomNumber = Random.Range(0, 2);
        bool directionDevider = randomNumber == 0;

        if (directionDevider) // Vertical Split
        {
            int minX = room.xMin + 5;
            int maxX = room.xMax - 5;
            if (minX >= maxX) return;

            int splitX = Random.Range(minX, maxX);
            RectInt leftRoom = new RectInt(room.x, room.y, splitX - room.x, room.height);
            RectInt rightRoom = new RectInt(splitX, room.y, room.xMax - splitX, room.height);

            SplittingRooms1(leftRoom, numberOfRooms - 1, parentRoom);
            SplittingRooms1(rightRoom, numberOfRooms - 1, parentRoom);
        }
        else // Horizontal Split
        {
            int minY = room.yMin + 5;
            int maxY = room.yMax - 5;
            if (minY >= maxY) return;

            int splitY = Random.Range(minY, maxY);
            RectInt topRoom = new RectInt(room.x, splitY, room.width, room.yMax - splitY);
            RectInt bottomRoom = new RectInt(room.x, room.y, room.width, splitY - room.y);

            SplittingRooms1(topRoom, numberOfRooms - 1, parentRoom);
            SplittingRooms1(bottomRoom, numberOfRooms - 1, parentRoom);
        }
    }*/

    /*void MakingDoors1()
    {
        newDoors.Clear();
        Debug.Log("MakingDoors() is running!");

        HashSet<string> processedWalls = new HashSet<string>();

        for (int i = 1; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                Room room1 = newRooms[i]; // Now we are using the Room object
                Room room2 = newRooms[j];

                // Check if either room is a child of the other
                if (IsChildRoom(room1, room2) || IsChildRoom(room2, room1))
                    continue; // Skip if one room is inside the other

                if (AlgorithmsUtils.Intersects(room1.area, room2.area))
                {
                    RectInt doorArea = AlgorithmsUtils.Intersect(room1.area, room2.area);

                    if (doorArea.width >= 2 && doorArea.height >= 2) // Ensure enough space for a door
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

                        string wallKey = GetWallKey(room1.area, room2.area);

                        if (!processedWalls.Contains(wallKey))
                        {
                            newDoors.Add(new RectInt(doorPos, doorSize));
                            processedWalls.Add(wallKey);
                        }
                    }
                }
            }
        }
    }*/

   /* bool IsChildRoom(Room parent, Room child)
    {
        return parent.childRooms.Contains(child);
    }*/

    /*string GetWallKey(RectInt room1, RectInt room2)
    {
        // Sort the coordinates to ensure consistency in the key
        int x1 = Mathf.Min(room1.x, room2.x);
        int y1 = Mathf.Min(room1.y, room2.y);
        int x2 = Mathf.Max(room1.x + room1.width, room2.x + room2.width);
        int y2 = Mathf.Max(room1.y + room1.height, room2.y + room2.height);

        return $"{x1},{y1},{x2},{y2}";
    }*/

}
