using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListManager : MonoBehaviour
{
    [SerializeField] GameObject serverList;
    [SerializeField] GameObject content;
    public struct Server
    {
        public string ipAddress;
        public DiscoveryResponse discoveryResponse;

        public Server(string ip, DiscoveryResponse response)
        {
            ipAddress = ip;
            discoveryResponse = response;
        }
    }
    List<Server> discoveredServers = new List<Server>();
    [SerializeField] GameObject serverObject;

    public void DisplayResponse(DiscoveryResponse response, string ipAddress)
    {
        Server s = new Server(ipAddress, response);
        discoveredServers.Add(s);
        GameObject ins = Instantiate(serverObject);
        ins.transform.SetParent(content.transform, false);
        ins.GetComponent<ServerDisplay>().Display(discoveredServers.FindIndex(x=>x.ipAddress == s.ipAddress), response.hostName, ipAddress, response.playerCount);
    }

    public void GetResponses(string response)
    {
        string[] servers = response.Split('_');

        foreach (string server in servers)
        {
            Debug.Log(server);
            string[] values = server.Split('|');

            bool yes = false;
            foreach (string address in KGNetworkManager.localAddresses)
            {
                for (int i = 0; i < values[2].Split('.').Length; i++)
                {
                    if (values[2].Split('.')[i] == address.Split('.')[2])
                    {
                        yes = true;
                    }
                }
            }
            if (yes) 
            {
                DiscoveryResponse r = new DiscoveryResponse();
                r.hostName = values[0];
                r.playerCount = int.Parse(values[1]);
                Server s = new Server(values[2], r);
                discoveredServers.Add(s);

                GameObject ins = Instantiate(serverObject);
                ins.transform.SetParent(content.transform, false);
                ins.GetComponent<ServerDisplay>().Display(discoveredServers.FindIndex(x=>x.ipAddress == s.ipAddress), r.hostName, values[2], r.playerCount);
            }
        }
    }

    public void ResetList()
    {
        serverList.SetActive(true);
        discoveredServers.Clear();
        try
        {
            foreach (ServerDisplay server in FindObjectsOfType<ServerDisplay>())
            {
                server.Destroy();
            }
        } catch { }
    }

    public void Hide()
    {
        ResetList();
        serverList.SetActive(false);
    }
}
