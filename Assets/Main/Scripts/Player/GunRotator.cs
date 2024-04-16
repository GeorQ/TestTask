using Mirror.Examples.BilliardsPredicted;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotator : MonoBehaviour 
{
    [SerializeField] private Transform transformGun;
    [SerializeField] private Transform transformBase;

    private PlayerConfig playerConfig;
    private float initialRotationZ = 0;
    private float initialRotationX = 0;


    public void Initialize(PlayerConfig config)
    {
        playerConfig = config;
    }

    private void CalculateRotation(Vector3 mouseDelta)
    {
        initialRotationZ += mouseDelta.y * playerConfig.RotationSpeed;
        initialRotationX = Mathf.Clamp(initialRotationX + mouseDelta.z * playerConfig.RotationSpeed, playerConfig.minX, playerConfig.maxX);
    }

    public void Rotate(Vector3 mouseDelta)
    {
        CalculateRotation(mouseDelta);
        transformGun.localRotation = Quaternion.Euler(initialRotationX, 0, 0);
        transformBase.rotation = Quaternion.Euler(-90, 0, initialRotationZ);
    }
}
