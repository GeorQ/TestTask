using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlayerCard : MonoBehaviour
{
    [SerializeField] private Color32[] colors;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private Image colorImage;
    private int currentColorID = 0;
    

    public void SetCard(string name, bool readyStatus, int colorID)
    {
        colorImage.color = colors[colorID];
        nameText.text = name;
        readyText.text = readyStatus ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
    }

    public void NextColor()
    {
        if (currentColorID >= 3)
        {
            return;
        }

        colorImage.color = colors[++currentColorID];
    }

    public void PreviousColor()
    {
        if (currentColorID <= 0)
        {
            return;
        }

        colorImage.color = colors[--currentColorID];
    }
}