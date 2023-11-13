using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class DirectConnectionUI : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Button host;
    [SerializeField] private Button connect;
    [SerializeField] private Button exit;
    [SerializeField] public TextMeshProUGUI connectionDisplay;

    private void Start()
    {
        host.onClick.AddListener(Host);
        connect.onClick.AddListener(Connect);
        exit.onClick.AddListener(Exit);
    }

    private void Host()
    {
        networkManager.StartHost();
        connectionDisplay.text = "Hosting";
    }

    private void Connect()
    {
        networkManager.StartClient();
        networkManager.networkAddress = DataManager.ipAddress;
        if (NetworkClient.active)
        {
            connectionDisplay.text = "Connected on " + DataManager.ipAddress;
        }
    }

    private void Exit()
    {
        Application.Quit();
    }

    public void UsernameEdit(string username)
    {
        DataManager.username = username;
    }

    public void IpAddressEdit(string ipAddress)
    {
        DataManager.ipAddress = ipAddress;
    }
}
