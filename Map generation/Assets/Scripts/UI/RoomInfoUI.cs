using UnityEngine;
using UnityEngine.UI;

public class RoomInfoUI : MonoBehaviour
{
    public GameObject roomInfoPanel;
    public Text roomInfoText;

    void Start()
    {
        roomInfoPanel.SetActive(false); 
    }

    public void ShowRoomInfo(string info)
    {
        roomInfoText.text = info;
        roomInfoPanel.SetActive(true);
    }

    public void HideRoomInfo()
    {
        roomInfoPanel.SetActive(false);
    }
}
