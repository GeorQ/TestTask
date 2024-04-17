using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class NetworkRoomPlayer : NetworkBehaviour
{
    struct PlayerInfo
    {
        public string playerName;
        public bool readyStatus;
        public Color32 playerColor;
        public byte clientID;
    }

    [Header("UI")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Transform rootObject;
    [SerializeField] private GameObject playerCard;

    private readonly SyncList<PlayerInfo> playersInfo = new SyncList<PlayerInfo>();

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private CustomNetworkManager room;
    private CustomNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        //lobbyUI.SetActive(true);
    }

    public void Start()
    {
        if (!isClient)
        {
            return;
        }

        Room.RoomPlayers.Add(this);

        playersInfo.Callback += OnPlayerUpdate;
        Debug.Log(7);
        // Process initial SyncList payload
        for (int index = 0; index < playersInfo.Count; index++)
            OnPlayerUpdate(SyncList<PlayerInfo>.Operation.OP_ADD, index, new PlayerInfo(), playersInfo[index]);

        UpdateDisplay();
    }

    

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!isOwned)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.isOwned)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        //for (int i = 0; i < playerNameTexts.Length; i++)
        //{
        //    playerNameTexts[i].text = "Waiting For Player...";
        //    playerReadyTexts[i].text = string.Empty;
        //}

        //for (int i = 0; i < Room.RoomPlayers.Count; i++)
        //{
        //    playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
        //    playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
        //        "<color=green>Ready</color>" :
        //        "<color=red>Not Ready</color>";
        //}
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        // Start Game
        Debug.Log("Start Game");
        room.StartGame();
    }

    void OnPlayerUpdate(SyncList<PlayerInfo>.Operation op, int index, PlayerInfo oldItem, PlayerInfo newItem)
    {
        switch (op)
        {
            case SyncList<PlayerInfo>.Operation.OP_ADD:
                Debug.Log("3");
                CreatePlayerCard(newItem);
                // index is where it was added into the list
                // newItem is the new item
                break;
            case SyncList<PlayerInfo>.Operation.OP_INSERT:
                // index is where it was inserted into the list
                // newItem is the new item
                break;
            case SyncList<PlayerInfo>.Operation.OP_REMOVEAT:
                // index is where it was removed from the list
                // oldItem is the item that was removed
                break;
            case SyncList<PlayerInfo>.Operation.OP_SET:
                UpdatePlayerCard();
                // index is of the item that was changed
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
            case SyncList<PlayerInfo>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }
    }

    private void CreatePlayerCard(PlayerInfo playerInfo)
    {
        GameObject card = Instantiate(playerCard, rootObject);
        Debug.Log("1");
        card.GetComponent<PlayerCard>().SetCard(playerInfo.playerName, false, playerInfo.clientID);
    }

    private void UpdatePlayerCard()
    {

    }

    public void AddPlayer(byte id, string name)
    {
        Debug.Log("2");
        playersInfo.Add(new PlayerInfo() { clientID = id, playerName = name, readyStatus = false });
    }
}