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

        Debug.Log($"Total Rooms: {newRooms.Count}");
        Debug.Log($"Total Graph Nodes: {dungeonGraph.GetNodes().Count}");
        dungeonGraph.PrintGraph();
    }

    void Update()
    {
        AlgorithmsUtils.DebugRectInt(startRoom, Color.yellow);

        StartCoroutine(GenerateDungeon());
        if (hasGenerated)
        {
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
            //randomPointX = Mathf.RoundToInt(randomPointX / 2f) * 2;

        RectInt leftRoom = new RectInt(room.x, room.y, randomPointX - room.x, room.height);
        newRooms.Add(leftRoom);

        RectInt rightRoom = new RectInt(randomPointX, room.y, room.xMax - randomPointX, room.height);
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
            //randomPointY = Mathf.RoundToInt(randomPointY / 2f) * 2;

        RectInt upRoom = new RectInt(room.x, randomPointY, room.width, room.yMax - randomPointY);
        newRooms.Add(upRoom);

        RectInt downRoom = new RectInt(room.x, room.y, room.width,randomPointY - room.y);
        newRooms.Add(downRoom);

        //Debug.Log($" Splitting room at {room.position}, size {room.size} -> Up: {upRoom.position} {upRoom.size}, Down: {downRoom.position} {downRoom.size}");

        SplittingRooms(upRoom, numberOfRooms - 1);
        SplittingRooms(downRoom, numberOfRooms - 1);
        }
    }

    void SplittingRooms1(RectInt room, int numberOfRooms)
    {
        if (numberOfRooms <= 0 || room.width < 10 || room.height < 10)
        {
            newRooms.Add(room);
            Debug.Log($"Added room at ({room.xMin}, {room.yMin}) with size ({room.width}, {room.height})");
            return;
        }

        // Split the room either horizontally or vertically
        int randomDirection = Random.Range(0, 2);

        if (randomDirection == 0) // Split horizontally
        {
            // We need to split the room into two horizontally adjacent rooms
            int splitX = Random.Range(room.xMin + 5, room.xMax - 5);
            if (splitX < room.xMin || splitX > room.xMax) return; // Ensure the split is within bounds

            RectInt leftRoom = new RectInt(room.x, room.y, splitX - room.x, room.height);
            RectInt rightRoom = new RectInt(splitX, room.y, room.xMax - splitX, room.height);

            newRooms.Add(leftRoom);
            newRooms.Add(rightRoom);

            // Recursively split both the left and right rooms
            SplittingRooms(leftRoom, numberOfRooms - 1);
            SplittingRooms(rightRoom, numberOfRooms - 1);
        }
        else // Split vertically
        {
            // Split the room into two vertically adjacent rooms
            int splitY = Random.Range(room.yMin + 5, room.yMax - 5);
            if (splitY < room.yMin || splitY > room.yMax) return; // Ensure the split is within bounds

            RectInt topRoom = new RectInt(room.x, room.y, room.width, splitY - room.y);
            RectInt bottomRoom = new RectInt(room.x, splitY, room.width, room.yMax - splitY);

            newRooms.Add(topRoom);
            newRooms.Add(bottomRoom);

            // Recursively split both the top and bottom rooms
            SplittingRooms(topRoom, numberOfRooms - 1);
            SplittingRooms(bottomRoom, numberOfRooms - 1);
        }
    }

    void MakingDoors()
    {
        newDoors.Clear();
        List<Vector2Int> placedDoors = new List<Vector2Int>();

        for (int i = 1; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersects(newRooms[i], newRooms[j]))
                {
                     RectInt doorArea = AlgorithmsUtils.Intersect(newRooms[i], newRooms[j]);

                    // Ensure a 2x2 door at the center of the intersection
                    Vector2Int doorPos;
                    RectInt doorRect;
                     if (doorArea.width > doorArea.height) // Horizontal door
                     {
                         doorPos = new Vector2Int(doorArea.x + (doorArea.width / 2) - 1, doorArea.y);
                         doorRect = new RectInt(doorPos, new Vector2Int(2, 1));
                         /*if (AlgorithmsUtils.Intersects(doorArea, newRooms[j + 1]))
                         {
                             return;
                         }
                         else if (AlgorithmsUtils.Intersects(doorArea, newRooms[j - 1]))
                         {
                             return;
                         }*/
                     }
                     else // Vertical door
                     {
                         doorPos = new Vector2Int(doorArea.x, doorArea.y + (doorArea.height / 2) - 1);
                         doorRect = new RectInt(doorPos, new Vector2Int(1, 2));
                         /*if (AlgorithmsUtils.Intersects(doorArea, newRooms[j + 1]))
                         {
                             return;
                         }
                         else if (AlgorithmsUtils.Intersects(doorArea, newRooms[j - 1]))
                         {
                             return;
                         }*/
                     }
                    /*bool isInsideAnotherRoom = false;

                    foreach (var room in newRooms)
                    {
                        if (room != newRooms[i] && room != newRooms[j] && room.Contains(doorPos))
                        {
                            isInsideAnotherRoom = true;
                            break;
                        }
                    }*/

                    // bool isOnBoundary = (newRooms[i].Overlaps(doorRect) && newRooms[j].Overlaps(doorRect));

                    /* int touchingRooms = 0;
                     foreach (var room in newRooms)
                     {
                         if (room.Overlaps(doorRect))
                         {
                             touchingRooms++;
                         }
                     }*/


                     // Prevent duplicate doors
                     if (!placedDoors.Contains(doorPos))
                     {
                         newDoors.Add(doorRect);
                         placedDoors.Add(doorPos);
                         Debug.Log($"Placed door at {doorPos}");
                     }
                  
                }
            }
        }
    }

    void MakingDoors1()
    {
        newDoors.Clear();
        List<Vector2Int> placedDoors = new List<Vector2Int>();

        for (int i = 0; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                if (AlgorithmsUtils.Intersects(newRooms[i], newRooms[j]))
                {
                    RectInt doorArea = AlgorithmsUtils.Intersect(newRooms[i], newRooms[j]);

                    // Only place doors if there is enough space (at least 2x1 or 1x2)
                    if (doorArea.width >= 2 || doorArea.height >= 2)
                    {
                        Vector2Int doorPos = Vector2Int.zero;
                        RectInt doorRect = new RectInt();

                        // Horizontal door placement (shared width)
                        if (doorArea.width > doorArea.height) // Horizontal door
                        {
                            // Place door in the center of the shared wall
                            doorPos = new Vector2Int(doorArea.x + (doorArea.width / 2) - 1, doorArea.y);
                            doorRect = new RectInt(doorPos, new Vector2Int(2, 1)); // 2x1 door

                            // Prevent placing duplicate doors on the same spot
                            if (!placedDoors.Contains(doorPos))
                            {
                                newDoors.Add(doorRect);
                                placedDoors.Add(doorPos);
                                //Debug.Log($"Placed horizontal door at {doorPos}");
                            }
                        }
                        else if (doorArea.height > doorArea.width) // Vertical door
                        {
                            // Place door in the center of the shared wall
                            doorPos = new Vector2Int(doorArea.x, doorArea.y + (doorArea.height / 2) - 1);
                            doorRect = new RectInt(doorPos, new Vector2Int(1, 2)); // 1x2 door

                            // Prevent placing duplicate doors on the same spot
                            if (!placedDoors.Contains(doorPos))
                            {
                                newDoors.Add(doorRect);
                                placedDoors.Add(doorPos);
                                //Debug.Log($"Placed vertical door at {doorPos}");
                            }
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
}
