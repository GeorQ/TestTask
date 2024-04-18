using Mirror;
using UnityEngine;

public class SyncObjectColor : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorUpdate))] private PlayerColor objectColor;

    [SerializeField] private MeshRenderer _mRenderer;


    public void UpdateColor(PlayerColor color)
    {
        objectColor = color;
    }

    private void OnColorUpdate(PlayerColor oldValue, PlayerColor newValue)
    {
        _mRenderer.material = GetPlatformMaterial(newValue);
    }

    private Material GetPlatformMaterial(PlayerColor playerColor)
    {
        switch (playerColor)
        {
            case PlayerColor.Green:
                return Resources.Load<Material>("PlayerMaterials/GreenMaterial");
            case PlayerColor.Blue:
                return Resources.Load<Material>("PlayerMaterials/BlueMaterial");
            case PlayerColor.Yellow:
                return Resources.Load<Material>("PlayerMaterials/YellowMaterial");
            case PlayerColor.Red:
                return Resources.Load<Material>("PlayerMaterials/RedMaterial");
            default:
                return null;
        }
    }
}