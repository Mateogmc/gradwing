/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;
    public SyncListMatch matches = new SyncListMatch();

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(instance.gameObject);

        }
    }
    public static string GetRandomMatchID()
    {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = Random.Range(0, 36);
            if (random < 26)
            {
                _id += (char)(random + 65);
            }
            else
            {
                _id += (random - 26).ToString();
            }
        }
        Debug.Log($"Random Match ID: {_id}");
        return _id;
    }

    public bool HostGame(string matchID)
    {
        return true;
    }
}

[System.Serializable]
public class Match
{
    public string matchID;
    public SyncListGameObject players = new SyncListGameObject();

    public Match(string matchID, GameObject player)
    {
        this.matchID = matchID;
        players.Add(player);
    }

    public Match() { }
}

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { }

[System.Serializable]
public class SyncListMatch : SyncList<Match> { }*/