using Mirror;
using Mirror.Examples.MultipleMatch;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Bootstrap : NetworkBehaviour
{
    private List<IDisposable> disposables = new List<IDisposable>();
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private TrajectoryPredictor trajectoryPrefab;
    [SerializeField] private GateCollisionDetection gatePrefab;

    public override void OnStartLocalPlayer()
    {
        IInput input = GetComponent<IInput>();
        ILaunch launcher = GetComponent<ILaunch>();

        TimeCounter timeCounter = new TimeCounter(input);
        disposables.Add(timeCounter);

        TrajectoryPredictorInitialize(timeCounter, launcher.StartPoint);
        PlayerInitialize(input, timeCounter, launcher);
    }

    private void TrajectoryPredictorInitialize(TimeCounter timeCounter, Transform startPoint)
    {
        TrajectoryPredictor trajectoryPredictor = Instantiate(trajectoryPrefab);
        trajectoryPredictor.Initialize(startPoint);
        TrajectoryPredictorMediator mediator = new TrajectoryPredictorMediator(trajectoryPredictor, timeCounter);
        disposables.Add(mediator);
    }

    public void PlayerInitialize(IInput input, TimeCounter timeCounter, ILaunch launcher)
    {
        GunRotator gunRotator = GetComponent<GunRotator>();
        gunRotator.Initialize(playerConfig);
        GetComponent<PlayerController>().Initialize(input, timeCounter, gunRotator, launcher);
    }

    public void ServerInitialize(ScoreManager scoreManager, RoomPlayerInfo playerInfo)
    {
        Vector3 spawnPoint = PlayerSpawnSystem.GetRandomPoint();
        GetComponent<BallLauncher>().Initialize(playerInfo.clientID);
        InitializePlayerGate(spawnPoint, playerInfo.playerColor, playerInfo.clientID, scoreManager);
    }

    private void InitializePlayerGate(Vector3 startPossition, PlayerColor color, byte ownerID, ScoreManager scoreManager)
    {
        GateCollisionDetection gateObject = Instantiate(gatePrefab, startPossition, Quaternion.identity);
        gateObject.Initialize(ownerID);
        gateObject.OnPlayerScored += scoreManager.PlayerScore;
        gateObject.GetComponent<SyncObjectColor>().UpdateColor(color);
        NetworkServer.Spawn(gateObject.gameObject);
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