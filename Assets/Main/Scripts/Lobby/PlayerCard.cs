using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


public class PlayerCard : MonoBehaviour
{
    [SerializeField] private Color32[] colors;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private Image colorImage;


    public void SetCard(string name, bool readyStatus, int colorID)
    {
        colorImage.color = colors[colorID - 1];
        nameText.text = name;
        readyText.text = readyStatus ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
    }
}