using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public RectInt startRoom;
    private List<RectInt> newRooms = new List<RectInt>();
    private List<RectInt> newDoors = new List<RectInt>();
    public int roomsToCreate;
    public float timeToGenerate;

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
            StartCoroutine(ShowingDoors());

        /*foreach (var room in newRooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }*/
    }

    void SplittingRooms(RectInt room, int numberOfRooms)
    {
        if (numberOfRooms <= 0 || room.width < 20 || room.height < 20)
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
        int WidthMinPos = room.xMin + 10;
        int WidthMaxPos = room.xMax - 10;

        if (WidthMinPos >= WidthMaxPos) return;

        int randomPointX = Random.Range(WidthMinPos, WidthMaxPos);

        RectInt leftRoom = new RectInt(room.x, room.y, randomPointX - room.x, room.height);
        RectInt rightRoom = new RectInt(randomPointX, room.y, room.xMax - randomPointX, room.height);

        SplittingRooms(leftRoom, numberOfRooms - 1);
        SplittingRooms(rightRoom, numberOfRooms - 1);
        }

        else 
        {         
        int HeightMinPos = room.yMin + 10;
        int HeightMaxPos = room.yMax - 10;

        if (HeightMinPos >= HeightMaxPos) return;

        int randomPointY = Random.Range(HeightMinPos, HeightMaxPos);

        RectInt upRoom = new RectInt(room.x, randomPointY, room.width, room.yMax - randomPointY);
        RectInt downRoom = new RectInt(room.x, room.y, room.width,randomPointY - room.y);

        SplittingRooms(upRoom, numberOfRooms - 1);
        SplittingRooms(downRoom, numberOfRooms - 1);
        }
    }

    void MakingDoors()
    {
        newDoors.Clear();
        Debug.Log($" Creating doors for {newRooms.Count} rooms...");

        for (int i = 0; i < newRooms.Count; i++)
        {
            for (int j = i + 1; j < newRooms.Count; j++)
            {
                if (j >= newRooms.Count) continue;

                if (AlgorithmsUtils.Intersects(newRooms[i], newRooms[j]))
                {
                    Debug.Log($" Room {i} at {newRooms[i].position} intersects with Room {j} at {newRooms[j].position}");
                    RectInt door = AlgorithmsUtils.Intersect(newRooms[i], newRooms[j]);

                    if (door.width > door.height)
                    {
                        door.height = 2;
                    }
                    else
                    {
                        door.width = 2;
                    }

                    newDoors.Add(door);
                    Debug.Log($" Door added at {door.position} with size {door.size}");
                }
            }
        }
        Debug.Log($" Total doors created: {newDoors.Count}");
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
    }
}
