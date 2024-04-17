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

    [SerializeField] private int minPlayers = 1;

    [Header("Room")]
    [SerializeField] private NetworkLobby networkLobby;

    [Header("Game")]
    [SerializeField] private GameObject gamePlayerPrefab;
    [SerializeField] private GameObject playerSpawnSystem;

    private const string MainSceneName = "SceneLobby";
    private const string GameSceneName = "MainGameScene";


    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<PlayerNameMessage>(OnNameReceived);
    }

    private void OnNameReceived(NetworkConnectionToClient conn, PlayerNameMessage playerNameMessage)
    {
        networkLobby.AddPlayer(conn, playerNameMessage);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
        PlayerNameMessage nameMessage = new PlayerNameMessage() { playerName = PlayerNameInput.DisplayName };
        NetworkClient.Send(nameMessage);
        NetworkClient.AddPlayer();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        networkLobby.NewPlayerInit(conn);
    }

    public override void OnClientDisconnect()
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

    public void StartGame()
    {
        ServerChangeScene(GameSceneName);
    }

    private void SpawnPlayer(NetworkConnectionToClient conn)
    {
        NetworkServer.DestroyPlayerForConnection(conn);

        Vector3 startPosition = GetStartPosition().position;

        var gameplayerInstance = Instantiate(gamePlayerPrefab, startPosition, Quaternion.identity);

        PlayerInfo playerInfo = networkLobby.GetData(conn);

        //gameplayerInstance.GetComponent<PlayerData>().Initialize(playerInfo.playerColor, playerInfo.clientID);

        NetworkServer.AddPlayerForConnection(conn, gameplayerInstance.gameObject);
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (SceneManager.GetActiveScene().name == GameSceneName)
        {
            SpawnPlayer(conn);
        }
    }
}