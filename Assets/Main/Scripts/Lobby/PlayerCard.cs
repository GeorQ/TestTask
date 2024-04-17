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
    [SerializeField] private GameObject nextColorButton;
    private int currentColorID = 0;

    public event Action onColorChange;


    private void Start()
    {
        nextColorButton.SetActive(true);
    }

    public void SetCard(string name, bool readyStatus, int colorID)
    {
        colorImage.color = colors[colorID];
        nameText.text = name;
        readyText.text = readyStatus ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
    }

    public void ChangeColor()
    {
        onColorChange?.Invoke();
    }
    
    public void SetButtonActive()
    {
        nextColorButton.SetActive(true);
    }
}