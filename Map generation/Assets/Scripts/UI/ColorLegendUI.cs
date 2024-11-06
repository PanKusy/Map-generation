using UnityEngine;
using UnityEngine.UI;

public class ColorLegendUI : MonoBehaviour
{
    public Text battleText;
    public Text treasureText;
    public Text emptyText;

    void Start()
    {
        //doda� fakcztyczne kolory zamiast ich opisu
        battleText.text = "Walka - Czerwony";
        treasureText.text = "Skarb - ��ty";
        emptyText.text = "Puste - Szary";
    }
}
