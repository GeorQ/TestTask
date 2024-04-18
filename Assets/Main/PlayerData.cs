using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public byte ID;
    public PlayerColor PlayerColor;


    public void Initialize(byte id, PlayerColor playerColor)
    {
        PlayerColor = playerColor;
        ID = id;
    }
}