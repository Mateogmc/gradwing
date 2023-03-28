using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ExitToLobby : MonoBehaviour
{

    public Button returnToLobby;

    public void ReturnToLobby()
    {
        Debug.Log("Click");
        RaceManager.GetInstance().CmdReturnToLobby();
    }

    private void Update()
    {
        if (GameStateManager.GetInstance().gameState == GameStateManager.GameState.Running && NetworkServer.active)
        {
            returnToLobby.gameObject.SetActive(true);
        }
        else
        {
            returnToLobby.gameObject.SetActive(false);
        }
    }
}
