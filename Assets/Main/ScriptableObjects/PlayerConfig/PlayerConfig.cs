using UnityEngine;


[CreateAssetMenu(fileName = "NewConfig", menuName = "PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [field:SerializeField] public float RotationSpeed { get; private set; } 
    public float minX = -30;
    public float maxX = 55;
}