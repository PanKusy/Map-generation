using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector3 position;
    public GameObject roomObject;
    public List<Room> connectedRooms;
    public int level; // Poziom pokoju

    public Room(Vector3 pos, GameObject obj, int lvl)
    {
        position = pos;
        roomObject = obj;
        connectedRooms = new List<Room>();
        level = lvl; // Ustal poziom
    }
}
