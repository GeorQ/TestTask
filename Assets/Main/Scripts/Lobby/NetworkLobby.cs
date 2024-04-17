using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;


public struct PlayerInfo
{
    public string playerName;
    public bool readyStatus;
    public PlayerColor playerColor;
    public byte clientID;
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
    [SerializeField] private GameObject playerCard;
    
    private Dictionary<NetworkConnectionToClient, byte> clientIDs = new Dictionary<NetworkConnectionToClient, byte>();
    private readonly SyncList<PlayerInfo> playersInfo = new SyncList<PlayerInfo>();
    private byte currentId = 0;


    public override void OnStartServer()
    {
        NetworkServer.OnConnectedEvent += OnClientConnected;
        NetworkServer.OnDisconnectedEvent += OnClientDisconect;
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
        foreach (PlayerInfo item in playersInfo)
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
        playersInfo.Add(new PlayerInfo() { clientID = clientIDs[conn], playerName = message.playerName, readyStatus = false });
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
            OnPlayerUpdate(SyncList<PlayerInfo>.Operation.OP_ADD, index, new PlayerInfo(), playersInfo[index]);
        }
    }

    void OnPlayerUpdate(SyncList<PlayerInfo>.Operation op, int index, PlayerInfo oldItem, PlayerInfo newItem)
    {
        switch (op)
        {
            case SyncList<PlayerInfo>.Operation.OP_ADD:
                CreatePlayerCard(newItem);
                break;
            case SyncList<PlayerInfo>.Operation.OP_REMOVEAT:
                RemovePlayerCard(index);
                break;
            case SyncList<PlayerInfo>.Operation.OP_SET:
                UpdatePlayerCard(newItem, index);
                break;
        }
    }

    private void CreatePlayerCard(PlayerInfo playerInfo)
    {
        GameObject card = Instantiate(playerCard, rootObject);
        PlayerCard temp = card.GetComponent<PlayerCard>();
        //PlayerIdentity identity = NetworkClient.localPlayer.GetComponent<PlayerIdentity>();
        //if (playerInfo.clientID == identity.playerID)
        //{
        //    temp.SetButtonActive();
        //}
        temp.onColorChange += ChangeColor;
        temp.SetCard(playerInfo.playerName, false, playerInfo.clientID);
    }

    [Command(requiresAuthority = false)]
    private void ChangePlayerColor(NetworkConnectionToClient conn = null)
    {
        Debug.Log(conn.connectionId);
        int randomColorID = GetRandomColor();
        
        if (randomColorID == -1) { return; }

        PlayerColor newColor = (PlayerColor) randomColorID;

        byte clientID = clientIDs[conn];
        for (int i = 0; i < playersInfo.Count; i++)
        {
            if (playersInfo[i].clientID == clientID)
            {
                PlayerInfo temp = playersInfo[i];
                temp.playerColor = newColor;
                playersInfo[i] = temp;
                break;
            }
        }
    }

    private void RemovePlayerCard(int index)
    {
        Destroy(rootObject.GetChild(index).gameObject);
    }

    private void UpdatePlayerCard(PlayerInfo playerInfo, int index)
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
                PlayerInfo temp = playersInfo[i];
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
        
        foreach (PlayerInfo playerInfo in playersInfo)
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

    public PlayerInfo GetData(NetworkConnectionToClient conn)
    {
        byte clientID = clientIDs[conn];

        foreach (PlayerInfo item in playersInfo)
        {
            if (item.clientID == clientID)
            {
                return item;
            }
        }

        return new PlayerInfo();
    }

    private int GetRandomColor()
    {
        List<int> freeColors = new List<int> { 0, 1, 2, 3 };

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
   
    private void ChangeColor()
    {
        ChangePlayerColor();
    }

    public void NewPlayerInit(NetworkConnectionToClient conn)
    {
        conn.identity.gameObject.GetComponent<PlayerIdentity>().SetPlayerID(clientIDs[conn]);
    }
}