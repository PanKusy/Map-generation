using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject roomPrefab; // Prefab pokoju
    public int totalLevels = 3; // Liczba poziomów
    public int roomsPerLevel = 3; // Liczba pokoi na poziomie
    public float spacing = 2.0f; // Odleg³oœæ miêdzy pokojami w poziomie

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        List<Room> rooms = new List<Room>();

        for (int level = 1; level <= totalLevels; level++)
        {
            for (int i = 0; i < roomsPerLevel; i++)
            {
                Vector3 position = new Vector3(i * spacing, level * 2, 0); // Ustal pozycjê w linii
                GameObject newRoomObject = Instantiate(roomPrefab, position, Quaternion.identity);
                Room newRoom = new Room(position, newRoomObject, level);
                rooms.Add(newRoom);
            }
        }

        // Po³¹czenia miêdzy pokojami
        CreateConnections(rooms);
    }

    void CreateConnections(List<Room> rooms)
    {
        for (int level = 1; level < totalLevels; level++)
        {
            var currentLevelRooms = rooms.Where(r => r.level == level).ToList();
            var nextLevelRooms = rooms.Where(r => r.level == level + 1).ToList();

            foreach (var roomA in currentLevelRooms)
            {
                foreach (var roomB in nextLevelRooms)
                {
                    ConnectRooms(roomA, roomB);
                }
            }
        }
    }

    void ConnectRooms(Room roomA, Room roomB)
    {
        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);

        LineRenderer line = roomA.roomObject.AddComponent<LineRenderer>();
        line.SetPosition(0, roomA.position);
        line.SetPosition(1, roomB.position);
        line.startWidth = 0.1f; // Gruboœæ linii
        line.endWidth = 0.1f;   // Gruboœæ linii
        line.material = new Material(Shader.Find("Sprites/Default")); // Ustaw materia³
        line.startColor = Color.white; // Kolor linii
        line.endColor = Color.white;   // Kolor linii
    }
}
