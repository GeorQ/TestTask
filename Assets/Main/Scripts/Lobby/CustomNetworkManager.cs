using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CustomNetworkManager : NetworkManager
{
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public List<NetworkRoomPlayer> RoomPlayers { get; } = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();

    [SerializeField] private int minPlayers = 1;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab;
    [SerializeField] private GameObject playerSpawnSystem;

    private const string MainSceneName = "SceneLobby";
    private const string GameSceneName = "MainGameScene";


    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect( )
    {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        if (SceneManager.GetActiveScene().name != MainSceneName)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == MainSceneName)
        {
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }
        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }
        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == MainSceneName)
        {
            if (!IsReadyToStart()) { return; }
            ServerChangeScene("MainGameScene");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().name == MainSceneName && newSceneName == GameSceneName)
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(GameSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == GameSceneName)
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}