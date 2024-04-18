using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public struct RoomPlayerInfo
{
    public string playerName;
    public bool readyStatus;
    public PlayerColor playerColor;
    public byte clientID;
}

public struct GamePlayerInfo
{
    public PlayerColor playerColor;
    public string playerName;
    public int score;
}

public struct PlayerNameMessage : NetworkMessage
{
    public string playerName;
}

public class NetworkLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Transform rootObject;
    [SerializeField] private PlayerCard playerCard;
    [SerializeField] private TMP_Text _hostNameText;
    
    private Dictionary<NetworkConnectionToClient, byte> clientIDs = new Dictionary<NetworkConnectionToClient, byte>();
    private readonly SyncList<RoomPlayerInfo> playersInfo = new SyncList<RoomPlayerInfo>();
    private byte currentId = 0;

    [SyncVar(hook = nameof(UpdateHostName))] private string hostName;


    private void UpdateHostName(string oldValue, string newValue)
    {
        _hostNameText.text = $"{newValue}'s Lobby";
    }

    public override void OnStartServer()
    {
        NetworkServer.OnConnectedEvent += OnClientConnected;
        NetworkServer.OnDisconnectedEvent += OnClientDisconect;

        hostName = PlayerNameInput.DisplayName;
    }

    public override void OnStopServer()
    {
        NetworkServer.OnConnectedEvent -= OnClientConnected;
        NetworkServer.OnDisconnectedEvent -= OnClientDisconect;
    }

    private void OnClientConnected(NetworkConnectionToClient conn)
    {
        clientIDs.Add(conn, currentId++);
    }

    private void OnClientDisconect(NetworkConnectionToClient conn)
    {
        byte clientID = clientIDs[conn];
        foreach (RoomPlayerInfo item in playersInfo)
        {
            if (item.clientID == clientID) 
            { 
                playersInfo.Remove(item); 
                break; 
            }
        }
        clientIDs.Remove(conn);
    }

    public void AddPlayer(NetworkConnectionToClient conn, PlayerNameMessage message)
    {
        playersInfo.Add(new RoomPlayerInfo() { clientID = clientIDs[conn], playerName = message.playerName, readyStatus = false, playerColor = (PlayerColor) GetRandomColor() });
    }

    public void Start()
    {
        if (isServer && isClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
        if (!isClient) { return; }
        playersInfo.Callback += OnPlayerUpdate;
        for (int index = 0; index < playersInfo.Count; index++)
        {
            OnPlayerUpdate(SyncList<RoomPlayerInfo>.Operation.OP_ADD, index, new RoomPlayerInfo(), playersInfo[index]);
        }
    }

    void OnPlayerUpdate(SyncList<RoomPlayerInfo>.Operation op, int index, RoomPlayerInfo oldItem, RoomPlayerInfo newItem)
    {
        switch (op)
        {
            case SyncList<RoomPlayerInfo>.Operation.OP_ADD:
                CreatePlayerCard(newItem);
                break;
            case SyncList<RoomPlayerInfo>.Operation.OP_REMOVEAT:
                RemovePlayerCard(index);
                break;
            case SyncList<RoomPlayerInfo>.Operation.OP_SET:
                UpdatePlayerCard(newItem, index);
                break;
        }
    }

    private void CreatePlayerCard(RoomPlayerInfo playerInfo)
    {
        PlayerCard card = Instantiate(playerCard, rootObject);
        card.SetCard(playerInfo.playerName, playerInfo.readyStatus, (byte) playerInfo.playerColor);
    }

    private void RemovePlayerCard(int index)
    {
        Destroy(rootObject.GetChild(index).gameObject);
    }

    private void UpdatePlayerCard(RoomPlayerInfo playerInfo, int index)
    {
        rootObject.GetChild(index).GetComponent<PlayerCard>().SetCard(playerInfo.playerName, playerInfo.readyStatus, (byte) playerInfo.playerColor);
        //UpdateClientsColors();
    }

    [Command(requiresAuthority = false)]
    public void CmdSetReadyStatus(NetworkConnectionToClient conn = null)
    {
        byte clientID = clientIDs[conn];
        for (int i = 0; i < playersInfo.Count; i++)
        {
            if(playersInfo[i].clientID == clientID)
            {
                RoomPlayerInfo temp = playersInfo[i];
                temp.readyStatus = !temp.readyStatus;
                playersInfo[i] = temp;
                break;
            }
        }

        CheckStartGame();
    }

    private void CheckStartGame()
    {
        bool isInteractable = true;
        
        foreach (RoomPlayerInfo playerInfo in playersInfo)
        {
            if (!playerInfo.readyStatus)
            {
                isInteractable = false;
                break;
            }
        }

        startGameButton.interactable = isInteractable;
    }

    public void UpdateReadyStatus()
    {
        CmdSetReadyStatus();
    }

    public void StartGame()
    {
        var manager = NetworkManager.singleton as CustomNetworkManager;
        manager.StartGame();
    }

    public RoomPlayerInfo GetData(NetworkConnectionToClient conn)
    {
        byte clientID = clientIDs[conn];

        foreach (RoomPlayerInfo item in playersInfo)
        {
            if (item.clientID == clientID)
            {
                return item;
            }
        }

        return new RoomPlayerInfo();
    }

    private int GetRandomColor()
    {
        List<int> freeColors = new List<int> { 1, 2, 3, 4 };

        for (int i = 0; i < playersInfo.Count; i++)
        {
            freeColors.Remove((byte)playersInfo[i].playerColor);
        }
        
        if (freeColors.Count <= 0)
        {
            return -1;
        }

        return freeColors[Random.Range(0, freeColors.Count)];
    }

    [Command(requiresAuthority = false)]
    private void CmdChangePlayerColor(NetworkConnectionToClient conn = null)
    {
        int randomColorID = GetRandomColor();

        if (randomColorID == -1) { return; }

        PlayerColor newColor = (PlayerColor)randomColorID;

        byte clientID = clientIDs[conn];
        for (int i = 0; i < playersInfo.Count; i++)
        {
            if (playersInfo[i].clientID == clientID)
            {
                RoomPlayerInfo temp = playersInfo[i];
                temp.playerColor = newColor;
                playersInfo[i] = temp;
                break;
            }
        }
    }

    public void ChangeColor()
    {
        CmdChangePlayerColor();
    }
}