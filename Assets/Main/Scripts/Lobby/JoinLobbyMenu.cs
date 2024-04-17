using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private CustomNetworkManager networkManager;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Button joinButton;


    private void OnEnable()
    {
        CustomNetworkManager.OnClientConnected += HandleClientConnected;
        CustomNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        CustomNetworkManager.OnClientConnected -= HandleClientConnected;
        CustomNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
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