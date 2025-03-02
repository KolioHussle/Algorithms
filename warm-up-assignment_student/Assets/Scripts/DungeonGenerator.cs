using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public RectInt startRoom = new RectInt(0, 0, 100, 50);
    private List<RectInt> newRooms = new List<RectInt>();
    public int roomsToCreate;

    void Start()
    {
        VerticalSplit(startRoom, roomsToCreate);
        //StartCoroutine(GenerateDungeon());
    }

    void Update()
    {
        foreach (var room in newRooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }
    }

    void VerticalSplit(RectInt room, int numberOfRooms)
    {
        if (numberOfRooms <= 0)
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
        int WidthMinPos = startRoom.xMin + 5;
        int WidthMaxPos = startRoom.xMax - 5;

        int randomPointX = Random.Range(WidthMinPos, WidthMaxPos);

        RectInt leftRoom = new RectInt(room.x, room.y, randomPointX - room.x, room.height);
        RectInt rightRoom = new RectInt(room.x, room.y, room.x - randomPointX, room.height);

        VerticalSplit(leftRoom, numberOfRooms - 1);
        VerticalSplit(rightRoom, numberOfRooms - 1);
        }

        else 
        {         
        int HeightMinPos = startRoom.yMin + 5;
        int HeightMaxPos = startRoom.yMax - 5;

        int randomPointY = Random.Range(HeightMinPos, HeightMaxPos);

        RectInt upRoom = new RectInt(room.x, room.y, room.width, room.y - randomPointY);
        RectInt downRoom = new RectInt(room.x, room.y, room.width,randomPointY - room.y);

        VerticalSplit(upRoom, numberOfRooms - 1);
        VerticalSplit(downRoom, numberOfRooms - 1);
        }
    }

    IEnumerator GenerateDungeon()
    {
        yield return new WaitForSeconds(1f);

        foreach (var room in newRooms)
        {
            VerticalSplit(room, roomsToCreate);
        }
    }
}
