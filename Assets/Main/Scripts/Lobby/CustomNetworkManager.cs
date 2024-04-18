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

    //[SerializeField] private int minPlayers = 1;

    [Header("Room")]
    [SerializeField] private NetworkLobby networkLobby;

    [Header("Game")]
    [SerializeField] private Bootstrap gamePlayerPrefab;
    [SerializeField] private ScoreManager scoreManagerPrefab;

    private const string MainSceneName = "SceneLobby";
    private const string GameSceneName = "MainGameScene";

    public ScoreManager scoreManager;


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

        Transform startTransform = GetStartPosition();

        RoomPlayerInfo roomPlayerInfo = networkLobby.GetData(conn);

        startTransform.GetComponent<SyncObjectColor>().UpdateColor(roomPlayerInfo.playerColor);

        Bootstrap gameplayerInstance = Instantiate(gamePlayerPrefab, startTransform.position, Quaternion.identity);

        gameplayerInstance.ServerInitialize(scoreManager, roomPlayerInfo);

        NetworkServer.AddPlayerForConnection(conn, gameplayerInstance.gameObject);
        
        scoreManager.AddPlayer(roomPlayerInfo.clientID, new GamePlayerInfo { playerColor = roomPlayerInfo.playerColor, playerName = roomPlayerInfo.playerName, score = 0 });
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        scoreManager = Instantiate(scoreManagerPrefab);
        NetworkServer.Spawn(scoreManager.gameObject);
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