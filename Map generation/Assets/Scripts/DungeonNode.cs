using System.Collections.Generic;
using UnityEngine;

public class DungeonNode : MonoBehaviour
{
    public int Level; // Poziom wêz³a
    public List<DungeonNode> Connections; // Lista po³¹czeñ z innymi wêz³ami
    private LineRenderer lineRenderer; // Komponent LineRenderer

    void Awake()
    {
        Connections = new List<DungeonNode>(); // Inicjalizacja listy po³¹czeñ
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // Na pocz¹tku nie ma pozycji
    }

    public void DrawConnections()
    {
        if (lineRenderer != null)
        {
            // Ustaw liczbê pozycji na liczbê po³¹czeñ + 1 dla samego wêz³a
            lineRenderer.positionCount = Connections.Count + 1;

            // Ustaw pozycje dla LineRenderer
            lineRenderer.SetPosition(0, transform.position); // Pozycja wêz³a
            for (int i = 0; i < Connections.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, Connections[i].transform.position); // Pozycje po³¹czeñ
            }
        }
    }

    // Dodajemy metodê do dodawania po³¹czeñ
    public void AddConnection(DungeonNode node)
    {
        if (!Connections.Contains(node))
        {
            Connections.Add(node);
            node.AddConnection(this); // Ustal dwukierunkowe po³¹czenie
        }
    }
}
