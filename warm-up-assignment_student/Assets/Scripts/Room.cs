using System.Collections.Generic;
using UnityEngine;

public class Room
{
        public RectInt area;
        public Room parentRoom; // Parent room (null if it’s a top-level room)
        public List<Room> childRooms = new List<Room>(); // Child rooms inside this room

        public Room(RectInt area)
        {
            this.area = area;
        }
}
