using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.IO;


public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public static DataManager GetInstance() { return instance; }

    public static string username;
    public static int userID;
    public static string ipAddress;
    public static string matchID;
    public static string serverName;

    public static string levelName;

    public static Dictionary<string, string> levelList = new Dictionary<string, string>();
    public List<Sprite> levelSprites = new List<Sprite>();

    [SerializeField] private Button toggleController;
    [SerializeField] private Button toggleStrafe;
    [SerializeField] private Button toggleRumble;
    [SerializeField] private Button toggleCameraRotation;
    [SerializeField] private Button toggleVisualEffects;

    [SerializeField] private Button guest;
    [SerializeField] public TextMeshProUGUI connectionDisplay;
    [SerializeField] public TextMeshProUGUI versionDisplay;
    [SerializeField] private TMP_InputField usernameField;

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
    public int spriteValue = 0;

    public bool xboxController;
    public bool strafeMode;
    public bool spectating = false;

    public static int maxPoints = 24;

    public static Color vehicleColor;

    public bool gameStarted = false;

    public static float soundVolume;
    public static float musicVolume;

    public static int lastPlacement;

    public bool rumble;
    public bool cameraRotation;
    public bool visualEffects;

    public bool loggedIn = false;

    public bool paused = false;
    public bool pauseMenuOpen = false;


    void Start()
    {
        guest.onClick.AddListener(PlayAsGuest);

        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        SceneManager.activeSceneChanged += ChangedActiveScene;

        StartCoroutine(ServerDataManager.GetVersion());

        CheckLogin();

        DontDestroyOnLoad(this);
    }

    public void SetVersion(string version)
    {
        if (version != versionDisplay.text)
        {
            versionDisplay.text += " - NEW VERSION AVALIABLE: " + version;
            StartCoroutine(NewVersionRoutine());
        }
    }

    IEnumerator NewVersionRoutine()
    {
        bool flag = false;
        while (true)
        {
            float t = 0.2f + Time.time;

            while (t > Time.time)
            {
                if (flag)
                {
                    versionDisplay.color = Color.Lerp(Color.red, Color.yellow, (t - Time.time) * 5);
                }
                else
                {
                    versionDisplay.color = Color.Lerp(Color.yellow, Color.red, (t - Time.time) * 5);
                }
                yield return null;
            }
            flag = !flag;
        }
    }
    
    void ChangedActiveScene(Scene current, Scene next)
    {
        levelName = next.name;
        paused = false;
        if (next.name == "Join Lobby")
        {
            MusicManager.instance.Stop();
        }
    }

    void PlayAsGuest()
    {
        loggedIn = false;
        if (usernameField.text == "")
        {
            username = "Guest" + Random.Range(10000, 99999).ToString();
        }
        else
        {
            username = usernameField.text + " - Guest";
        }
        Initialize();

        SceneManager.LoadScene("Join Lobby");
    }

    void CheckLogin()
    {
        if (File.Exists(Application.persistentDataPath + "/login.dat"))
        {
            string mail;
            string password;
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/login.dat"))
            {
                mail = sr.ReadLine();
                password = sr.ReadLine();
            }
            StartCoroutine(ServerDataManager.Login(mail, password));
        }
    }

    public void Login(string mail, string password)
    {
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/login.dat"))
        {
            sw.WriteLine(mail);
            sw.WriteLine(password);
        }
        loggedIn = true;
        StartCoroutine(ServerDataManager.RemoveServers(userID));
        Initialize();

        Debug.Log("Logged");

        SceneManager.LoadScene("Join Lobby");
    }

    public void Logout()
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/login.dat");
        }
        catch 
        {

        }
        SceneManager.LoadScene("Login");
    }

    void Initialize()
    {
        if (loggedIn)
        {
            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/stats.dat"))
            {
                sw.WriteLine(0);
                sw.WriteLine(0);
                sw.WriteLine(0);
                sw.WriteLine(0);
                sw.WriteLine(1);
                sw.WriteLine(0);
                sw.WriteLine(0);
                sw.WriteLine(0.2f);
                sw.WriteLine(0.2f);
                sw.WriteLine(1);
                sw.WriteLine("");
                sw.WriteLine(0);
                sw.WriteLine(0);
            }
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/stats.dat"))
            {

                initSpeed = float.Parse(sr.ReadLine());
                initAcceleration = float.Parse(sr.ReadLine());
                initWeight = float.Parse(sr.ReadLine());
                initHandling = float.Parse(sr.ReadLine());
                xboxController = sr.ReadLine() == "1";
                strafeMode = sr.ReadLine() == "1";
                spriteValue = int.Parse(sr.ReadLine());
                soundVolume = float.Parse(sr.ReadLine());
                musicVolume = float.Parse(sr.ReadLine());
                rumble = sr.ReadLine() == "1";
                username = sr.ReadLine();
                usernameField.text = username;
                cameraRotation = sr.ReadLine() == "1";
                visualEffects = sr.ReadLine() == "1";
            }
            StartCoroutine(ServerDataManager.GetUserData(userID));
        }

        else
        {
            using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/stats.dat"))
            {

                initSpeed = float.Parse(sr.ReadLine());
                initAcceleration = float.Parse(sr.ReadLine());
                initWeight = float.Parse(sr.ReadLine());
                initHandling = float.Parse(sr.ReadLine());
                xboxController = sr.ReadLine() == "1";
                strafeMode = sr.ReadLine() == "1";
                spriteValue = int.Parse(sr.ReadLine());
                soundVolume = float.Parse(sr.ReadLine());
                musicVolume = float.Parse(sr.ReadLine());
                rumble = sr.ReadLine() == "1";
                cameraRotation = sr.ReadLine() == "1";
                visualEffects = sr.ReadLine() == "1";
            }
        }
        StartCoroutine(ServerDataManager.GetLevels());
    }

    public void InitializeStats()
    {
        if (loggedIn)
        {
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/stats.dat"))
            {
                initSpeed = float.Parse(sr.ReadLine());
                initAcceleration = float.Parse(sr.ReadLine());
                initWeight = float.Parse(sr.ReadLine());
                initHandling = float.Parse(sr.ReadLine());
            }
        }
        else
        {
            using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/stats.dat"))
            {
                initSpeed = float.Parse(sr.ReadLine());
                initAcceleration = float.Parse(sr.ReadLine());
                initWeight = float.Parse(sr.ReadLine());
                initHandling = float.Parse(sr.ReadLine());
            }
        }
    }

    public static void IPAddressEdit(string ipAddress)
    {
        DataManager.ipAddress = ipAddress;
    }

    public void ChangeController()
    {
        xboxController = !xboxController;
        if (xboxController)
        {
            toggleController.GetComponentInChildren<TextMeshProUGUI>().text = "XBOX";
        }
        else
        {
            toggleController.GetComponentInChildren<TextMeshProUGUI>().text = "PlayStation";
        }
        WriteStats();
    }

    public void ChangeStrafeMode()
    {
        strafeMode = !strafeMode;
        if (strafeMode)
        {
            toggleStrafe.GetComponentInChildren<TextMeshProUGUI>().text = "Inverted";
        }
        else
        {
            toggleStrafe.GetComponentInChildren<TextMeshProUGUI>().text = "Classic";
        }
        WriteStats();
    }

    public void ToggleRumble()
    {
        rumble = !rumble;
        if (rumble)
        {
            toggleRumble.GetComponentInChildren<TextMeshProUGUI>().text = "Enabled";
        }
        else
        {
            toggleRumble.GetComponentInChildren<TextMeshProUGUI>().text = "Disabled";
        }
        WriteStats();
    }

    public void ToggleCameraRotation()
    {
        cameraRotation = !cameraRotation;
        if (cameraRotation)
        {
            toggleCameraRotation.GetComponentInChildren<TextMeshProUGUI>().text = "Dynamic";
        }
        else
        {
            toggleCameraRotation.GetComponentInChildren<TextMeshProUGUI>().text = "Static";
        }
        WriteStats();
    }

    public void ToggleVisualEffects()
    {
        visualEffects = !visualEffects;
        if (visualEffects)
        {
            toggleVisualEffects.GetComponentInChildren<TextMeshProUGUI>().text = "Enabled";
        }
        else
        {
            toggleVisualEffects.GetComponentInChildren<TextMeshProUGUI>().text = "Disabled";
        }
        WriteStats();
    }

    public void SetVehicleSprite(int i)
    {
        spriteValue = i;
        WriteStats();
    }

    public void SetStats()
    {
        if (loggedIn)
        {
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/stats.dat"))
            {
                initSpeed = float.Parse(sr.ReadLine());
                initAcceleration = float.Parse(sr.ReadLine());
                initWeight = float.Parse(sr.ReadLine());
                initHandling = float.Parse(sr.ReadLine());
                xboxController = sr.ReadLine() == "1";
                strafeMode = sr.ReadLine() == "1";
                spriteValue = int.Parse(sr.ReadLine());
                soundVolume = float.Parse(sr.ReadLine());
                musicVolume = float.Parse(sr.ReadLine());
                rumble = sr.ReadLine() == "1";
                username = sr.ReadLine();
                cameraRotation = sr.ReadLine() == "1";
                visualEffects = sr.ReadLine() == "1";
            }
        }
        else
        {
            using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/stats.dat"))
            {
                initSpeed = float.Parse(sr.ReadLine());
                initAcceleration = float.Parse(sr.ReadLine());
                initWeight = float.Parse(sr.ReadLine());
                initHandling = float.Parse(sr.ReadLine());
                xboxController = sr.ReadLine() == "1";
                strafeMode = sr.ReadLine() == "1";
                spriteValue = int.Parse(sr.ReadLine());
                soundVolume = float.Parse(sr.ReadLine());
                musicVolume = float.Parse(sr.ReadLine());
                rumble = sr.ReadLine() == "1";
                username = sr.ReadLine();
                usernameField.text = username;
                cameraRotation = sr.ReadLine() == "1";
                visualEffects = sr.ReadLine() == "1";
            }
        }
        if (xboxController)
        {
            toggleController.GetComponentInChildren<TextMeshProUGUI>().text = "XBOX";
        }
        else
        {
            toggleController.GetComponentInChildren<TextMeshProUGUI>().text = "PlayStation";
        }
        if (strafeMode)
        {
            toggleStrafe.GetComponentInChildren<TextMeshProUGUI>().text = "Inverted";
        }
        else
        {
            toggleStrafe.GetComponentInChildren<TextMeshProUGUI>().text = "Classic";
        }
        if (rumble)
        {
            toggleRumble.GetComponentInChildren<TextMeshProUGUI>().text = "Enabled";
        }
        else
        {
            toggleRumble.GetComponentInChildren<TextMeshProUGUI>().text = "Disabled";
        }
        if (cameraRotation)
        {
            toggleCameraRotation.GetComponentInChildren<TextMeshProUGUI>().text = "Dynamic";
        }
        else
        {
            toggleCameraRotation.GetComponentInChildren<TextMeshProUGUI>().text = "Static";
        }
        if (visualEffects)
        {
            toggleVisualEffects.GetComponentInChildren<TextMeshProUGUI>().text = "Enabled";
        }
        else
        {
            toggleVisualEffects.GetComponentInChildren<TextMeshProUGUI>().text = "Disabled";
        }
        serverName = username + "\'s Server";
    }

    public void SetStats(float speed, float acceleration, float weight, float handling)
    {
        this.speed = speed;
        this.acceleration = acceleration;
        this.weight = weight / 5;
        this.handling = handling / 10;
    }

    public void WriteStats()
    {
        if (loggedIn)
        {
            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/stats.dat"))
            {
                sw.WriteLine(this.speed);
                sw.WriteLine(this.acceleration);
                sw.WriteLine(this.weight);
                sw.WriteLine(this.handling);
                sw.WriteLine(xboxController ? 1 : 0);
                sw.WriteLine(strafeMode ? 1 : 0);
                sw.WriteLine(this.spriteValue);
                sw.WriteLine(soundVolume);
                sw.WriteLine(musicVolume);
                sw.WriteLine(rumble ? 1 : 0);
                sw.WriteLine(username);
                sw.WriteLine(cameraRotation ? 1 :  0);
                sw.WriteLine(visualEffects ? 1 :  0);
            }
            string w = weight.ToString().Replace(',', '.');
            string h = handling.ToString().Replace(',', '.');
            string s = soundVolume.ToString().Replace(',', '.');
            string m = musicVolume.ToString().Replace(',', '.');
            StartCoroutine(ServerDataManager.SetUserData($"?id={userID}&speed={speed}&acceleration={acceleration}&weight={w}&&handling={h}&controller={xboxController}&strafe={strafeMode}&vehicle={spriteValue}&sound={s}&music={m}&rumble={rumble}&rotation={cameraRotation}&effects={visualEffects}"));
        }
        else
        {
            using (StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/stats.dat"))
            {
                sw.WriteLine(this.speed);
                sw.WriteLine(this.acceleration);
                sw.WriteLine(this.weight);
                sw.WriteLine(this.handling);
                sw.WriteLine(xboxController ? 1 : 0);
                sw.WriteLine(strafeMode ? 1 : 0);
                sw.WriteLine(this.spriteValue);
                sw.WriteLine(soundVolume);
                sw.WriteLine(musicVolume);
                sw.WriteLine(rumble ? 1 : 0);
                sw.WriteLine(username);
                sw.WriteLine(cameraRotation ? 1 : 0);
                sw.WriteLine(visualEffects ? 1 : 0);
            }
        }
    }

    public void WriteStats(string stats)
    {
        string[] statsArray = stats.Split('_');
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/stats.dat"))
        {
            foreach (string stat in statsArray)
            {
                string s = stat.Replace('.', ',');
                sw.WriteLine(s);
            }
        }
        SetStats();
    }

    public void WriteLevels(string levels)
    {
        string[] levelArray = levels.Split('|');
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/levels.dat"))
        {
            foreach (string level in levelArray)
            {
                sw.WriteLine(level);
            }
        }

        if (levelList.Values.Count == 0)
        {
            if (loggedIn)
            {
                using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/levels.dat"))
                {
                    while (sr.Peek() >= 0)
                    {
                        string[] s = sr.ReadLine().Split('_');
                        if (s[0] != "")
                        {
                            levelList.Add(s[0], s[1]);
                        }
                    }
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/levels.dat"))
                {
                    while (sr.Peek() >= 0)
                    {
                        string[] s = sr.ReadLine().Split('_');
                        if (s[0] != "")
                        {
                            levelList.Add(s[0], s[1]);
                        }
                    }
                }
            }
        }
    }

    public IEnumerator GetRecord()
    {
        using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/records.dat"))
        {
            while (sr.Peek() >= 0)
            {
                string[] s = sr.ReadLine().Split('_');
                if (s[0] == levelName)
                {
                    float min = float.Parse(s[1].Split(':')[0]);
                    float sec = float.Parse(s[1].Split(':')[1]) + (min * 60);
                    Timer.instance.SetRecordTime(s[1], sec);
                    yield return 1;
                }
            }
        }
        yield return 0;
    }

    public IEnumerator SaveRecord(string record)
    {
        string saveData = "";
        using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/records.dat"))
        {
            while (sr.Peek() >= 0)
            {
                string s = sr.ReadLine();
                if (s.Split('_')[0] == levelName)
                {
                    saveData += levelName + "_" + record + "\n";
                }
                else
                {
                    saveData += s + "\n";
                }
            }
        }

        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/records.dat"))
        {
            sw.Write(saveData);
        }

        yield return 1;
    }
}
