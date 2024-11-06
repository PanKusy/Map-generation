using UnityEngine;

public class RoomData : MonoBehaviour
{
    public RoomType roomType;

    public void SetRoomType(RoomType type)
    {
        roomType = type;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            switch (roomType)
            {
                case RoomType.Battle:
                    renderer.material.color = Color.red;
                    break;
                case RoomType.Treasure:
                    renderer.material.color = Color.yellow;
                    break;
                case RoomType.Trap:
                    renderer.material.color = Color.magenta;
                    break;
                case RoomType.Empty:
                    renderer.material.color = Color.gray;
                    break;
                case RoomType.Puzzle:
                    renderer.material.color = Color.blue;
                    break;
            }
        }
    }
}