using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    int totalLevels = 3;


    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        List<Room> rooms = new List<Room>();
        int totalLevels = 3; // Liczba poziom�w

        for (int level = 1; level <= totalLevels; level++)
        {
            int roomsPerLevel = 3; // Liczba pokoi na danym poziomie

            for (int i = 0; i < roomsPerLevel; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), level * 2, Random.Range(-10f, 10f)); // Ustal pozycj� na poziomie
                GameObject newRoomObject = Instantiate(roomPrefab, randomPosition, Quaternion.identity);
                Room newRoom = new Room(randomPosition, newRoomObject, level); // Przekazuj poziom
                rooms.Add(newRoom);
            }
        }

        // Po��czenia mi�dzy pokojami
        CreateConnections(rooms, totalLevels);
    }


    void ConnectRooms(Room roomA, Room roomB)
    {
        // Dodaj roomB do po��cze� roomA
        roomA.connectedRooms.Add(roomB);

        // Dodaj roomA do po��cze� roomB, je�li chcesz, aby po��czenia by�y dwukierunkowe
        roomB.connectedRooms.Add(roomA);

        // Tworzenie LineRenderer mi�dzy pokojami
        LineRenderer line = roomA.roomObject.AddComponent<LineRenderer>();
        line.SetPosition(0, roomA.position);
        line.SetPosition(1, roomB.position);
        // Dodatkowe ustawienia linii, np. kolor, grubo�� itp.
    }


    void CreateConnections(List<Room> rooms, int totalLevels)
    {
        for (int level = 1; level < totalLevels; level++)
        {
            var currentLevelRooms = rooms.Where(r => r.level == level).ToList();
            var nextLevelRooms = rooms.Where(r => r.level == level + 1).ToList();

            foreach (var roomA in currentLevelRooms)
            {
                foreach (var roomB in nextLevelRooms)
                {
                    // Wywo�anie ConnectRooms z odpowiednimi argumentami
                    ConnectRooms(roomA, roomB);
                }
            }
        }
    }
}
