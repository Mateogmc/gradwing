using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.IO;


public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public static DataManager GetInstance() { return instance; }

    public static string username;
    public static string ipAddress;

    public static List<string> levelList = new List<string>();
    public List<Sprite> levelSprites = new List<Sprite>();

    [SerializeField] private Button host;
    [SerializeField] private Button connect;
    [SerializeField] private Button exit;
    [SerializeField] private Button controller;
    [SerializeField] private Sprite xbox; 
    [SerializeField] private Sprite ps4;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI connectionDisplay;

    public const float MAX_SPEED = 45;
    public const float ACCELERATION = 10;
    public const float WEIGHT = 1;
    public const float HANDLING = 0.5f;

    public float initSpeed;
    public float initAcceleration;
    public float initWeight;
    public float initHandling;

    public float speed = MAX_SPEED;
    public float acceleration = ACCELERATION;
    public float weight = WEIGHT;
    public float handling = HANDLING;

    public bool xboxController;

    public static int maxPoints = 24;


    void Start()
    {
        DontDestroyOnLoad(this);
        host.onClick.AddListener(Host);
        connect.onClick.AddListener(Connect);
        exit.onClick.AddListener(Exit);
        controller.onClick.AddListener(ChangeController);

        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/stats.dat"))
        {
            initSpeed = float.Parse(sr.ReadLine());
            initAcceleration = float.Parse(sr.ReadLine());
            initWeight = float.Parse(sr.ReadLine());
            initHandling = float.Parse(sr.ReadLine());
            xboxController = sr.ReadLine() == "1";
        }
        using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/levels.dat"))
        {
            while (sr.Peek() >= 0)
            {
                levelList.Add(sr.ReadLine());
            }
        }
        if (xboxController)
        {
            controller.GetComponent<Image>().sprite = xbox;
        }
        else
        {
            controller.GetComponent<Image>().sprite = ps4;
        }
        instance = this;
    }

    private void Update()
    {
        if (NetworkServer.active && NetworkClient.active)
        {
            connectionDisplay.text = "Hosting";
        } else if (NetworkClient.active)
        {
            connectionDisplay.text = "Connected on " + ipAddress;
        } else
        {
            connectionDisplay.text = "Not connected";
        }
    }

    private void Host()
    {
        Debug.Log(username);
        networkManager.StartHost();
    }

    private void Connect()
    {
        networkManager.StartClient();
        networkManager.networkAddress = ipAddress;
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void ChangeController()
    {
        xboxController = !xboxController;
        if (xboxController)
        {
            controller.GetComponent<Image>().sprite = xbox;
        }
        else
        {
            controller.GetComponent<Image>().sprite = ps4;
        }
        SetStats();
    }

    public void UsernameEdit(string username)
    {
        DataManager.username = username;
    }

    public void IpAddressEdit(string ipAddress)
    {
        DataManager.ipAddress = ipAddress;
    }

    public void SetStats()
    {
        using (StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/stats.dat"))
        {
            sw.WriteLine(speed);
            sw.WriteLine(acceleration);
            sw.WriteLine(weight);
            sw.WriteLine(handling);
            sw.WriteLine(xboxController ? 1 : 0);
        }
    }

    public void SetStats(float speed, float acceleration, float weight, float handling)
    {
        this.speed = speed;
        this.acceleration = acceleration;
        this.weight = weight / 5;
        this.handling = handling / 10;

        using (StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/stats.dat"))
        {
            sw.WriteLine(this.speed);
            sw.WriteLine(this.acceleration);
            sw.WriteLine(this.weight);
            sw.WriteLine(this.handling);
            sw.WriteLine(xboxController ? 1 : 0);
        }
    }
}
