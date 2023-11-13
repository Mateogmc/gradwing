using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] Button startGame;
    [SerializeField] Button stopGame;
    [SerializeField] Button tutorial;
    [SerializeField] Button setStats;

    [SerializeField] GameObject statPanel;
    [SerializeField] GameObject vehiclePanel;
    [SerializeField] GameObject leaderboardPanel;

    [SerializeField] Slider velocity;
    [SerializeField] Slider acceleration;
    [SerializeField] Slider weight;
    [SerializeField] Slider handling;

    public Button[] vehicles;

    [SerializeField] Button level1;
    [SerializeField] TextMeshProUGUI level1Text;
    [SerializeField] Button level2;
    [SerializeField] TextMeshProUGUI level2Text;
    [SerializeField] Button level3;
    [SerializeField] TextMeshProUGUI level3Text;

    [SerializeField] TextMeshProUGUI remainingPoints;
    [SerializeField] VehicleScrollView vehicle;

    DataManager dataManager;
    [SerializeField] LobbyManager lobbyManager;

    bool levelSelected = false;



    private void Start()
    {
        startGame.onClick.AddListener(() => StartGame());
        stopGame.onClick.AddListener(() => StopGame());  
        tutorial.onClick.AddListener(() => Tutorial());
        velocity.onValueChanged.AddListener(delegate { SliderChange(0); });
        acceleration.onValueChanged.AddListener(delegate { SliderChange(1); });
        weight.onValueChanged.AddListener(delegate { SliderChange(2); });
        handling.onValueChanged.AddListener(delegate { SliderChange(3); });
        setStats.onClick.AddListener(() => SetStats());


        level1.onClick.AddListener(() => SelectLevel(1));
        level2.onClick.AddListener(() => SelectLevel(2));
        level3.onClick.AddListener(() => SelectLevel(3));

        dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
        DataManager.GetInstance().InitializeStats();
        Debug.Log(DataManager.GetInstance().initSpeed);
        velocity.value = dataManager.initSpeed;
        acceleration.value = dataManager.initAcceleration;
        weight.value = dataManager.initWeight * 5;
        handling.value = dataManager.initHandling * 10;

        startGame.gameObject.SetActive(NetworkServer.active && !DataManager.GetInstance().gameStarted);
        stopGame.gameObject.SetActive(NetworkServer.active && DataManager.GetInstance().gameStarted);

        VehicleScrollView.currentVehicle = DataManager.GetInstance().spriteValue;
        SetStats();
        //tutorial.gameObject.SetActive(NetworkServer.active);
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (DataManager.GetInstance().pauseMenuOpen)
        {
            return;
        }
        if (InputManager.instance.controls.Buttons.EditVehicle.WasPressedThisFrame())
        {
            EditVehicle();
        } 
        if (InputManager.instance.controls.Buttons.Leaderboard.WasPressedThisFrame())
        {
            Leaderboard();
        }
        if (lobbyManager.lobbyReady)
        {
            if (InputManager.instance.controls.General.Level1.WasPressedThisFrame() && !levelSelected)
            {
                SelectLevel(1);
            }
            else if (InputManager.instance.controls.General.Level2.WasPressedThisFrame() && !levelSelected)
            {
                SelectLevel(2);
            }
            else if (InputManager.instance.controls.General.Level3.WasPressedThisFrame() && !levelSelected)
            {
                SelectLevel(3);
            }
        }
        if (InputManager.instance.controls.Buttons.StartGame.WasPressedThisFrame() && NetworkServer.active)
        {
            if (startGame.IsActive())
            {
                StartGame();
            }
            else
            {
                StopGame();
            }
        }
    }

    private void SliderChange(int stat)
    {
        if (velocity.value + acceleration.value + weight.value + handling.value > DataManager.maxPoints)
        {
            while (velocity.value + acceleration.value + weight.value + handling.value > DataManager.maxPoints)
            {
                switch (stat)
                {
                    case 0:
                        velocity.value--;
                        break;

                    case 1:
                        acceleration.value--;
                        break;

                    case 2:
                        weight.value--;
                        break;

                    case 3:
                        handling.value--;
                        break;
                }
            }
        }
        remainingPoints.text = (DataManager.maxPoints - (velocity.value + acceleration.value + weight.value + handling.value)).ToString();
    }

    public void EditVehicle()
    {
        statPanel.SetActive(!statPanel.activeSelf);
        if (statPanel.activeSelf)
        {
            velocity.Select();
            leaderboardPanel.SetActive(false);
        }
        DataManager.GetInstance().paused = statPanel.activeSelf;
    }

    public void EditVehicle(bool activate)
    {
        statPanel.SetActive(activate);
        if (statPanel.activeSelf)
        {
            velocity.Select();
            leaderboardPanel.SetActive(false);
        }
        DataManager.GetInstance().paused = statPanel.activeSelf;
    }

    public void SetStats()
    {
        EditVehicle(false);
        DataManager.GetInstance().SetStats(velocity.value, acceleration.value, weight.value, handling.value);
        DataManager.GetInstance().SetVehicleSprite(VehicleScrollView.currentVehicle);
        DataManager.GetInstance().WriteStats();
        NetworkClient.localPlayer.gameObject.GetComponent<MultiplayerController>().Restart();
    }

    public void SelectLevel(int level)
    {
        if (level == 0)
        {
            level1.interactable = true;
            level2.interactable = true;
            level3.interactable = true;
            levelSelected = false;
        }
        else
        {
            level1.interactable = false;
            level2.interactable = false;
            level3.interactable = false;

            switch (level)
            {
                case 1:
                    level1.GetComponent<Image>().color = Color.yellow;
                    break;

                case 2:
                    level2.GetComponent<Image>().color = Color.yellow;
                    break;

                case 3:
                    level3.GetComponent<Image>().color = Color.yellow;
                    break;
            }

            lobbyManager.CmdSelectLevel(level);
            levelSelected = true;
        }
    }

    private void StartGame()
    {
        DataManager.GetInstance().gameStarted = true;
        startGame.gameObject.SetActive(false);
        stopGame.gameObject.SetActive(true);
        LobbyManager.GetInstance().CmdStartLobby(true);
    }

    private void StopGame()
    {
        DataManager.GetInstance().gameStarted = false;
        startGame.gameObject.SetActive(true);
        stopGame.gameObject.SetActive(false);
        LobbyManager.GetInstance().CmdStartLobby(false);
        levelSelected = false;
    }

    private void Tutorial()
    {
        LobbyManager.GetInstance().CmdStartGame("Tutorial");
    }

    /*private void Spectate()
    {
        if (DataManager.GetInstance().spectating && GameStateManager.CanPlay())
        {
            DataManager.GetInstance().spectating = false;
            MultiplayerController.localPlayer.Spectate(false);
            spectate.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Spectate";
        }
        else if (!DataManager.GetInstance().spectating)
        {
            DataManager.GetInstance().spectating = true;
            MultiplayerController.localPlayer.Spectate(true);
            spectate.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Play";
        }
    }*/

    public void Leaderboard()
    {
        leaderboardPanel.SetActive(!leaderboardPanel.activeSelf);
        EditVehicle(false);
        DataManager.GetInstance().paused = leaderboardPanel.activeSelf;
    }

    public void Leaderboard(bool activate)
    {
        leaderboardPanel.SetActive(activate);
        EditVehicle(false);
        DataManager.GetInstance().paused = leaderboardPanel.activeSelf;
    }
}
