using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    private static LobbyManager instance;

    public static LobbyManager GetInstance() { return instance; }

    public MultiplayerController localPlayer;

    private void Awake()
    {
        GameStateManager.GetInstance().gameState = GameStateManager.GameState.OnLobby;
        Debug.Log("Lobby");
        instance = this;
    }

    private void Start()
    {
        if (isServer)
        {
            countdownValue = 30;
            Debug.Log(DataManager.GetInstance().gameStarted);
            if (DataManager.GetInstance().gameStarted)
            {
                CmdStartLobby(true);
            }
        }
    }

    [SerializeField] UIManager uiManager;

    [SyncVar(hook = nameof(OnCountdownChange))] public int countdownValue;
    [SyncVar(hook = nameof(OnLobbyReady))] public bool lobbyReady = false;

    [SyncVar(hook = nameof(OnPlayerCountChange))] public int playerCount;

    private List<int> levels = new List<int>();

    [SyncVar(hook = nameof(OnGameReady))] public bool gameReady = false;

    [SyncVar(hook = nameof(Level1Change))] private int level1;
    [SyncVar(hook = nameof(Level2Change))] private int level2;
    [SyncVar(hook = nameof(Level3Change))] private int level3;

    private void Level1Change(int oldVal, int newVal)
    {
        level1 = newVal;
    }

    private void Level2Change(int oldVal, int newVal)
    {
        level2 = newVal;
    }

    private void Level3Change(int oldVal, int newVal)
    {
        level3 = newVal;
    }
    private void OnPlayerCountChange(int oldVal, int newVal)
    {
        playerCount = newVal;
    }

    private void OnGameReady(bool oldReady, bool newReady)
    {
        gameReady = newReady;
    }

    private void OnLobbyReady(bool oldBool, bool newBool)
    {
        lobbyReady = newBool;
    }

    private void OnCountdownChange(int oldVal, int newVal)
    {
        countdownValue = newVal;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetCountdown(int val)
    {
        if (isServer)
        {
            countdownValue = val;
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdSelectLevel(int level)
    {
        switch (level)
        {
            case 1:
                level1++;
                break;

            case 2:
                level2++;
                break;

            case 3:
                level3++;
                break;
        }

        if (level1 + level2 + level3 >= playerCount)
        {
            gameReady = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdStartGame()
    {
        string levelSelected = "";
        int random = Random.Range(1, level1 + level2 + level3);
        bool flag = true;
        random -= level1;
        if (random <= 0 && flag)
        {
            levelSelected = DataManager.levelList[levels[0]];
            flag = false;
        }
        random -= level2;
        if (random <= 0 && flag)
        {
            levelSelected = DataManager.levelList[levels[1]];
            flag = false;
        }
        if (flag)
        {
            levelSelected = DataManager.levelList[levels[2]];
        }
        RpcStartGame();
        NetworkManager.singleton.ServerChangeScene(levelSelected);
    }

    [Command(requiresAuthority = false)]
    public void CmdStartGame(string level)
    {
        NetworkManager.singleton.ServerChangeScene(level);
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        //uiManager.OnStartGame();
    }

    [Command(requiresAuthority = false)]
    public void CmdStartLobby(bool startLobby)
    {
        lobbyReady = startLobby;
        LobbyReady(playerCount);
    }

    [Server]
    public void LobbyReady(int playerCount)
    {
        this.playerCount = playerCount;
        levels.Clear();
        level1 = 0;
        level2 = 0;
        level3 = 0;
        while (levels.Count < 3)
        {
            int val = Random.Range(0, DataManager.levelList.Count);
            if (!levels.Contains(val))
            {
                levels.Add(val);
            }
        }
        RpcLobbyReady(playerCount, levels);
    }

    [ClientRpc]
    private void RpcLobbyReady(int playerCount, List<int> list)
    {
        uiManager.OnLobbyReady(playerCount, list);
    }
}
