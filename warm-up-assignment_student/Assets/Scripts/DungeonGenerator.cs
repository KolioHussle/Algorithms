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
    public int roomsToCreate;
    public float timeToGenerate;

    void Start()
    {
        startRoom = new RectInt(0, 0, width, height);
        SplittingRooms(startRoom, roomsToCreate);
    }

    void Update()
    {
        AlgorithmsUtils.DebugRectInt(startRoom, Color.yellow);
        StartCoroutine(GenerateDungeon());
        
        /*foreach (var room in newRooms)
        {
            AlgorithmsUtils.DebugRectInt(room, Color.yellow);
        }*/
    }

    void SplittingRooms(RectInt room, int numberOfRooms)
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
        int WidthMinPos = room.xMin + 10;
        int WidthMaxPos = room.xMax - 10;

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

        int randomPointY = Random.Range(HeightMinPos, HeightMaxPos);

        RectInt upRoom = new RectInt(room.x, randomPointY, room.width, room.yMax - randomPointY);
        RectInt downRoom = new RectInt(room.x, room.y, room.width,randomPointY - room.y);

        SplittingRooms(upRoom, numberOfRooms - 1);
        SplittingRooms(downRoom, numberOfRooms - 1);
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
