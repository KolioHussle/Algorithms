using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        startRoom = new RectInt(0, 0, width, height);
        SplittingRooms(startRoom, roomsToCreate);
        MakingDoors();
    }

    void Update()
    {
        AlgorithmsUtils.DebugRectInt(startRoom, Color.yellow);

        StartCoroutine(GenerateDungeon());
        if (hasGenerated)
        {
            StartCoroutine(ShowingDoors());
        }
    }

    void SplittingRooms(RectInt room, int numberOfRooms)
    {
        if (numberOfRooms <= 0 || room.width < 10 || room.height < 10)
        {
            newRooms.Add(room);
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
        newRooms.Add(leftRoom);

        RectInt rightRoom = new RectInt(randomPointX, room.y, room.xMax - randomPointX, room.height);
        newRooms.Add(rightRoom);

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
        newRooms.Add(upRoom);

        RectInt downRoom = new RectInt(room.x, room.y, room.width,randomPointY - room.y);
        newRooms.Add(downRoom);

        SplittingRooms(upRoom, numberOfRooms - 1);
        SplittingRooms(downRoom, numberOfRooms - 1);
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
                    /*if (doorArea.width > doorArea.height) // Horizontal door
                    {
                        doorPos = new Vector2Int(doorArea.x + (doorArea.width / 2) - 1, doorArea.y);
                        doorArea = new RectInt(doorPos, new Vector2Int(2, 1));
                    }
                    else // Vertical door
                    {
                        doorPos = new Vector2Int(doorArea.x, doorArea.y + (doorArea.height / 2) - 1);
                        doorArea = new RectInt(doorPos, new Vector2Int(1, 2));
                    }

                    // Prevent duplicate doors
                    if (!placedDoors.Contains(doorPos))
                    {
                        newDoors.Add(doorArea);
                        placedDoors.Add(doorPos);
                    }*/
                    if (doorArea.width > doorArea.height) // Horizontal door
                    {
                        int doorX = doorArea.x + (doorArea.width / 2);
                        int doorY = doorArea.y - 1; // Small gap between rooms
                        doorPos = new Vector2Int(doorX, doorY);
                        doorArea = new RectInt(doorPos, new Vector2Int(2, 2)); // Make the door a small square
                    }
                    else // Vertical door
                    {
                        int doorX = doorArea.x - 1; // Small gap between rooms
                        int doorY = doorArea.y + (doorArea.height / 2);
                        doorPos = new Vector2Int(doorX, doorY);
                        doorArea = new RectInt(doorPos, new Vector2Int(2, 2)); // Make the door a small square
                    }
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
