using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int maxPointsPerLevel = 4;     // Maksymalna liczba punktów na poziomie
    public int maxConnections = 3;        // Maksymalna liczba po³¹czeñ z punktu
    public int numberOfLevels = 5;        // Liczba poziomów
    public GameObject pointPrefab;        // Prefab punktu do wizualizacji
    [Range(0,1)]
    public float positionVariance = 1.0f; // Zakres losowego przesuniêcia pozycji
    public Material material;

    private List<List<GameObject>> dungeonPoints = new List<List<GameObject>>();   // Lista poziomów i punktów

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Generacja punktu startowego
        GameObject startPoint = Instantiate(pointPrefab, Vector3.zero, Quaternion.identity);
        startPoint.name = "Start Point";
        List<GameObject> firstLevel = new List<GameObject> { startPoint };
        dungeonPoints.Add(firstLevel);

        // Generacja poziomów pomiêdzy startowym a koñcowym
        for (int level = 1; level < numberOfLevels - 1; level++)
        {
            GenerateLevel(level);
        }

        // Generacja punktu koñcowego
        GameObject endPoint = Instantiate(pointPrefab, new Vector3(0, numberOfLevels * 2, 0), Quaternion.identity);
        endPoint.name = "End Point";
        List<GameObject> lastLevel = new List<GameObject> { endPoint };
        dungeonPoints.Add(lastLevel);

        // Tworzenie po³¹czeñ
        CreateConnections();
    }

    void GenerateLevel(int level)
    {
        List<GameObject> levelPoints = new List<GameObject>();
        int pointsInLevel = Random.Range(1, maxPointsPerLevel + 1);

        for (int i = 0; i < pointsInLevel; i++)
        {
            // Generowanie losowej pozycji z pewnym przesuniêciem
            float xOffset = Random.Range(-positionVariance, positionVariance);
            float yOffset = Random.Range(-positionVariance, positionVariance);
            Vector3 position = new Vector3(i * 2 + xOffset, level * 2 + yOffset, 0);

            GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
            point.name = $"Point {level}-{i}";
            levelPoints.Add(point);
        }

        dungeonPoints.Add(levelPoints);
    }

    void CreateConnections()
    {
        for (int level = 0; level < dungeonPoints.Count - 1; level++)
        {
            List<GameObject> currentLevel = dungeonPoints[level];
            List<GameObject> nextLevel = dungeonPoints[level + 1];

            // Gwarantowanie, ¿e ka¿dy punkt ma przynajmniej jedn¹ œcie¿kê wychodz¹c¹
            foreach (GameObject point in currentLevel)
            {
                List<GameObject> connectedPoints = new List<GameObject>();  // Œledzimy, do których punktów ju¿ jest po³¹czenie

                // Tworzymy przynajmniej jedno po³¹czenie, aby ka¿dy punkt mia³ œcie¿kê wychodz¹c¹
                GameObject initialTarget = nextLevel[Random.Range(0, nextLevel.Count)];
                CreateConnection(point, initialTarget);
                connectedPoints.Add(initialTarget);  // Dodajemy punkt docelowy do listy po³¹czonych

                // Dodawanie dodatkowych po³¹czeñ, jeœli jest miejsce do osi¹gniêcia maxConnections
                int additionalConnections = Random.Range(1, maxConnections);
                for (int i = 1; i < additionalConnections; i++)
                {
                    // Sprawdzenie, czy osi¹gnêliœmy limit maxConnections dla tego punktu
                    if (connectedPoints.Count >= maxConnections)
                        break;

                    // Wybieramy losowy punkt z nastêpnego poziomu, unikaj¹c podwójnych po³¹czeñ
                    GameObject target;
                    do
                    {
                        target = nextLevel[Random.Range(0, nextLevel.Count)];
                    } while (connectedPoints.Contains(target));

                    CreateConnection(point, target);
                    connectedPoints.Add(target);
                }
            }

            // Gwarantowanie, ¿e ka¿dy punkt na nastêpnym poziomie ma przynajmniej jedn¹ œcie¿kê docieraj¹c¹
            foreach (GameObject targetPoint in nextLevel)
            {
                bool hasIncomingPath = false;
                foreach (GameObject source in currentLevel)
                {
                    if (IsConnected(source, targetPoint))
                    {
                        hasIncomingPath = true;
                        break;
                    }
                }

                if (!hasIncomingPath)
                {
                    GameObject sourcePoint = currentLevel[Random.Range(0, currentLevel.Count)];
                    CreateConnection(sourcePoint, targetPoint);
                }
            }
        }

        // Po³¹czenie punktów startowego i koñcowego
        List<GameObject> lastLevel = dungeonPoints[dungeonPoints.Count - 2];
        GameObject endPoint = dungeonPoints[dungeonPoints.Count - 1][0];
        foreach (GameObject point in lastLevel)
        {
            CreateConnection(point, endPoint);
        }
    }


    void CreateConnection(GameObject from, GameObject to)
    {
        // Tworzenie linii po³¹czenia
        GameObject connection = new GameObject("Connection");
        connection.transform.position = from.transform.position;
        LineRenderer lineRenderer = connection.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, from.transform.position);
        lineRenderer.SetPosition(1, to.transform.position);
        lineRenderer.material = material;
        lineRenderer.textureMode = LineTextureMode.Tile;
        //lineRenderer.material.mainTextureScale = new Vector2(Vector3.Distance(from.transform.position, to.transform.position), 1);
    }

    bool IsConnected(GameObject from, GameObject to)
    {
        // Sprawdzenie, czy istnieje ju¿ po³¹czenie miêdzy dwoma punktami
        LineRenderer[] lines = from.GetComponentsInChildren<LineRenderer>();
        foreach (LineRenderer line in lines)
        {
            if (line.GetPosition(1) == to.transform.position)
            {
                return true;
            }
        }
        return false;
    }
}
