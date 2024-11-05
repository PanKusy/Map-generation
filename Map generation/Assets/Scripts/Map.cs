using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public List<Room> rooms;

    public Map()
    {
        rooms = new List<Room>();
    }

    public void AddRoom(Room room)
    {
        rooms.Add(room);
    }

    public void ConnectRooms(Room roomA, Room roomB)
    {
        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }
}