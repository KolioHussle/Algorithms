using System.Diagnostics;
using UnityEngine;

public class History : MonoBehaviour
{
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

    /*void MakingDoors()
    {
        RectInt doorArea = AlgorithmsUtils.Intersect(roomA, roomB);
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
            }
        }
    }*/

    /*void CreatdEdges()
     {
        for (int i = 0; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                if (AreRoomsAdjacent(newRooms[i], newRooms[j]))
                {
                    Vector2Int doorPosition = GetSharedEdgePoint(newRooms[i], newRooms[j]);

                    if (doorPosition != Vector2Int.zero)
                    {
                        Vector2Int center1 = GetRoomCenter(newRooms[i]);
                        Vector2Int center2 = GetRoomCenter(newRooms[j]);

                        dungeonGraph.AddEdge(center1, doorPosition);
                        dungeonGraph.AddEdge(center2, doorPosition);
                    }
                }
            }
        }
     }*/

    /*void MakingDoors()
    {
        newDoors.Clear();
        Debug.Log("MakingDoors() is running!");
        //List<RectInt> usedDoorPositions = new List<RectInt>();
        List<Vector2Int> usedDoorPositions1 = new List<Vector2Int>();

        for (int i = 0; i < newRooms.Count; i++)
        {
            RectInt roomA = newRooms[i];
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                RectInt roomB = newRooms[j];

                if (AreRoomsAdjacent(roomA, roomB)//AlgorithmsUtils.Intersects(roomA, roomB)//)
                {
                    Debug.Log("hello");
                    Vector2Int doorPosition = GetSharedEdgePoint(roomA, roomB);

                    if (doorPosition != Vector2Int.zero && !usedDoorPositions1.Contains(doorPosition))
                    {
                        newDoors.Add(new RectInt(doorPosition, Vector2Int.one)); // Size of the door is 1x1
                        usedDoorPositions1.Add(doorPosition); // Track the door position
                    }
                }
                
            }
        }
        Debug.Log($"Final door count: {newDoors.Count}");
    }*/

    /*void CreateNodes()
     {
         foreach (var room in newRooms)
         {
             if (room != null)
             {
            Vector2Int center = GetRoomCenter(room);
            dungeonGraph.AddNode(center);
             }
         }
         foreach (var door in newDoors)
         {
            dungeonGraph.AddNode(door.position);
         }
     }*/

    /*void CreateEdges()
    {
        foreach (var door in newDoors)
        {
            Vector2Int doorPos = door.position;

            foreach (var room in newRooms)
            {
                if (IsOnRoomBorder(room, doorPos))
                {
                    Vector2Int roomCenter = GetRoomCenter(room);
                    dungeonGraph.AddEdge(roomCenter, doorPos);
                }
            }
        }
    }*/

    /*bool IsOnRoomBorder(RectInt room, Vector2Int pos)
    {
          bool onXEdge = (pos.x == room.xMin || pos.x == room.xMax - 1) && pos.y >= room.yMin && pos.y < room.yMax;
           bool onYEdge = (pos.y == room.yMin || pos.y == room.yMax - 1) && pos.x >= room.xMin && pos.x < room.xMax;
           return onXEdge || onYEdge;
        // Check if position is adjacent to room borders (in the gap between rooms)
    }*/

    /*public void SpawningDungeonAssets()
    {
        HashSet<Vector2Int> placedFloors = new HashSet<Vector2Int>();
        HashSet<Vector2Int> placedWalls = new HashSet<Vector2Int>();
        HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();

        foreach(var door in newDoors)
        {
            doorPositions.Add(door.position);
        }

        foreach (var room in newRooms)
        {
            // Place floors
            for (int x = 0; x < room.width; x++)
            {
                for (int y = 0; y < room.height; y++)
                {
                    Vector2Int pos = new Vector2Int(room.x + x, room.y + y);
                    if (!placedFloors.Contains(pos))
                    {
                        placedFloors.Add(pos);
                        Instantiate(floor, new Vector3(pos.x, 0, pos.y), floor.transform.rotation);
                    }
                }
            }

            // Place walls along the border
            for (int x = 0; x < room.width; x++)
            {
                Vector2Int bot = new Vector2Int(room.x + x, room.y);
                Vector2Int top = new Vector2Int(room.x + x, room.y + room.height - 1);

                if (!placedWalls.Contains(bot) && !doorPositions.Contains(bot))
                {
                    placedWalls.Add(bot);
                    Instantiate(wall, new Vector3(bot.x + 0.5f, 0.5f, bot.y + 0.5f), Quaternion.identity);
                }

                if (!placedWalls.Contains(top) && !doorPositions.Contains(top))
                {
                    placedWalls.Add(top);
                    Instantiate(wall, new Vector3(top.x + 0.5f, 0.5f, top.y + 0.5f), Quaternion.identity);
                }
            }

            for (int y = 0; y < room.height; y++)
            {
                Vector2Int left = new Vector2Int(room.x, room.y + y);
                Vector2Int right = new Vector2Int(room.x + room.width - 1, room.y + y);

                if (!placedWalls.Contains(left) && !doorPositions.Contains(left))
                {
                    placedWalls.Add(left);
                    Instantiate(wall, new Vector3(left.x + 0.5f, 0.5f, left.y + 0.5f), Quaternion.identity);
                }

                if (!placedWalls.Contains(right) && !doorPositions.Contains(right))
                {
                    placedWalls.Add(right);
                    Instantiate(wall, new Vector3(right.x + 0.5f, 0.5f, right.y + 0.5f), Quaternion.identity);
                }
            }
        }
    }*/

    /*bool AreRoomsAdjacent(RectInt a, RectInt b)
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
    }*/

    /*Vector2Int GetSharedEdgePoint(RectInt a, RectInt b)
    {
         // Rooms touching vertically
         if (a.yMax == b.yMin || b.yMax == a.yMin)
         {
             int minX = Mathf.Max(a.xMin + 2, b.xMin + 2);
             int maxX = Mathf.Min(a.xMax - 2, b.xMax - 2);
             int x = Random.Range(minX, maxX);
             int y = a.yMax == b.yMin ? a.yMax : b.yMax; // edge between rooms
             return new Vector2Int(x, y);
         }

         // Rooms touching horizontally
         if (a.xMax == b.xMin || b.xMax == a.xMin)
         {
             int minY = Mathf.Max(a.yMin + 2, b.yMin + 2);
             int maxY = Mathf.Min(a.yMax - 2, b.yMax - 2);
             int y = Random.Range(minY, maxY);
             int x = a.xMax == b.xMin ? a.xMax : b.xMax;
             return new Vector2Int(x, y);
         }

         return Vector2Int.zero; // not adjacent
    }*/

    /*void SplittingRooms(RectInt startRoom, int maxRooms)
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
    }*/
}
