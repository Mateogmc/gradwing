using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RaceManager : NetworkBehaviour
{
    private static RaceManager instance;
    public static RaceManager GetInstance() { return instance; }

    private void Start()
    {
        instance = this;
    }

    [Command(requiresAuthority = false)]
    public void CmdReturnToLobby()
    {
        if (MultiplayerController.localPlayer.isServer)
        {
            GameStateManager.GetInstance().CmdSetState(GameStateManager.GameState.OnLobby);
        }
        NetworkManager.singleton.ServerChangeScene("Lobby");
    }
}
