using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIDNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook = nameof(PlayerLobbySlotUpdate))] public int lobbySlot = -1;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;


    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        //LobbyController.instance.UpdatePlayerList();
    }

    //public override void OnStopClient()
    //{
    //    Manager.GamePlayers.Remove(this);
    //    LobbyController.instance.UpdatePlayerList();
    //}

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.playerName, playerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer)
        {
            this.playerName = NewValue;
        }
        else
        {
            //LobbyController.instance.UpdatePlayerList();
        }
    }

    [Command]
    public void CmdTest(int id)
    {
        this.PlayerLobbySlotUpdate(this.lobbySlot, id);
    }

    [ClientRpc]
    public void PlayerLobbySlotUpdate(int oldValue, int newValue)
    {
        if (isServer)
        {
            this.lobbySlot = newValue;
        }
        else
        {
            LobbyController.instance.UpdatePlayerItem();
        }
    }

}