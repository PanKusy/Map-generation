using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector3 position;
    public GameObject roomObject; // Dodajemy GameObject
    public List<Room> connectedRooms;
    public int level;

    public Room(Vector3 pos, GameObject obj, int lvl)
    {
        position = pos;
        roomObject = obj; // Przechowujemy referencj� do GameObject
        connectedRooms = new List<Room>();
        level = lvl; // Ustal poziom
    }
}
