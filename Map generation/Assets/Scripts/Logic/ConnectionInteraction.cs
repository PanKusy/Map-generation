using UnityEngine;

public class ConnectionInteraction : MonoBehaviour
{
    public GameObject originPoint;           
    public GameObject targetNode;            
    private DungeonGenerator dungeonGenerator;
    private bool isSelectable = false;       
    private LineRenderer lineRenderer;       

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>(); 
    }

    private void OnMouseEnter()
    {
        if (isSelectable)
        {
            GetComponent<LineRenderer>().material.color = Color.green;
        }
    }

    private void OnMouseExit()
    {
        GetComponent<LineRenderer>().material.color = Color.white;
    }

    public void SetDungeonGenerator(DungeonGenerator generator)
    {
        dungeonGenerator = generator;
    }

    public void SetIsSelectable(bool selectable)
    {
        isSelectable = selectable;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = selectable;
        }
    }

    private void OnMouseDown()
    {
        if (isSelectable && dungeonGenerator != null && targetNode != null)
        {
            dungeonGenerator.MoveCameraToPoint(targetNode);
        }
    }
}
