using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SecondCustomNetworkManager networkManager;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();
        Debug.Log(networkManager.networkAddress);
        landingPagePanel.SetActive(false);
    }
}