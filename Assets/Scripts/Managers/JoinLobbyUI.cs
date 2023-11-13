using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class JoinLobbyUI : MonoBehaviour
{
    [SerializeField] private KGNetworkManager networkManager;
    [SerializeField] private Button host;
    [SerializeField] private Button directConnection;
    [SerializeField] private Button connect;
    [SerializeField] private Button back;
    [SerializeField] private Button search;
    [SerializeField] private Button exit;
    [SerializeField] private Button logout;
    [SerializeField] public TextMeshProUGUI connectionDisplay;
    [SerializeField] private GameObject directConnectionScreen;
    [SerializeField] private TextMeshProUGUI loggedInAs;

    private void Start()
    {
        host.onClick.AddListener(Host);
        directConnection.onClick.AddListener(DirectConnection);
        connect.onClick.AddListener(ConnectToIP);
        back.onClick.AddListener(Back);
        search.onClick.AddListener(Search);
        exit.onClick.AddListener(Exit);
        logout.onClick.AddListener(Logout);

        host.Select();

        connectionDisplay = DataManager.GetInstance().connectionDisplay;
        loggedInAs.text = "Logged in as " + DataManager.username;
    }

    private void Host()
    {
        networkManager.StartHost();
        connectionDisplay.text = "Hosting";
    }

    private void DirectConnection()
    {
        directConnectionScreen.SetActive(true);
    }

    public void ConnectToIP()
    {
        networkManager.networkAddress = DataManager.ipAddress;
        networkManager.StartClient();
        if (NetworkClient.active)
        {
            connectionDisplay.text = "Connected on " + DataManager.ipAddress;
        }
    }

    public void ServerNameEdit(string name)
    {
        DataManager.serverName = name;
    }

    private void Back()
    {
        directConnectionScreen.SetActive(false);
    }

    public void Search()
    {
        networkManager.FindServers();
    }

    private void Logout()
    {
        DataManager.GetInstance().Logout();
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
