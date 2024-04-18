using Mirror;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private PlayerRecord _recordPrefab;
    [SerializeField] private Transform _rootObject;
    
    private readonly SyncDictionary<byte, GamePlayerInfo> _playersInfo = new SyncDictionary<byte, GamePlayerInfo>();
    private Dictionary<byte, PlayerRecord> _playersRecord = new Dictionary<byte, PlayerRecord>();


    public void AddPlayer(byte playerID,GamePlayerInfo gamePlayerInfo)
    {
        _playersInfo.Add(playerID, gamePlayerInfo);
    }

    public void PlayerScore(byte shooterID, byte gateID)
    {
        if (shooterID == gateID) { return; }

        GamePlayerInfo temp = _playersInfo[shooterID];
        temp.score += 1;
        _playersInfo[shooterID] = temp;

        temp = _playersInfo[gateID];
        temp.score = Mathf.Clamp(temp.score - 1, 0, int.MaxValue);
        _playersInfo[gateID] = temp;
    }

    public override void OnStartClient()
    {
        // Equipment is already populated with anything the server set up
        // but we can subscribe to the callback in case it is updated later on
        _playersInfo.Callback += OnPlayersInfoChanged;

        // Process initial SyncDictionary payload
        foreach (KeyValuePair<byte, GamePlayerInfo> kvp in _playersInfo)
            OnPlayersInfoChanged(SyncDictionary<byte, GamePlayerInfo>.Operation.OP_ADD, kvp.Key, kvp.Value);
    }

    void OnPlayersInfoChanged(SyncDictionary<byte, GamePlayerInfo>.Operation op, byte key, GamePlayerInfo value)
    {
        switch (op)
        {
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_ADD:
                AddPlayerRecord(key, value);
                break;
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_SET:
                UpdatePlayerRecord(key, value);
                break;
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_REMOVE:
                // entry removed
                break;
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_CLEAR:
                // Dictionary was cleared
                break;
        }
    }

    private void AddPlayerRecord(byte playerID, GamePlayerInfo gamePlayerInfo)
    {
        PlayerRecord temp = Instantiate(_recordPrefab, _rootObject);
        _playersRecord.Add(playerID, temp);
        temp.Initialize(gamePlayerInfo);
    }

    private void UpdatePlayerRecord(byte playerID, GamePlayerInfo gamePlayerInfo)
    {
        _playersRecord[playerID].UpdateDisplayInfo(gamePlayerInfo);
    }

    private void RemovePlayerRecord()
    {

    }
}