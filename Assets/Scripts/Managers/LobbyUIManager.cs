using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] Button disconnect;
    [SerializeField] Button startGame;
    [SerializeField] Button editStats;
    [SerializeField] Button setStats;
    [SerializeField] Button setVehicle;

    [SerializeField] GameObject statPanel;
    [SerializeField] GameObject vehiclePanel;

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

    DataManager dataManager;
    [SerializeField] LobbyManager lobbyManager;



    private void Start()
    {
        disconnect.onClick.AddListener(() => Disconnect());
        startGame.onClick.AddListener(() => StartGame());
        editStats.onClick.AddListener(() => EditStats());
        velocity.onValueChanged.AddListener(delegate { SliderChange(0); });
        acceleration.onValueChanged.AddListener(delegate { SliderChange(1); });
        weight.onValueChanged.AddListener(delegate { SliderChange(2); });
        handling.onValueChanged.AddListener(delegate { SliderChange(3); });
        setStats.onClick.AddListener(() => SetStats());
        setVehicle.onClick.AddListener(() => OpenVehicleWindow());


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
    }

    public void SetVehicleListeners()
    {
        for (int i = 0; i < vehicles.Length; i++)
        {
            int j = i;
            vehicles[i].onClick.AddListener(() => SetVehicle(j));
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
        dataManager.SetStats(velocity.value, acceleration.value, weight.value, handling.value);
        remainingPoints.text = (DataManager.maxPoints - (velocity.value + acceleration.value + weight.value + handling.value)).ToString();
    }

    private void EditStats()
    {
        statPanel.SetActive(!statPanel.activeSelf);
        vehiclePanel.SetActive(false);
    }

    private void OpenVehicleWindow()
    {
        vehiclePanel.SetActive(!vehiclePanel.activeSelf);
        statPanel.SetActive(false);
    }

    private void SetVehicle(int i)
    {
        OpenVehicleWindow();
        DataManager.GetInstance().SetVehicleSprite(i);
        NetworkClient.localPlayer.gameObject.GetComponent<MultiplayerController>().Restart();
    }

    private void SetStats()
    {
        EditStats();
        DataManager.GetInstance().WriteStats();
        NetworkClient.localPlayer.gameObject.GetComponent<MultiplayerController>().Restart();
    }

    private void SelectLevel(int level)
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
    }

    private void StartGame()
    {
        DataManager.GetInstance().gameStarted = true;
        startGame.gameObject.SetActive(false);
        LobbyManager.GetInstance().CmdStartLobby(true);
    }

    private void Disconnect()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
