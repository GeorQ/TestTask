using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Bootstrap : NetworkBehaviour
{
    private List<IDisposable> disposables = new List<IDisposable>();
    [SerializeField] private PlayerConfig playerConfig;

    public override void OnStartAuthority()
    {
        PlayerInitialize();
    }


    public void PlayerInitialize()
    {
        IInput input = GetComponent<IInput>();
        TrajectoryPredictor trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        GunRotator gunRotator = GetComponent<GunRotator>();
        gunRotator.Initialize(playerConfig);
        TimeCounter timeCounter = new TimeCounter(input);
        TrajectoryPredictorMediator mediator = new TrajectoryPredictorMediator(trajectoryPredictor, timeCounter);
        disposables.Add(mediator);
        disposables.Add(timeCounter);
        GetComponent<PlayerController>().Initialize(input, timeCounter, gunRotator);
    }

    private void OnDestroy()
    {
        foreach (var item in disposables)
        {
            item.Dispose();
        }

        disposables.Clear();
    }
}