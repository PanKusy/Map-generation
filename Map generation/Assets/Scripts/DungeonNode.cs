using System.Collections.Generic;
using UnityEngine;

public class DungeonNode : MonoBehaviour
{
    public int Level; // Poziom w�z�a
    public List<DungeonNode> Connections; // Lista po��cze� z innymi w�z�ami
    private LineRenderer lineRenderer; // Komponent LineRenderer

    void Awake()
    {
        Connections = new List<DungeonNode>(); // Inicjalizacja listy po��cze�
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // Na pocz�tku nie ma pozycji
    }

    public void DrawConnections()
    {
        if (lineRenderer != null)
        {
            // Ustaw liczb� pozycji na liczb� po��cze� + 1 dla samego w�z�a
            lineRenderer.positionCount = Connections.Count + 1;

            // Ustaw pozycje dla LineRenderer
            lineRenderer.SetPosition(0, transform.position); // Pozycja w�z�a
            for (int i = 0; i < Connections.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, Connections[i].transform.position); // Pozycje po��cze�
            }
        }
    }

    // Dodajemy metod� do dodawania po��cze�
    public void AddConnection(DungeonNode node)
    {
        if (!Connections.Contains(node))
        {
            Connections.Add(node);
            node.AddConnection(this); // Ustal dwukierunkowe po��czenie
        }
    }
}
