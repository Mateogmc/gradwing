using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;

    public static GameStateManager GetInstance() { return instance; }

    public enum GameState
    {
        None,
        OnLobby,
        Running,
        GameOver
    }

    public GameState gameState = GameState.OnLobby;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
}
