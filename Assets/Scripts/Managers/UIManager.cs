using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public LobbyStatus lobbyStatus;

    IEnumerator Start()
    {
        while (LobbyManager.GetInstance() == null || LobbyManager.GetInstance().localPlayer == null)
        {
            yield return null;
        }
    }

    public void OnStartGame()
    {
        lobbyStatus.StartCountDown();
    }

    public void OnLobbyReady(bool ready, List<int> list)
    {
        lobbyStatus.LobbyReady(ready, list);
    }
}
