using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
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

    private Graph<Vector2Int> dungeonGraph;
    [SerializeField]
    private GameObject wall;
    [SerializeField]
    private GameObject floor;

    public NavMeshSurface navMeshSurface;


    /// <summary>
    /// Initializes the dungeon generation process and builds the complete dungeon
    void Start()
    {
        dungeonGraph = new Graph<Vector2Int>();

        startRoom = new RectInt(0, 0, width, height);

        SplittingRooms(startRoom, roomsToCreate);
        MakingDoors();

        CreateNodes();
        CreatdEdges();

        SpawnDungeonAssets();
        BakeNavMesh();
    }


    /// <summary>
    /// Handles visual debugging and coroutine management for dungeon generation
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


    /// <summary>
    /// Recursively splits rooms using BSP algorithm until target room count is reached
    void SplittingRooms(RectInt startRoom, int maxRooms)
    {
        /*newRooms.Clear();
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
                // Create bottom room (same as before)
                RectInt bottom = new RectInt(currentRoom.x, currentRoom.y, currentRoom.width, splitY - currentRoom.y);
                // Create top room with 1-unit gap (start from splitY + 1)
                RectInt top = new RectInt(currentRoom.x, splitY + 1, currentRoom.width, currentRoom.yMax - (splitY + 1));

                queue.Enqueue(bottom);
                queue.Enqueue(top);
            }
            else if (!splitHorizontally && currentRoom.width >= 10)
            {
                int splitX = Random.Range(currentRoom.xMin + 5, currentRoom.xMax - 5);
                // Create left room (same as before)
                RectInt left = new RectInt(currentRoom.x, currentRoom.y, splitX - currentRoom.x, currentRoom.height);
                // Create right room with 1-unit gap (start from splitX + 1)
                RectInt right = new RectInt(splitX + 1, currentRoom.y, currentRoom.xMax - (splitX + 1), currentRoom.height);

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
        }*/
        newRooms.Clear();
        Queue<RectInt> queue = new Queue<RectInt>();
        queue.Enqueue(startRoom);

        while (queue.Count > 0 && newRooms.Count < maxRooms)
        {
            RectInt currentRoom = queue.Dequeue();

            // Check if room is too small to split or if we would exceed max rooms
            if ((currentRoom.width < 10 || currentRoom.height < 10) || newRooms.Count + queue.Count + 1 >= maxRooms)
            {
                newRooms.Add(currentRoom);
                continue;
            }

            bool splitHorizontally = Random.value > 0.5f;
            bool canSplitHorizontally = currentRoom.height >= 5;
            bool canSplitVertically = currentRoom.width >= 5;

            // If we can't split in the chosen direction, try the other
            if (splitHorizontally && !canSplitHorizontally && canSplitVertically)
            {
                splitHorizontally = false;
            }
            else if (!splitHorizontally && !canSplitVertically && canSplitHorizontally)
            {
                splitHorizontally = true;
            }

            if (splitHorizontally && canSplitHorizontally)
            {
                int splitY = Random.Range(currentRoom.yMin + 5, currentRoom.yMax - 5);
                // Create bottom room
                RectInt bottom = new RectInt(currentRoom.x, currentRoom.y, currentRoom.width, splitY - currentRoom.y);
                // Create top room with 1-unit gap
                RectInt top = new RectInt(currentRoom.x, splitY + 1, currentRoom.width, currentRoom.yMax - (splitY + 1));

                queue.Enqueue(bottom);
                queue.Enqueue(top);
            }
            else if (!splitHorizontally && canSplitVertically)
            {
                int splitX = Random.Range(currentRoom.xMin + 5, currentRoom.xMax - 5);
                // Create left room
                RectInt left = new RectInt(currentRoom.x, currentRoom.y, splitX - currentRoom.x, currentRoom.height);
                // Create right room with 1-unit gap
                RectInt right = new RectInt(splitX + 1, currentRoom.y, currentRoom.xMax - (splitX + 1), currentRoom.height);

                queue.Enqueue(left);
                queue.Enqueue(right);
            }
            else
            {
                // Can't split in either direction, add to final rooms
                newRooms.Add(currentRoom);
            }
        }

        // Add any remaining rooms in queue (happens when we reach maxRooms limit)
        while (queue.Count > 0)
        {
            newRooms.Add(queue.Dequeue());
        }
    }


    /// <summary>
    /// Creates doors between adjacent rooms by finding shared edge points
    void MakingDoors()
    {
        newDoors.Clear();
        Debug.Log("MakingDoors() is running!");
        List<Vector2Int> usedDoorPositions1 = new List<Vector2Int>();

        for (int i = 0; i < newRooms.Count; i++)
        {
            RectInt roomA = newRooms[i];
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                RectInt roomB = newRooms[j];

                if (AreRoomsAdjacent(roomA, roomB))
                {
                    Debug.Log($"Adjacent rooms found: Room A({roomA.xMin}, {roomA.yMin}) and Room B({roomB.xMin}, {roomB.yMin})");
                    Vector2Int doorPosition = GetSharedEdgePoint(roomA, roomB);

                    if (doorPosition != Vector2Int.zero && !usedDoorPositions1.Contains(doorPosition))
                    {
                        newDoors.Add(new RectInt(doorPosition, Vector2Int.one));
                        usedDoorPositions1.Add(doorPosition);
                        Debug.Log($"Door created at: {doorPosition}");
                    }
                }
            }
        }
        Debug.Log($"Final door count: {newDoors.Count}");
    }


    /// <summary>
    /// Adds room centers and door positions as nodes to the dungeon graph
    void CreateNodes()
    {
        foreach (var room in newRooms)
        {
            if (room != null)
            {
                Vector2Int center = GetRoomCenter(room);
                dungeonGraph.AddNode(center);
                Debug.Log($"Added room node at: {center}");
            }
        }
        foreach (var door in newDoors)
        {
            dungeonGraph.AddNode(door.position);
            Debug.Log($"Added door node at: {door.position}");
        }
    }


    /// <summary>
    /// Creates graph edges connecting room centers to their adjacent doors
    void CreatdEdges()
    {
        Debug.Log("Creating edges...");
        int edgeCount = 0;

        foreach (var door in newDoors)
        {
            Vector2Int doorPos = door.position;
            Debug.Log($"Checking door at: {doorPos}");

            foreach (var room in newRooms)
            {
                if (IsOnRoomBorder(room, doorPos))
                {
                    Vector2Int roomCenter = GetRoomCenter(room);
                    dungeonGraph.AddEdge(roomCenter, doorPos);
                    edgeCount++;
                    Debug.Log($"Edge created between room center {roomCenter} and door {doorPos}");
                }
            }
        }
        Debug.Log($"Total edges created: {edgeCount}");
    }


    /// <summary>
    /// Checks if a position is adjacent to a room's border (used for door placement validation)
    bool IsOnRoomBorder(RectInt room, Vector2Int pos)
    {
        bool adjacentToXEdge = ((pos.x == room.xMin - 1 || pos.x == room.xMax) &&
                               pos.y >= room.yMin && pos.y < room.yMax);
        bool adjacentToYEdge = ((pos.y == room.yMin - 1 || pos.y == room.yMax) &&
                               pos.x >= room.xMin && pos.x < room.xMax);

        Debug.Log($"Checking if door at {pos} is on border of room ({room.xMin}, {room.yMin}, {room.width}, {room.height}): adjacentX={adjacentToXEdge}, adjacentY={adjacentToYEdge}");

        return adjacentToXEdge || adjacentToYEdge;
    }


    /// <summary>
    /// Draws debug visualization of the dungeon graph showing nodes and connections
    void VisualizeGraph()
    {
        foreach (var node in dungeonGraph.GetNodes())
        {
            // Visualize the node (center of the room + doors) with a circle
            Vector3 nodePosition = new Vector3(node.x, 0, node.y); // Convert from 2D to 3D space
            DebugExtension.DebugCircle(nodePosition);

            foreach (var neighbor in dungeonGraph.GetNeighbors(node))
            {
                // Draw a line to each neighbor
                Vector3 neighborPosition = new Vector3(neighbor.x, 0, neighbor.y); // Convert from 2D to 3D space
                Debug.DrawLine(nodePosition, neighborPosition, Color.red);
            }
        }
    }


    /// <summary>
    /// Instantiates floor and wall GameObjects to create the physical dungeon structure
    public void SpawnDungeonAssets()
    {
        HashSet<Vector2Int> placedFloors = new HashSet<Vector2Int>();
        HashSet<Vector2Int> placedWalls = new HashSet<Vector2Int>();
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();

        foreach (var door in newDoors)
        {
            doorPositions.Add(door.position);
        }

        // Get the bounds of the entire dungeon (including the start room)
        int minX = startRoom.xMin;
        int maxX = startRoom.xMax;
        int minY = startRoom.yMin;
        int maxY = startRoom.yMax;

        // Place floors everywhere within the dungeon bounds
        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!placedFloors.Contains(pos))
                {
                    placedFloors.Add(pos);
                    Instantiate(floor, new Vector3(pos.x, 0, pos.y), floor.transform.rotation);
                }
            }
        }

        // Fill the gap areas with walls (except where doors are)
        FillGapAreas(placedWalls, doorPositions);

        // Add perimeter walls around the entire dungeon
        AddPerimeterWalls(placedWalls, minX, maxX, minY, maxY);
    }


    /// <summary>
    /// Places wall objects in gap areas between rooms, excluding door positions
    void FillGapAreas(HashSet<Vector2Int> placedWalls, HashSet<Vector2Int> doorPositions)
    {
        // Use the start room bounds for the entire dungeon
        int minX = startRoom.xMin;
        int maxX = startRoom.xMax;
        int minY = startRoom.yMin;
        int maxY = startRoom.yMax;

        // Check every position in the dungeon bounds
        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                // If this position is not inside any room and not already a wall and not a door
                if (!IsInsideAnyRoom(pos) && !placedWalls.Contains(pos) && !doorPositions.Contains(pos))
                {
                    placedWalls.Add(pos);
                    Instantiate(wall, new Vector3(pos.x + 0.5f, 0.5f, pos.y + 0.5f), Quaternion.identity);
                }
            }
        }
    }


    /// <summary>
    /// Creates perimeter walls around the entire dungeon boundaries
    void AddPerimeterWalls(HashSet<Vector2Int> placedWalls, int minX, int maxX, int minY, int maxY)
    {
        // Add walls around the entire perimeter of the dungeon
        for (int x = minX - 1; x <= maxX; x++)
        {
            // Bottom wall
            Vector2Int bot = new Vector2Int(x, minY - 1);
            if (!placedWalls.Contains(bot))
            {
                placedWalls.Add(bot);
                Instantiate(wall, new Vector3(bot.x + 0.5f, 0.5f, bot.y + 0.5f), Quaternion.identity);
            }

            // Top wall
            Vector2Int top = new Vector2Int(x, maxY);
            if (!placedWalls.Contains(top))
            {
                placedWalls.Add(top);
                Instantiate(wall, new Vector3(top.x + 0.5f, 0.5f, top.y + 0.5f), Quaternion.identity);
            }
        }

        for (int y = minY - 1; y <= maxY; y++)
        {
            // Left wall
            Vector2Int left = new Vector2Int(minX - 1, y);
            if (!placedWalls.Contains(left))
            {
                placedWalls.Add(left);
                Instantiate(wall, new Vector3(left.x + 0.5f, 0.5f, left.y + 0.5f), Quaternion.identity);
            }

            // Right wall
            Vector2Int right = new Vector2Int(maxX, y);
            if (!placedWalls.Contains(right))
            {
                placedWalls.Add(right);
                Instantiate(wall, new Vector3(right.x + 0.5f, 0.5f, right.y + 0.5f), Quaternion.identity);
            }
        }
    }


    /// <summary>
    /// Checks if a given position is inside any of the generated rooms
    bool IsInsideAnyRoom(Vector2Int pos)
    {
        foreach (var room in newRooms)
        {
            if (pos.x >= room.xMin && pos.x < room.xMax && pos.y >= room.yMin && pos.y < room.yMax)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Coroutine that visualizes doors with timed delays for debugging purposes
    IEnumerator ShowingDoors()
    {
        foreach(var door in newDoors)
        {
            yield return new WaitForSeconds(timeToGenerate);
            AlgorithmsUtils.DebugRectInt(door, Color.blue);
        }
        VisualizeGraph();

        /*SpawnDungeonAssets();

        BakeNavMesh();*/
    }


    /// <summary>
    /// Coroutine that visualizes room generation with timed delays for debugging
    IEnumerator GenerateDungeon()
    {
        foreach (var room in newRooms)
        {
            yield return new WaitForSeconds(timeToGenerate);
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }

        hasGenerated = true;
    }


    /// <summary>
    /// Builds the NavMesh for AI pathfinding on the generated dungeon
    public void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("NavMeshSurface is null - cannot bake NavMesh");
        }
    }


    /// <summary>
    /// Determines if two rooms are adjacent with exactly 1 unit separation
    bool AreRoomsAdjacent(RectInt a, RectInt b)
    {
        // Check if rooms are separated by exactly 1 unit horizontally
        bool adjacentVertically =
            (a.xMin < b.xMax && a.xMax > b.xMin) &&
            (a.yMax + 1 == b.yMin || b.yMax + 1 == a.yMin);

        // Check if rooms are separated by exactly 1 unit vertically
        bool adjacentHorizontally =
            (a.yMin < b.yMax && a.yMax > b.yMin) &&
            (a.xMax + 1 == b.xMin || b.xMax + 1 == a.xMin);

        return adjacentVertically || adjacentHorizontally;
    }


    /// <summary>
    /// Finds a random point along the shared edge between two adjacent rooms for door placement
    Vector2Int GetSharedEdgePoint(RectInt a, RectInt b)
    {
        // Rooms separated vertically by 1 unit
        if (a.yMax + 1 == b.yMin || b.yMax + 1 == a.yMin)
        {
            int minX = Mathf.Max(a.xMin, b.xMin);
            int maxX = Mathf.Min(a.xMax - 1, b.xMax - 1);
            int x = Random.Range(minX, maxX + 1);
            int y = a.yMax + 1 == b.yMin ? a.yMax : b.yMax; // The gap position
            return new Vector2Int(x, y);
        }

        // Rooms separated horizontally by 1 unit
        if (a.xMax + 1 == b.xMin || b.xMax + 1 == a.xMin)
        {
            int minY = Mathf.Max(a.yMin, b.yMin);
            int maxY = Mathf.Min(a.yMax - 1, b.yMax - 1);
            int y = Random.Range(minY, maxY + 1);
            int x = a.xMax + 1 == b.xMin ? a.xMax : b.xMax; // The gap position
            return new Vector2Int(x, y);
        }

        return Vector2Int.zero; // not adjacent
    }


    /// <summary>
    /// Gets the center point of the rooms
    Vector2Int GetRoomCenter(RectInt room)
    {
        int centerX = room.xMin + room.width / 2;
        int centerY = room.yMin + room.height / 2;
        return new Vector2Int(centerX, centerY);
    }
}
