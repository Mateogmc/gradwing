using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GWNetworkManager : NetworkManager
{
    public List<NetworkConnectionToClient> players = new List<NetworkConnectionToClient>();
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        CheckPlayerCount();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        players.Add(conn);

        CheckPlayerCount();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        players.Remove(conn);
        base.OnServerDisconnect(conn);

        CheckPlayerCount();
    }

    private void CheckPlayerCount()
    {
        LobbyManager.GetInstance().LobbyReady(players.Count);
    }
}
