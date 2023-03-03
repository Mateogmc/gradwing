using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public Button returnToLobby;
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.current;
    }

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
