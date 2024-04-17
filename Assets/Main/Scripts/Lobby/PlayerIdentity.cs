using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentity : NetworkBehaviour
{
    [SyncVar] public byte playerID;


    public void SetPlayerID(byte id)
    {
        playerID = id;
    }
}