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
        Starting,
        Running,
        GameOver
    }

    public GameState gameState = GameState.OnLobby;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
