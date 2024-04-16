using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;
using System.Linq;
using TMPro;


public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;

    public TMP_Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    //private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    private List<GameObject> players = new List<GameObject>();
    public PlayerListItem[] playerListItems;
    public PlayerObjectController localPlayerController;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    //public void UpdatePlayerList()
    //{
    //    Debug.Log($"{players.Count} < {Manager.GamePlayers.Count}");


    //    if (!PlayerItemCreated)
    //    {
    //        Debug.Log("I was here");
    //        CreateHostItem(); //Host
    //    }

    //    if (players.Count < Manager.GamePlayers.Count)
    //    {
    //        Debug.Log("I was here 2");
    //        CreateClientPlayerItem();
    //    }

        //if (playerListItems.Count > Manager.GamePlayers.Count)
        //{
        //    Debug.Log("I was here 3");
        //    RemovePlayerItem();
        //}

        //if (playerListItems.Count == Manager.GamePlayers.Count)
        //{
        //    Debug.Log("I was here 4");
        //    UpdatePlayerItem();
        //}

        //CustomUpdate();
    //}

    public void CreateHostItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (player.lobbySlot == -1)
            {
                continue;
            }

            PlayerListItem newPlayerItemScript = playerListItems[player.lobbySlot];

            newPlayerItemScript.ConnectionID = player.connectionID;
            newPlayerItemScript.PlayerSteamID = player.playerSteamID;
            newPlayerItemScript.SetPlayerValues(player.playerName);

            //newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            //newPlayerItem.transform.localScale = Vector3.one;

            //playerListItems.Add(newPlayerItemScript);
        }

        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!playerListItems.Any(b => b.ConnectionID == player.connectionID))
            {
                GameObject newPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.ConnectionID = player.connectionID;
                newPlayerItemScript.PlayerSteamID = player.playerSteamID;
                newPlayerItemScript.SetPlayerValues(player.playerName);

                newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                //playerListItems.Add(newPlayerItemScript);
                players.Add(newPlayerItemScript.gameObject);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        for (int i = 0; i < playerListItems.Length; i++)
        {
            playerListItems[i].SetPlayerValues(string.Empty);
        }

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            for (int i = 0; i < playerListItems.Length; i++)
            {
                if (i == player.lobbySlot)
                {
                    playerListItems[i].SetPlayerValues(player.playerName);
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerListItem in playerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.connectionID == playerListItem.ConnectionID))
            {
                playerListToRemove.Add(playerListItem);
            }
        }

        if (playerListToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in playerListToRemove)
            {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                //playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void Call(int id)
    {
        localPlayerController.CmdTest(id);
    }
}