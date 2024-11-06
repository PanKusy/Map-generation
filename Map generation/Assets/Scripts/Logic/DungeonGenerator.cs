using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int maxPointsPerLevel = 4;     
    public int maxConnections = 3;        
    public int numberOfLevels = 5;        
    public GameObject pointPrefab;        
    [Range(0,1)]
    public float positionVariance = 1.0f; 
    public Material material;

    private List<List<GameObject>> dungeonPoints = new List<List<GameObject>>();   // Lista poziomów i punktów

    private Dictionary<GameObject, List<GameObject>> connections = new Dictionary<GameObject, List<GameObject>>();

    [Header("Camera Settings")]
    public Camera mainCamera;               
    public float cameraMoveSpeed = 2.0f;
    [SerializeField] private GameObject currentPoint;
    private GameObject targetPoint;
    private bool isMoving = false;

    private List<GameObject> activeConnections = new List<GameObject>(); // Aktywne po³¹czenia z currentPoint

    void Start()
    {
        GenerateDungeon();

        currentPoint = dungeonPoints[0][0]; 
        mainCamera.transform.position = new Vector3(currentPoint.transform.position.x, currentPoint.transform.position.y, -3);

        ActivateConnectionsFromCurrentPoint();
    }

    void GenerateDungeon()
    {
        GameObject startPoint = Instantiate(pointPrefab, Vector3.zero, Quaternion.identity);
        startPoint.name = "Start Point";
        List<GameObject> firstLevel = new List<GameObject> { startPoint };
        dungeonPoints.Add(firstLevel);

        for (int level = 1; level < numberOfLevels - 1; level++)
        {
            GenerateLevel(level);
        }

        GameObject endPoint = Instantiate(pointPrefab, new Vector3(0, numberOfLevels * 2, 0), Quaternion.identity);
        endPoint.name = "End Point";
        List<GameObject> lastLevel = new List<GameObject> { endPoint };
        dungeonPoints.Add(lastLevel);

        CreateConnections();
    }

    void GenerateLevel(int level)
    {
        List<GameObject> levelPoints = new List<GameObject>();
        int pointsInLevel = Random.Range(1, maxPointsPerLevel + 1);

        for (int i = 0; i < pointsInLevel; i++)
        {
            float xOffset = Random.Range(-positionVariance, positionVariance);
            float yOffset = Random.Range(-positionVariance, positionVariance);
            Vector3 position = new Vector3(i * 2 + xOffset, level * 2 + yOffset, 0);

            GameObject point = Instantiate(pointPrefab, position, Quaternion.identity);
            point.name = $"Point {level}-{i}";

            RoomType roomType = GetRandomRoomType();
            point.GetComponent<RoomData>().SetRoomType(roomType);

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

            foreach (GameObject point in currentLevel)
            {
                List<GameObject> connectedPoints = new List<GameObject>();

                GameObject initialTarget = nextLevel[Random.Range(0, nextLevel.Count)];
                CreateConnection(point, initialTarget);
                connectedPoints.Add(initialTarget);

                int attempts = 0;
                while (connectedPoints.Count < maxConnections && attempts < nextLevel.Count * 2)
                {
                    GameObject target = nextLevel[Random.Range(0, nextLevel.Count)];
                    if (!connectedPoints.Contains(target))
                    {
                        CreateConnection(point, target);
                        connectedPoints.Add(target);
                    }
                    attempts++;
                }
            }


            //sprawdza, czy do punktów dociera przynajmniej jedna droga
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

        if (!connections.ContainsKey(from))
            connections[from] = new List<GameObject>();

        connections[from].Add(to);

        BoxCollider boxCollider = connection.AddComponent<BoxCollider>();
        Vector3 direction = to.transform.position - from.transform.position;
        boxCollider.size = new Vector3(0.1f, 0.1f, direction.magnitude);  
        boxCollider.transform.position = from.transform.position + direction / 2;  
        boxCollider.transform.rotation = Quaternion.LookRotation(direction);  
        boxCollider.center = new Vector3(0, 0, 0);

        ConnectionInteraction interaction = connection.AddComponent<ConnectionInteraction>();
        interaction.originPoint = from;
        interaction.targetNode = to;
        interaction.SetDungeonGenerator(this);
    }

    bool IsConnected(GameObject from, GameObject to)
    {
        return connections.ContainsKey(from) && connections[from].Contains(to);
    }

    void ClearDungeon()
    {
        foreach (var level in dungeonPoints)
        {
            foreach (var point in level)
            {
                Destroy(point);
            }
        }
        dungeonPoints.Clear();
        connections.Clear();
    }

    RoomType GetRandomRoomType()
    {
        // s³ownik z rodzajami pokoi i ich wagami
        Dictionary<RoomType, int> roomWeights = new Dictionary<RoomType, int>
    {
        { RoomType.Battle, 2 },
        { RoomType.Treasure, 1 },
        //{ RoomType.Trap, 1 },
        { RoomType.Empty, 1 },
        //{ RoomType.Puzzle, 1 }
    };

        List<RoomType> weightedRoomList = new List<RoomType>();
        foreach (var room in roomWeights)
        {
            for (int i = 0; i < room.Value; i++)
            {
                weightedRoomList.Add(room.Key);
            }
        }
        return weightedRoomList[Random.Range(0, weightedRoomList.Count)];
    }

    public void MoveCameraToPoint(GameObject selectedPoint)
    {
        if (!isMoving || targetPoint != selectedPoint)
        {
            targetPoint = selectedPoint;
            isMoving = true;
            StartCoroutine(MoveCamera());
        }
    }

    private IEnumerator MoveCamera()
    {
        while (isMoving && targetPoint != null)
        {
            //mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(targetPoint.transform.position.x, targetPoint.transform.position.y, -3), cameraMoveSpeed * Time.deltaTime);
            Vector3 targetPosition = new Vector3(targetPoint.transform.position.x, targetPoint.transform.position.y, -3);
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, targetPosition, cameraMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.1f)
            {
                SetCameraPosition(targetPosition);
                currentPoint = targetPoint;
                targetPoint = null;
                isMoving = false;

                ActivateConnectionsFromCurrentPoint();
            }
            yield return null;
        }
    }
    private void SetCameraPosition(Vector3 position)
    {
        mainCamera.transform.position = new Vector3(position.x, position.y, -3);
    }

    private void ActivateConnectionsFromCurrentPoint()
    {
        activeConnections.Clear();
        ConnectionInteraction[] connections = FindObjectsOfType<ConnectionInteraction>(); //pamiêtaæ, ¿eby to poprawiæ 

        foreach (var connection in connections)
        {
            if (connection.originPoint == currentPoint)
            {
                connection.SetIsSelectable(true);
                activeConnections.Add(connection.gameObject);
            }
            else
            {
                connection.SetIsSelectable(false);
            }
        }
    }
}