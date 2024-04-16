using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private SecondCustomNetworkManager networkManager;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;


    private void OnEnable()
    {
        SecondCustomNetworkManager.OnClientConnected += HandleClientConnected;
        SecondCustomNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        SecondCustomNetworkManager.OnClientConnected -= HandleClientConnected;
        SecondCustomNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;

        networkManager.StartClient();
        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        Debug.Log("I was here");
        Invoke("ResetButton", 2);
    }

    public void ResetButton()
    {
        joinButton.interactable = true;
    }
}