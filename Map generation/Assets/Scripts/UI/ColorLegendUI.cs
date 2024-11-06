using UnityEngine;
using UnityEngine.UI;

public class ColorLegendUI : MonoBehaviour
{
    public Text battleText;
    public Text treasureText;
    public Text emptyText;

    void Start()
    {
        //dodaæ fakcztyczne kolory zamiast ich opisu
        battleText.text = "Walka - Czerwony";
        treasureText.text = "Skarb - ¯ó³ty";
        emptyText.text = "Puste - Szary";
    }
}
