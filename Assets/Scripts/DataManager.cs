using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;


public class DataManager : MonoBehaviour
{
    public static string username;
    public static string ipAddress;

    [SerializeField] private Button host;
    [SerializeField] private Button connect;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI connectionDisplay;


    void Start()
    {
        DontDestroyOnLoad(this);
        host.onClick.AddListener(Host);
        connect.onClick.AddListener(Connect);
    }

    private void Update()
    {
        if (NetworkServer.active && NetworkClient.active)
        {
            connectionDisplay.text = "Hosting";
        } else if (NetworkClient.active)
        {
            connectionDisplay.text = "Connected on " + ipAddress;
        } else
        {
            connectionDisplay.text = "Not connected";
        }
    }

    private void Host()
    {
        Debug.Log(username);
        networkManager.StartHost();
    }

    private void Connect()
    {
        networkManager.StartClient();
        networkManager.networkAddress = ipAddress;
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
