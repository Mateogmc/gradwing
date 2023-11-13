using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class KGNetworkManager : NetworkManager
{
    public static KGNetworkManager instance;
    public static List<string> localAddresses = new List<string>();
    [SerializeField] GameObject gameStateManager;
    

    public override void Start()
    {
        base.Start();

        instance = this;

        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<KGNetworkDiscovery>();
        }

        localAddresses.Clear();
        IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress address in iPHostEntry.AddressList)
        {
            if (Regex.IsMatch(address.ToString(), "^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$") && address.ToString().Split('.')[2] != "0")
            {
                localAddresses.Add(address.ToString());
                Debug.Log(address.ToString());
            }
        }
    }

    public KGNetworkDiscovery networkDiscovery;

    public List<NetworkConnectionToClient> players = new List<NetworkConnectionToClient>();
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        CheckPlayerCount(conn);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        //GameStateManager.GetInstance().UpdateState();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        players.Add(conn);
        GameStateManager.GameState state = GameStateManager.GetInstance().gameState;
        NetworkServer.Destroy(GameStateManager.GetInstance().gameObject);
        NetworkServer.Spawn(Instantiate(gameStateManager));
        GameStateManager.GetInstance().gameState = state;

        CheckPlayerCount(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        players.Remove(conn);
        base.OnServerDisconnect(conn);

        CheckPlayerCount(conn);
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }

    private void CheckPlayerCount(NetworkConnectionToClient conn)
    {
        if (GameStateManager.GetInstance() != null)
        {
            GameStateManager.GetInstance().CmdCheckPlayerCount(conn);
        }

        StartCoroutine(ServerDataManager.UpdateServer(DataManager.userID, players.Count));

        LobbyManager.GetInstance().LobbyReady(players.Count, true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        try
        {
            FindObjectOfType<ServerListManager>().ResetList();
            FindObjectOfType<ServerListManager>().Hide();
        }
        catch { }
        PostServers();
        networkDiscovery.AdvertiseServer();
        NetworkServer.Spawn(Instantiate(gameStateManager));
    }


    public void FindServers()
    {
        FindObjectOfType<ServerListManager>().ResetList();

        networkDiscovery.StartDiscovery();

        StartCoroutine(ServerDataManager.GetServers());
    }

    void PostServers()
    {
        string strHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);

        List<string> ipList = new List<string>();
        foreach(IPAddress ipAddress in ipHostEntry.AddressList)
        {
            if (Regex.IsMatch(ipAddress.ToString(), "^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$") && ipAddress.ToString().Split('.')[2] != "0")
            {
                ipList.Add(ipAddress.ToString());
            }
        }
        StartCoroutine(ServerDataManager.PostServers(DataManager.userID, ipList));
    }
}
