using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController gamePlayerInstance = Instantiate(GamePlayerPrefab);
            gamePlayerInstance.connectionID = conn.connectionId;
            gamePlayerInstance.playerIDNumber = GamePlayers.Count + 1;
            gamePlayerInstance.playerSteamID = (ulong) SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Debug.Log("Server has started");
    }
}