using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameStateManager : NetworkBehaviour
{
    private static GameStateManager instance;

    public static GameStateManager GetInstance() { return instance; }

    public enum GameState
    {
        None,
        OnLobby,
        Starting,
        Running,
        GameOver
    }

    [SyncVar(hook = nameof(OnStateChange))] public GameState gameState = GameState.OnLobby;

    private void OnStateChange(GameState oldState, GameState newState)
    {
        gameState = newState;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetState(GameState state)
    {
        RpcSetState(state);
    }

    [ClientRpc]
    private void RpcSetState(GameState state)
    {
        gameState = state;
        Debug.Log($"State changed: {gameState}");
    }

    public GameState GetState()
    {
        return gameState;
    }

    [Command]
    public void CmdCheckPlayerCount(NetworkConnectionToClient target)
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 8)
        {
            TargetMakeSpectator(target);
        }
    }

    [TargetRpc]
    private void TargetMakeSpectator(NetworkConnectionToClient target)
    {
        DataManager.GetInstance().spectating = true;
        MultiplayerController.localPlayer.Spectate(true);
    }

    public static bool CanPlay()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 8)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Debug.Log("GameState");
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
}
