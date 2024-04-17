using Mirror;
using System.Drawing;
using UnityEngine;


public class PlayerData : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerColorChanged))] private Color32 playerColor;
    [SyncVar(hook = nameof(OnPlayerScoreChanged))] private int score;
    public byte id;


    public void Initialize(Color32 color, byte id)
    {
        playerColor = color;
        this.id = id;
    }

    private void OnPlayerColorChanged(Color32 oldValue, Color32 newValue)
    {

    }

    private void OnPlayerScoreChanged(int oldValue, int newValue)
    {

    }
}