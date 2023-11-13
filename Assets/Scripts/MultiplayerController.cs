using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;

public enum PlayerStates
{
    None, Starting, Grounded, Jumping, Dead, Finish
}

public class MultiplayerController : FSM
{
    public static MultiplayerController localPlayer;

    [SyncVar(hook = nameof(OnStateChange))]public PlayerStates currentState = PlayerStates.Grounded;

    [Header("Inspector variables")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] GameObject audioListener;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Windshield vehicle;
    [SerializeField] [SyncVar(hook = nameof(OnColorChange))] public Color vehicleColor;
    DataManager dataManager;
    bool xboxController = true;


    [Header("Player variables")]
    [SerializeField] [SyncVar(hook = nameof(OnMaxSpeedChange))] public float maxSpeed;
    float currentMaxSpeed;
    [SerializeField] [SyncVar(hook = nameof(OnAccelerationChange))] float acceleration;
    [SerializeField] [SyncVar(hook = nameof(OnWeightChange))] float weight;
    [SerializeField] float turnSpeed;
    [SerializeField] float drag;
    float currentDrag;
    [SerializeField] float gravity;
    [SerializeField] float jumpHeight;
    [SerializeField] [SyncVar(hook = nameof(OnHandlingChange))] float rotationSpeed;
    float currentRotationSpeed;
    [SerializeField] float strafingAngle;
    [SerializeField] float shieldDuration;
    [SerializeField] float deathTimer;
    [SerializeField][SyncVar(hook = nameof(OnSpriteChange))] int spriteValue;
    bool smoking = false;
    bool onFire = false;

    float dead;
    float currentSpeed = 0;
    float bounceDuration = 0.5f;
    [HideInInspector] public float bounceTime = 0f;
    bool wasBouncing;
    [SyncVar(hook = nameof(OnBoostChange))]public float boostTime = 0f;
    float initialBoost = 0;
    private GameObject explosion;
    float airborne = 0f;
    [SyncVar(hook = nameof(OnAirborneChange))] public float currentAirborne = 1;
    float airborneVelocity = 0f;
    float iceVelocity = 0f;
    int grounded = 1;
    public float currentScale = 1;
    public Vector2 direction;
    [HideInInspector] [SyncVar(hook = nameof(LastSpeedVectorChange))] public Vector2 lastSpeed;
    [SyncVar(hook = nameof(LastSpeedChange))] public float lastSpeedMagnitude;
    [SyncVar(hook = nameof(OnLayerChange))]int currentLayer = 6;
    [SyncVar(hook = nameof(OnSpectatingChange))] bool spectating = false;

    bool fallProtection;

    [HideInInspector] bool accelerating = false;
    [HideInInspector] bool braking = false;
    [HideInInspector] public bool rotatingRight = false;
    [HideInInspector] public bool rotatingLeft = false;
    [HideInInspector] public bool strafingRight = false;
    [HideInInspector] public bool strafingLeft = false;
    [HideInInspector] public float strafeValue;
    [HideInInspector] public float verticalValue;
    [HideInInspector] public bool rolling;
    private bool rollingEnded = false;
    [HideInInspector] [SyncVar(hook = nameof(OnRollingChange))] public bool rollingSync;

    [SerializeField] [SyncVar(hook = nameof(OnMaxHealthChange))] float maxHealth;
    [SerializeField] float health;

    [Header("Item Sprites")]
    [SerializeField] Sprite none;
    [SerializeField] Sprite shield;
    [SerializeField] Sprite jump;
    [SerializeField] Sprite trap;
    [SerializeField] Sprite rebounder;
    [SerializeField] Sprite laser;
    [SerializeField] Sprite boost;
    [SerializeField] Sprite missile;
    [SerializeField] Sprite shockwave;
    [SerializeField] Sprite equalizer;
    [SerializeField] Sprite flash;
    [SerializeField] Image itemSprite;

    [SyncVar(hook = nameof(OnShieldChange))] float shielded = 0f;
    bool wasShielded = false;
    float slowTimer = 0f;

    [SerializeField] GameObject trapPrefab;
    [SerializeField] GameObject rebounderPrefab;
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] GameObject equalizerPrefab;
    [SerializeField] GameObject flashPrefab;
    GameObject flashActive;
    Vector3 itemPosition;

    bool gettingItem = false;

    // Laser tools
    [SerializeField] GameObject laserHit;
    [SerializeField] LineRenderer lineRenderer;
    [SyncVar(hook = nameof(OnFiringLaserChange))] bool firingLaser = false;
    [SyncVar(hook = nameof(OnLaserCountdownChange))] float laserCountdown = 0f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject laserPoint;
    [SerializeField] Material laserMaterial;
    [SerializeField] Transform laserPosition;
    [SerializeField] GameObject laserVFX;

    // Flash tools

    [SerializeField] [SyncVar(hook = nameof(OnItemChange))] Items currentItem = Items.None;

    [SyncVar(hook = nameof(OnUsernameChange))] public string usernameText;

    // Lap Manager
    float raceCountdown;
    [SerializeField] GameObject lapTextRenderer;
    [SerializeField] [SyncVar(hook = nameof(OnLapChange))] int currentLap = 1;
    [SyncVar(hook = nameof(OnCheckpointChange))] int currentCheckpoint;
    LapManager lapManager;
    int checkpointCount = 0;
    Vector2 spawnPos = Vector2.zero;
    float spawnRotation = 0;
    [SyncVar(hook = nameof(OnPlacementChange))]public int placement;
    public EndgameManager endgameManager;
    [SerializeField] Timer timerManager;
    [SyncVar(hook = nameof(OnTimerChange))] public string timer;

    // Interface
    [SerializeField] TextMeshProUGUI lapRenderer;
    [SerializeField] Canvas playerInterface;
    [SerializeField] Canvas minimap;
    [SerializeField] Canvas enemyInterface;
    [SerializeField] HealthBar healthBar;
    [SerializeField] HealthBar playerHealthBar;
    [SerializeField] Image playerItemSprite;
    [SerializeField] GameObject rollingArrow;
    [SerializeField] Canvas ui;
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] SpriteRenderer shieldRenderer;
    [SerializeField] Image placementNumber;
    [SerializeField] Canvas endgameInterface;
    [SerializeField] Image endgamePlacementImage;
    [SerializeField] Canvas startingCanvas;
    [SerializeField] TextMeshProUGUI startingText;
    [SerializeField] TextMeshProUGUI courseName;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] CommandLine consoleMenu;
    [SerializeField] TextMeshProUGUI velocityDisplay;
    [SerializeField] VisualEffect fireVFX;

    // Audio
    [SerializeField] VehicleAudioManager vehicleAudioManager;

    public override void OnStartLocalPlayer()
    {
        if (LobbyManager.GetInstance().localPlayer == null && GameStateManager.GetInstance().GetState() == GameStateManager.GameState.OnLobby)
        {
            LobbyManager.GetInstance().localPlayer = this;
        }
    }

    protected override void Initialize()
    {
        if (GameStateManager.GetInstance().GetState() == GameStateManager.GameState.Running)
        {
            DataManager.GetInstance().spectating = true;
        }
        if (isLocalPlayer)
        {
            pauseMenu = FindObjectOfType<PauseMenu>();
            consoleMenu = FindObjectOfType<CommandLine>();
            localPlayer = this;
            audioListener.SetActive(true);
            playerInterface.gameObject.SetActive(true);
            minimap.gameObject.SetActive(true);
            AudioManager.instance.Stop("Ice");
            AudioManager.instance.Stop("Gravel");
            AudioManager.instance.Stop("Heal");
            MusicManager.instance.Stop("Lobby");
            HapticsManager.instance.heal = false;
            HapticsManager.instance.ice = false;
            HapticsManager.instance.gravel = false;
            HapticsManager.instance.fire = false;
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                MusicManager.instance.Play(FindObjectOfType<LevelData>().GetWorld());
                currentState = PlayerStates.Grounded;
                placement = 3;
            }
            else
            {
                if (isServer)
                {
                    GameStateManager.GetInstance().CmdSetState(GameStateManager.GameState.Starting);
                }
                currentState = PlayerStates.Starting;
                startingCanvas.enabled = true;
                StartRace();
                CmdStartRace();
            }
            spawnPos = transform.position;
            grounded = 0;
            rb.rotation = 90;
            currentDrag = drag;
            currentCheckpoint = 0;
            dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Initialize(gameObject);
            playerInterface.transform.parent = null;
            minimap.transform.parent = null;
            startingCanvas.transform.parent = null;
            endgameInterface.transform.parent = null;
            lineRenderer.material = new Material(laserMaterial);
            if (FindObjectOfType<LevelData>() != null)
            {
                courseName.text = FindObjectOfType<LevelData>().GetCourse();
                courseName.fontMaterial = GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().GetMaterial();
                startingText.fontMaterial = GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().GetMaterial();
            }
            endgameManager.SetInstance();
            timerManager.SetInstance();
            Restart();
        }
        if (DataManager.GetInstance().spectating && SceneManager.GetActiveScene().name != "Lobby")
        {
            Spectate(true);
        }
    }

    protected override void FSMUpdate()
    {
        PlayerStates state = currentState;
        ui.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        audioListener.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        if (Mathf.FloorToInt(raceCountdown - (float)NetworkTime.time) == 0 && currentState == PlayerStates.Starting)
        {
            GetComponent<Timer>().Initialize();
        }
        if (!isLocalPlayer) 
        {
            CheckStateRemote();
            return; 
        }
        CheckInput();
        CheckState();
        if (currentItem == Items.Laser)
        {
            LaserUpdate();
        }
        if (currentItem == Items.Flash)
        {
            FlashUpdate();
        }
        if (currentSpeed < 0.1f && !accelerating && bounceTime < Time.time)
        {
            rb.velocity = Vector2.zero;
        }
        currentSpeed = rb.velocity.magnitude;
        lastSpeed = rb.velocity;
        lastSpeedMagnitude = lastSpeed.magnitude;
        CmdSetLastSpeed(lastSpeed);
        CmdSetSpeedMagnitude(lastSpeedMagnitude);
        if (state != currentState)
        {
            CmdSetState(currentState);
        }
    }

    protected override void FSMFixedUpdate()
    {
        Rendering();
        Move();
        if (!isLocalPlayer) 
        {
            if (shielded > (float)NetworkTime.time)
            {
                if (health > 15)
                {
                    fireVFX.SendEvent("StopFire");
                }
                if (health > 40)
                {
                    fireVFX.SendEvent("StopSmoke");
                }
                health = health + (maxHealth / 150);
                if (health > maxHealth) { health = maxHealth; }
                playerHealthBar.SetHealth((maxHealth - health) / maxHealth);
                healthBar.SetHealth((maxHealth - health) / maxHealth);
            }
            return;
        }
        if (shielded > (float)NetworkTime.time)
        {
            if (health > 15)
            {
                fireVFX.SendEvent("StopFire");
            }
            if (health > 40)
            {
                fireVFX.SendEvent("StopSmoke");
            }
            health = health + (maxHealth / 150);
            if (health > maxHealth) { health = maxHealth; }
            playerHealthBar.SetHealth((maxHealth - health) / maxHealth);
            healthBar.SetHealth((maxHealth - health) / maxHealth);
            wasShielded = true;
        }
        else if (wasShielded)
        {
            wasShielded = false;
            CmdChangeHealth(health);
        }
    }

    private void CheckStateRemote()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (healthBar.GetHealth() < 1 && health == maxHealth)
        {
            healthBar.SetHealth(0);
            Debug.Log("Fixing");
        }
        if (health > 40 && smoking) 
        { 
            fireVFX.SendEvent("StopSmoke");
            smoking = false;
        }
        if (health > 15 && onFire) 
        { 
            fireVFX.SendEvent("StopFire");
            onFire = false;
        }
        if (health <= 40 && !smoking)
        {
            fireVFX.SendEvent("StartSmoke");
            smoking = true;
        }
        if (health <= 15 && !onFire)
        {
            fireVFX.SendEvent("StartFire");
            onFire = true;
        }
    }

    private void CheckState()
    {
        if (currentState == PlayerStates.Finish)
        {
            SetLayer(13);

            if (isLocalPlayer && (playerInterface.enabled == true || GameStateManager.GetInstance().gameState == GameStateManager.GameState.GameOver))
            {
                playerInterface.enabled = false;
                minimap.enabled = false;
                endgameInterface.enabled = true;
                endgamePlacementImage.sprite = Resources.Load<Sprite>("UI/placement" + endgameManager.GetPlacement(usernameText));
            }
        }
        else if (currentState == PlayerStates.Starting)
        {
            int currentCountdown = Mathf.FloorToInt(raceCountdown - (float)NetworkTime.time);
            if (currentCountdown == 4)
            {
                CmdStartCountdown();
            }
            if (accelerating && currentCountdown < 4)
            {
                initialBoost += 1 * Time.deltaTime;
            }
            if (currentCountdown > 0 && currentCountdown < 4)
            {
                if (startingText.text == "READY?")
                {
                    courseName.gameObject.SetActive(false);
                    PlaySound("RaceStart");
                }
                startingText.text = currentCountdown.ToString();
            }
            else if (currentCountdown == 0)
            {
                startingText.text = "GO!";
                startingCanvas.GetComponentInChildren<Image>().enabled = false;
                currentState = PlayerStates.Grounded;
                if (initialBoost < 0.05f && initialBoost != 0)
                {
                    Boost(direction, 80, 2);
                }
                else if (initialBoost != 0 && initialBoost < 1)
                {
                    Boost(direction, Mathf.Lerp(60, 0, initialBoost));
                    Debug.Log(Mathf.Lerp(60, 0, initialBoost));
                }
                if (GameStateManager.GetInstance().gameState != GameStateManager.GameState.Running)
                {
                    GameStateManager.GetInstance().CmdSetState(GameStateManager.GameState.Running);
                }
                if (dataManager.loggedIn && isLocalPlayer)
                {
                    StartCoroutine(ServerDataManager.GetRecord(DataManager.userID, DataManager.levelName));
                }
                else
                {
                    StartCoroutine(dataManager.GetRecord());
                }
                StartCoroutine(CloseStartingScreen());
            }
        }
        else
        {
            if (currentState == PlayerStates.Jumping && currentAirborne <= 1 && airborne <= 0/* && !CheckOffRoadUp(new Vector2(transform.position.x, transform.position.y), 0)*/)
            {
                if (grounded > 0)
                {
                    if (isLocalPlayer)
                    {
                        HapticsManager.instance.RumbleLinear(0.3f, 0.3f, 0.1f);
                    }
                    vehicleAudioManager.Play("RollingEnd");
                }
                currentState = PlayerStates.Grounded;
            }
            if (currentState == PlayerStates.Grounded && grounded <= 0 && !fallProtection)
            {
                CmdChangeHealth(0);
                HapticsManager.instance.RumbleLinear(1, 1, 1f);
                PlaySound("Fall");
            }
            if (health <= 0f && currentState != PlayerStates.Dead)
            {
                currentState = PlayerStates.Dead;
                dead = Time.time + deathTimer;
                if (spawnRotation == transform.eulerAngles.z)
                {
                    transform.eulerAngles = new Vector3(0, 0, spawnRotation + 100);
                }
                fallProtection = true;
            }
            else if (health > 0f && currentState == PlayerStates.Dead)
            {
                currentState = PlayerStates.Grounded;
            }
            if (currentState == PlayerStates.Dead && dead < Time.time)
            {
                Restart();
                fireVFX.SendEvent("StopFire");
                fireVFX.SendEvent("StopSmoke");
            }
            if (username.text != usernameText)
            {
                username.text = usernameText;
            }
            if (currentMaxSpeed < maxSpeed && slowTimer < Time.time)
            {
                currentMaxSpeed = maxSpeed;
            }
            if (currentState == PlayerStates.Dead)
            {
                SetLayer(10);
            }
            if (firingLaser && laserCountdown < NetworkTime.time)
            {
                CmdLaserHit(transform.position + new Vector3(direction.x, direction.y, 0) * 2, direction, laserPosition.position, transform.rotation, usernameText);
                firingLaser = false;
                lineRenderer.enabled = false;
                CmdUseItem();
                if (isLocalPlayer)
                {
                    HapticsManager.instance.Rumble(0.6f, 0.4f, 0.1f);
                }
                currentItem = Items.None;
                itemSprite.sprite = none;
            }
            if (airborne > 0 && currentState == PlayerStates.Grounded && !fallProtection)
            {
                currentState = PlayerStates.Jumping;
            }
        }
    }

    [Command]
    private void CmdStartCountdown()
    {
        RpcStartCountdown();
    }

    [ClientRpc]
    private void RpcStartCountdown()
    {
        raceCountdown = 3.9f + (float)NetworkTime.time;
    }

    private void Rendering()
    {
        if (!isLocalPlayer)
        {

            sr.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
        if (shielded > (float)NetworkTime.time)
        {
            shieldRenderer.enabled = !shieldRenderer.enabled;
        }
        else if (shieldRenderer.enabled)
        {
            shieldRenderer.enabled = false;
        }
        if (currentItem == Items.Laser && laserCountdown < NetworkTime.time)
        {
            lineRenderer.enabled = true;
        }
        if (firingLaser && laserCountdown > NetworkTime.time)
        {
            lineRenderer.enabled = !lineRenderer.enabled;
        } else if (currentItem != Items.Laser)
        {
            lineRenderer.enabled = false;
        }
        if (isLocalPlayer && GameStateManager.GetInstance().gameState == GameStateManager.GameState.Running)
        {
            if (!lapTextRenderer.activeSelf)
            {
                lapTextRenderer.SetActive(true);
            }
            if (!placementNumber.gameObject.activeSelf)
            {
                placementNumber.gameObject.SetActive(true);
            }
            enemyInterface.enabled = false;
        }
        if (isLocalPlayer && GameStateManager.GetInstance().gameState != GameStateManager.GameState.Running)
        {
            if (lapTextRenderer.activeSelf)
            {
                lapTextRenderer.SetActive(false);
            }
            if (placementNumber.gameObject.activeSelf)
            {
                placementNumber.gameObject.SetActive(false);
            }
            enemyInterface.enabled = false;
        }
        if (spectating && SceneManager.GetActiveScene().name != "Lobby")
        {
            enemyInterface.enabled = false;
            sr.enabled = false;
        }
        if (rolling && currentState == PlayerStates.Grounded)
        {
            rollingArrow.SetActive(true);
        }
        else
        {
            rollingArrow.SetActive(false);
        }
        if (currentItem == Items.None && !gettingItem)
        {
            itemSprite.sprite = none;
        }
        if (strafingLeft || strafingRight)
        {
            sr.transform.rotation = Quaternion.Euler(Mathf.Cos(rb.rotation * Mathf.Deg2Rad) * (40 * strafeValue), Mathf.Sin(rb.rotation * Mathf.Deg2Rad) * (40 * strafeValue), transform.rotation.eulerAngles.z);
        }
        else
        {
            sr.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }

        sr.sprite = Resources.Load<Sprite>("Vehicles/Vehicle" + this.spriteValue);
        vehicle.SetColor();

        velocityDisplay.text = ((int)Mathf.Floor(currentSpeed * 10)).ToString();
    }

    public void FollowPlayer()
    {
        enemyInterface.gameObject.SetActive(true);
        enemyInterface.enabled = true;
        playerInterface.gameObject.SetActive(true);
        minimap.gameObject.SetActive(true);
        lapTextRenderer.SetActive(true);
    }
    public void UnfollowPlayer()
    {
        playerInterface.gameObject.SetActive(false);
        minimap.gameObject.SetActive(false);
        lapTextRenderer.SetActive(false);
    }

    public void ToggleResults()
    {
        endgameInterface.enabled = !endgameInterface.enabled;
    }

    public void Spectate(bool spectate)
    {
        if (spectate)
        {
            enemyInterface.gameObject.SetActive(false);
            if (isLocalPlayer && SceneManager.GetActiveScene().name != "Lobby")
            {
                currentState = PlayerStates.Finish;
                timer = "";
                StartCoroutine(CloseStartingScreen(false));
                endgameInterface.gameObject.SetActive(false);
                sr.gameObject.SetActive(false);
                CmdSetTag("Spectator");
            }
        }
        else
        {
            currentState = PlayerStates.Grounded;
            enemyInterface.gameObject.SetActive(true);
            CmdSetTag("Player");
        }
    }

    private void SetPlacementImage()
    {
        placementNumber.sprite = Resources.Load<Sprite>("UI/placement" + placement);
        endgamePlacementImage.sprite = Resources.Load<Sprite>("UI/placement" + placement);
    }

    public void Restart()
    {
        transform.position = new Vector3(spawnPos.x, spawnPos.y, -5);
        transform.eulerAngles = new Vector3(0, 0, spawnRotation);
        CmdSetUsername(DataManager.username);
        CmdSetName(usernameText);
        SetStats(DataManager.MAX_SPEED + dataManager.speed, DataManager.ACCELERATION + dataManager.acceleration, DataManager.WEIGHT + dataManager.weight, DataManager.HANDLING + dataManager.handling, dataManager.spriteValue);
        CmdSetColor(DataManager.GetInstance().speed, DataManager.GetInstance().acceleration, DataManager.GetInstance().weight, DataManager.GetInstance().handling);
        //healthBar.value = 0f;
        playerHealthBar.SetHealth(0);
        healthBar.SetHealth(0);
        CmdChangeHealth(maxHealth);
        rb.velocity = Vector3.zero;
        rb.rotation = 90;
        sr.transform.localScale = Vector3.one;
        airborne = 0;
        currentAirborne = 1;
        CmdSetScale(sr.transform.localScale.x);
        xboxController = DataManager.GetInstance().xboxController;
        SetPlacementImage();
        SetLayer(6);
        currentRotationSpeed = rotationSpeed;
        fallProtection = true;
        fireVFX.SendEvent("StopSmoke");
        fireVFX.SendEvent("StopFire");
        if (isLocalPlayer)
        {
            PostProcessingManager.instance.ClearPostProcess();
        }
        StartCoroutine(FallProtection());
    }

    IEnumerator FallProtection()
    {
        yield return new WaitForSeconds(0.5f);
        fallProtection = false;
    }

    private void OnHealthChange(float oldHealth, float newHealth)
    {
        healthBar.SetHealth((maxHealth - newHealth) / maxHealth);
        playerHealthBar.SetHealth((maxHealth - newHealth) / maxHealth);
        if (oldHealth > newHealth)
        {
            if (newHealth > 40)
            {
                fireVFX.SendEvent("StopSmoke");
                fireVFX.SendEvent("StopFire");
                smoking = false;
                onFire = false;
            }
            else if (newHealth > 15 && newHealth <= 40)
            {
                fireVFX.SendEvent("StopFire");
                onFire = false;
            }
        }

        else
        {
            if (newHealth > 15 && newHealth <= 40)
            {
                fireVFX.SendEvent("StartSmoke");
                smoking = true;
            }
            else if (newHealth > 0 && newHealth <= 15)
            {
                fireVFX.SendEvent("StopSmoke");
                fireVFX.SendEvent("StartSmoke");
                fireVFX.SendEvent("StartFire");
                smoking = true;
                onFire = true;
            }
        }
    }

    private void OnMaxHealthChange(float oldHealth, float newHealth)
    {
        maxHealth = newHealth;
    }

    private void OnUsernameChange(string oldName, string newName)
    {
        username.text = newName;
    }

    private void OnAirborneChange(float oldAirborne, float newAirborne)
    {
        currentScale = newAirborne;
        currentAirborne = newAirborne;
        //sr.transform.localScale = new Vector3(newAirborne, newAirborne, sr.transform.localScale.z);
    }

    private void OnShieldChange(float oldShield, float newShield)
    {
        shielded = (float)NetworkTime.time + shieldDuration;
    }

    private void OnFiringLaserChange(bool oldBool, bool newBool)
    {
        firingLaser = newBool;
    }

    private void OnLaserCountdownChange(float oldCountdown, float newCountdown)
    {
        laserCountdown = newCountdown;
    }

    private void OnLayerChange(int oldLayer, int newLayer)
    {
        gameObject.layer = newLayer;
    }

    private void OnSpectatingChange(bool oldValue, bool newValue)
    {
        spectating = newValue;
    }

    private void OnMaxSpeedChange(float oldSpeed, float newSpeed)
    {
        maxSpeed = newSpeed;
    }

    private void OnAccelerationChange(float oldAccel, float newAccel)
    {
        acceleration = newAccel;
    }

    private void OnWeightChange(float oldWeight, float newWeight)
    {
        weight = newWeight;
    }

    private void OnHandlingChange(float oldHandling, float newHandling)
    {
        rotationSpeed = newHandling;
    }

    private void OnSpriteChange(int oldValue, int newValue)
    {
        spriteValue = newValue;
    }

    private void OnColorChange(Color oldColor, Color newColor)
    {
        vehicleColor = newColor;
    }

    private void OnLapChange(int oldLap, int newLap)
    {
        currentLap = newLap;
    }
    private void OnCheckpointChange(int oldCheckpoint, int newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    private void OnPlacementChange(int oldPlacement, int newPlacement)
    {
        placement = newPlacement;
    }

    private void LastSpeedVectorChange(Vector2 oldSpeed, Vector2 newSpeed)
    {
        lastSpeed = newSpeed;
    }

    private void LastSpeedChange(float oldSpeed, float newSpeed)
    {
        lastSpeedMagnitude = newSpeed;
    }

    private void OnRollingChange(bool oldRolling, bool newRolling)
    {
        rollingSync = newRolling;
    }

    private void OnStateChange(PlayerStates oldState, PlayerStates newState)
    {
        currentState = newState;
    }

    private void OnBoostChange(float oldBoost, float newBoost)
    {
        boostTime = newBoost;
    }

    private void OnTimerChange(string oldTimer, string newTimer)
    {
        timer = newTimer;
    }

    private void OnItemChange(Items oldItem, Items newItem)
    {
        switch (newItem)
        {
            case Items.None:
                itemSprite.sprite = none;
                playerItemSprite.sprite = none;
                currentItem = Items.None;
                break;

            case Items.Shield:
                itemSprite.sprite = shield;
                playerItemSprite.sprite = shield;
                currentItem = Items.Shield;
                break;

            case Items.Jump:
                itemSprite.sprite = jump;
                playerItemSprite.sprite = jump;
                currentItem = Items.Jump;
                break;

            case Items.Trap:
                itemSprite.sprite = trap;
                playerItemSprite.sprite = trap;
                currentItem = Items.Trap;
                break;

            case Items.Rebounder:
                itemSprite.sprite = rebounder;
                playerItemSprite.sprite = rebounder;
                currentItem = Items.Rebounder;
                break;

            case Items.Laser:
                itemSprite.sprite = laser;
                playerItemSprite.sprite = laser;
                currentItem = Items.Laser;
                break;

            case Items.Boost:
                currentItem = Items.Boost;
                itemSprite.sprite = boost;
                playerItemSprite.sprite = boost;
                break;

            case Items.Missile:
                currentItem = Items.Missile;
                itemSprite.sprite = missile;
                playerItemSprite.sprite = missile;
                break;

            case Items.Shockwave:
                currentItem = Items.Shockwave;
                itemSprite.sprite = shockwave;
                playerItemSprite.sprite = shockwave;
                break;

            case Items.Flash:
                currentItem = Items.Flash;
                itemSprite.sprite = flash;
                playerItemSprite.sprite = flash;
                break;
        }
    }

    private void CheckInput()
    {
        if (pauseMenu.IsActive() || consoleMenu.IsActive() || DataManager.GetInstance().paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || (xboxController ? Input.GetKeyDown(KeyCode.Joystick1Button7) : Input.GetKeyDown(KeyCode.Joystick1Button8)))
            {
                pauseMenu.Hide();
            }
        }
        else
        {
            if (xboxController)
            {

                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
                {
                    pauseMenu.Show();
                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button6))
                {
                    //Restart();
                }
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Accel"))
                {
                    accelerating = true;
                }
                else
                {
                    accelerating = false;
                }

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton1))
                {
                    braking = true;
                }
                else
                {
                    braking = false;
                }

                if (accelerating && braking)
                {
                    if (!rolling && currentState == PlayerStates.Grounded)
                    {
                        rolling = true;
                        rb.velocity /= 2;
                        airborneVelocity /= 2;
                        iceVelocity /= 2;
                        if (isLocalPlayer)
                        {
                            PostProcessingManager.instance.Roll(true);
                        }
                        PostProcessingManager.instance.Roll(true);
                        StartCoroutine(RollingBeep());
                    }
                }
                else
                {
                    if (rolling)
                    {
                        rolling = false;
                        rollingEnded = true;
                        rb.velocity *= 2;
                        airborneVelocity *= 2;
                        iceVelocity *= 2;
                        if (currentState == PlayerStates.Grounded)
                        {
                            if (isLocalPlayer)
                            {
                                HapticsManager.instance.Rumble(0.1f, 0.4f, 0.1f);
                            }
                            vehicleAudioManager.Play("RollingEnd");
                        }
                        if (isLocalPlayer)
                        {
                            PostProcessingManager.instance.Roll(false);
                        }
                    }
                }

                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal1") < -0.2)
                {
                    rotatingLeft = true;
                }
                else
                {
                    rotatingLeft = false;
                }

                if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal1") > 0.2)
                {
                    rotatingRight = true;
                }
                else
                {
                    rotatingRight = false;
                }

                verticalValue = Input.GetKey(KeyCode.UpArrow) ? 1 : (Input.GetKey(KeyCode.DownArrow) ? -1 : Input.GetAxis("Vertical1"));

                strafeValue = Input.GetAxis("Strafe1");

                if (Input.GetKey(KeyCode.Z) || Input.GetAxis("Strafe1") < -0.1)
                {
                    strafingLeft = true;
                }
                else
                {
                    strafingLeft = false;
                }

                if (Input.GetKey(KeyCode.C) || Input.GetAxis("Strafe1") > 0.1)
                {
                    strafingRight = true;
                }
                else
                {
                    strafingRight = false;
                }

                if (Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.C))
                {
                    strafeValue = -1;
                }
                else if (Input.GetKey(KeyCode.C))
                {
                    strafeValue = 1;
                }


                if (InputManager.instance.controls.Buttons.Item.WasPressedThisFrame())
                {
                    UseItem();
                }

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button2))
                {
                    CmdPlaySound("Honk" + spriteValue);
                }

                if (InputManager.instance.controls.Rumble.RumbleAction.WasPressedThisFrame())
                {
                    if (isLocalPlayer)
                    {
                        HapticsManager.instance.Rumble(Input.GetAxis("LTrigger"), Input.GetAxis("RTrigger"), 2);
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button8))
                {
                    pauseMenu.Show();
                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button6))
                {
                    //Restart();
                }
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Joystick1Button1))
                {
                    accelerating = true;
                }
                else
                {
                    accelerating = false;
                }

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton2))
                {
                    braking = true;
                }
                else
                {
                    braking = false;
                }

                if (accelerating && braking)
                {
                    if (!rolling && currentState == PlayerStates.Grounded)
                    {
                        rolling = true;
                        rb.velocity /= 2;
                        airborneVelocity /= 2;
                        StartCoroutine(RollingBeep());
                    }
                }
                else
                {
                    if (rolling)
                    {
                        rolling = false;
                        rollingEnded = true;
                        rb.velocity *= 2;
                        airborneVelocity *= 2;
                        if (currentState == PlayerStates.Grounded)
                        {
                            if (isLocalPlayer)
                            {
                                HapticsManager.instance.Rumble(0.1f, 0.4f, 0.1f);
                            }
                            vehicleAudioManager.Play("RollingEnd");
                        }
                    }
                }

                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal1") < -0.2)
                {
                    rotatingLeft = true;
                }
                else
                {
                    rotatingLeft = false;
                }

                if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal1") > 0.2)
                {
                    rotatingRight = true;
                }
                else
                {
                    rotatingRight = false;
                }

                verticalValue = Input.GetKeyDown(KeyCode.UpArrow) ? 1 : Input.GetAxis("Vertical1");

                strafeValue = Mathf.Lerp(0, 1, (Input.GetAxis("StrafeR") + 1) / 2) - Mathf.Lerp(0, 1, (Input.GetAxis("StrafeL") + 1) / 2);

                if (strafeValue < -0.1f || Input.GetKeyDown(KeyCode.Z))
                {
                    strafingLeft = true;
                    strafingRight = false;
                }
                else if (strafeValue > 0.1f || Input.GetKeyDown(KeyCode.C))
                {
                    strafingLeft = false;
                    strafingRight = true;
                }
                else
                {
                    strafingLeft = false;
                    strafingRight = false;
                }

                if (Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.C))
                {
                    strafeValue = -1;
                }
                else if (Input.GetKey(KeyCode.C))
                {
                    strafeValue = 1;
                }

                if (InputManager.instance.controls.Buttons.Item.WasPressedThisFrame())
                {
                    UseItem();
                }

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    CmdPlaySound("Honk" + spriteValue);
                }
            }
            if (rollingSync != rolling)
            {
                CmdRolling(rolling);
            }

            if (DataManager.GetInstance().strafeMode) 
            {
                strafeValue = -strafeValue;
                if (strafingLeft || strafingRight)
                {
                    strafingLeft = !strafingLeft;
                    strafingRight = !strafingRight;
                }
            }

            if (strafingLeft || strafingRight)
            {
                AudioManager.instance.SetVolume("Drift", Mathf.Lerp(0f, DataManager.soundVolume, Mathf.Abs(strafeValue)));
                if (currentState == PlayerStates.Grounded)
                {
                    AudioManager.instance.SetPitch("Drift", Mathf.Lerp(0.2f, 2f, currentSpeed / 80));
                }
                else if (currentState == PlayerStates.Jumping)
                {
                    AudioManager.instance.SetPitch("Drift", 2.5f);
                }
                AudioManager.instance.Play("Drift");
            }
            else
            {
                AudioManager.instance.SetVolume("Drift", 0);
                AudioManager.instance.SetPitch("Drift", 0);
                AudioManager.instance.Stop("Drift");
            }
        }
    }

    private IEnumerator RollingBeep()
    {
        while (rolling)
        {
            if (currentState == PlayerStates.Grounded)
            {
                vehicleAudioManager.Play("Rolling");
            }
            yield return new WaitForSeconds(Mathf.Lerp(1f, 0.2f, currentSpeed / 40));
        }
    }

    private void Move()
    {
        direction = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)); //Iba en Grounded
        switch (currentState)
        {
            case PlayerStates.None:

                break;

            case PlayerStates.Grounded:
                if (airborneVelocity != 0) { airborneVelocity = 0; }
                SetLayer(6);
                CmdSetScale(1);
                sr.sortingLayerID = SortingLayer.NameToID("Players");
                if (rolling)
                {
                    if (currentSpeed > 1)
                    {
                        rb.AddForce(rb.velocity.normalized * -(currentDrag / 4));
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (accelerating && !braking)
                {
                    if (rb.velocity.magnitude < currentMaxSpeed)
                    {
                        rb.AddForce(direction * (slowTimer > Time.time ? acceleration / 2 : acceleration));
                    } else
                    {
                        rb.AddForce(rb.velocity.normalized * -(currentDrag / 3));
                    }
                }
                else if (braking)
                {
                    if (currentSpeed > 0.05)
                    {
                        rb.AddForce(rb.velocity.normalized * -(currentDrag * 8));
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else
                {
                    if (currentSpeed > 0.05)
                    {
                        rb.AddForce(rb.velocity.normalized * -currentDrag);
                    }
                }

                if (currentMaxSpeed < maxSpeed)
                {
                    rb.AddForce(rb.velocity.normalized * -(currentDrag * ((strafingLeft || strafingRight) ? 1 : 5)));
                }

                if (!rolling && bounceTime < Time.time)
                {

                    if (iceVelocity != 0 && bounceTime < Time.time)
                    {
                        if (rb.velocity.magnitude > iceVelocity) { iceVelocity = rb.velocity.magnitude; }
                        rb.velocity = Vector2.Lerp(rb.velocity.normalized, new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)), 0.1f * dataManager.handling + 0.1f) * iceVelocity;

                        if (strafingLeft && !strafingRight)
                        {
                            float newAngle = -Vector2.SignedAngle(rb.velocity, Vector2.right) - ((4 * dataManager.handling + 2) * strafeValue);
                            Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                            rb.velocity = tempDirection * iceVelocity;
                            //rb.AddForce(tempDirection * 30 * strafeValue * rotationSpeed);
                        }
                        else if (strafingRight && !strafingLeft)
                        {
                            float newAngle = -Vector2.SignedAngle(rb.velocity, Vector2.right) - ((4 * dataManager.handling + 2) * strafeValue);
                            Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                            rb.velocity = tempDirection * iceVelocity;
                            //rb.AddForce(tempDirection * 30 * strafeValue * rotationSpeed);
                        }
                        if (rollingEnded)
                        {
                            rb.velocity = direction * iceVelocity;
                        }
                    }
                    else if (strafingLeft || strafingRight)
                    {
                        currentMaxSpeed = (rotatingLeft || rotatingRight ? maxSpeed + (15 * rotationSpeed) : maxSpeed);
                        float newAngle = rb.rotation - (strafingAngle * strafeValue);
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;

                    }
                    else
                    {
                        currentMaxSpeed = maxSpeed;
                        rb.velocity = direction * rb.velocity.magnitude;
                    }
                    if (wasBouncing && iceVelocity != 0)
                    {
                        rb.velocity = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)) * iceVelocity;
                        wasBouncing = false;
                    }
                }
                else if (bounceTime > Time.time)
                {
                    wasBouncing = true;
                    if (currentSpeed > 1)
                    {
                        rb.AddForce(rb.velocity.normalized * -currentDrag * 4);
                    }
                }

                if (rotatingLeft && !rotatingRight)
                {
                    rb.rotation += (rolling ? 6f : ((3f + -strafeValue * 2f)) * currentRotationSpeed * (Input.GetKey(KeyCode.LeftArrow) ? 1 : Mathf.Abs(Input.GetAxis("Horizontal1"))));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 6f : ((3f + strafeValue * 2f)) * currentRotationSpeed * (Input.GetKey(KeyCode.RightArrow) ? 1 : Mathf.Abs(Input.GetAxis("Horizontal1"))));
                }
                break;

            case PlayerStates.Jumping:
                if (airborneVelocity == 0)
                {
                    airborneVelocity = rb.velocity.magnitude;
                }
                currentAirborne = sr.transform.localScale.x;
                if (airborne > 0)
                {
                    airborne -= (braking ? gravity * 2 : gravity);
                    sr.transform.localScale = new Vector3(currentAirborne + airborne, currentAirborne + airborne, 1);
                    CmdSetScale(sr.transform.localScale.x);
                    //CmdSetScale(sr.transform.localScale);
                }
                else if (airborne < 0 && currentAirborne + airborne > 1)
                {
                    airborne -= (braking ? gravity * 3 : gravity);
                    sr.transform.localScale = new Vector3(currentAirborne + airborne, currentAirborne + airborne, 1);
                    CmdSetScale(sr.transform.localScale.x);
                }
                else if (airborne < 0 && currentAirborne + airborne < 1)
                {
                    sr.transform.localScale = new Vector3(1, 1, 1);
                }
                if (rotatingLeft && !rotatingRight)
                {
                    rb.rotation += (rolling ? 4f : (3f - strafeValue * 2f)) * currentRotationSpeed * (Input.GetKey(KeyCode.LeftArrow) ? 1 : Mathf.Abs(Input.GetAxis("Horizontal1")));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 4f : (3f + strafeValue * 2f)) * currentRotationSpeed * (Input.GetKey(KeyCode.RightArrow) ? 1 : Mathf.Abs(Input.GetAxis("Horizontal1")));
                }

                rb.velocity = Vector2.Lerp(rb.velocity.normalized, new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)), 0.05f * dataManager.handling + 0.02f) * airborneVelocity;

                        if (strafingLeft && !strafingRight)
                        {
                            float newAngle = -Vector2.SignedAngle(rb.velocity, Vector2.right) - ((2.5f * dataManager.handling + 0.5f) * strafeValue);
                            Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                            rb.velocity = tempDirection * airborneVelocity;
                            //rb.AddForce(tempDirection * 30 * strafeValue * rotationSpeed);
                        }
                        else if (strafingRight && !strafingLeft)
                        {
                            float newAngle = -Vector2.SignedAngle(rb.velocity, Vector2.right) - ((2.5f * dataManager.handling + 0.5f) * strafeValue);
                            Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                            rb.velocity = tempDirection * airborneVelocity;
                            //rb.AddForce(tempDirection * 30 * strafeValue * rotationSpeed);
                        }

                break;

            case PlayerStates.Dead:
                if (sr.transform.localScale.x > 0)
                {
                    sr.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
                    CmdSetScale(sr.transform.localScale.x);
                    CmdChangeHealth(0);
                }
                SetLayer(10);
                if (currentSpeed > 0.05)
                {
                    rb.AddForce(rb.velocity.normalized * (-currentDrag * 2));
                }
                break;

            case PlayerStates.Finish:
                if (currentSpeed > 1)
                {
                    rb.AddForce(rb.velocity.normalized * -(currentDrag * 3));
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
                break;
        }
        if (rollingEnded)
        {
            rollingEnded = false;
        }
    }

    private void SetLap()
    {
        if (lapManager == null)
        {
            lapManager = GameObject.FindGameObjectWithTag("LapManager").GetComponent<LapManager>();
        }
        lapRenderer.text = string.Format("{0}/{1}", currentLap < lapManager.lapCount ? currentLap : lapManager.lapCount, lapManager.lapCount);
    }

    private IEnumerator CloseStartingScreen()
    {
        yield return new WaitForSeconds(1);
        startingCanvas.enabled = false;
        MusicManager.instance.Play(FindObjectOfType<LevelData>().GetWorld());
    }

    private IEnumerator CloseStartingScreen(bool wait)
    {
        yield return new WaitForSeconds(wait ? 0 : 1);
        startingCanvas.enabled = false;
        MusicManager.instance.Play(FindObjectOfType<LevelData>().GetWorld());
    }

    public void StartRace()
    {
        raceCountdown = (float)NetworkTime.time + 8;
        startingText.text = "READY?";
    }

    [Command]
    private void CmdStartRace()
    {
        if (GameStateManager.GetInstance().gameState == GameStateManager.GameState.Starting)
        {
            RpcStartRace();
        }
    }

    [ClientRpc]
    private void RpcStartRace()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<MultiplayerController>().StartRace();
        }
    }

    [Command]
    private void CmdSetLap(int lap, int placement)
    {
        currentLap = lap;
        if (lapManager == null)
        {
            lapManager = GameObject.FindGameObjectWithTag("LapManager").GetComponent<LapManager>();
        }
        if (currentLap > lapManager.lapCount && currentState != PlayerStates.Finish)
        {
            currentState = PlayerStates.Finish;
            PlaySound("RaceEnd");
            RpcEndRace();
        }
    }

    [ClientRpc]
    private void RpcEndRace()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gameObjects)
        {
            GetComponent<Timer>().Stop(isLocalPlayer);
            timer = GetComponent<Timer>().GetTime();
            Debug.Log(timer);
            go.GetComponent<MultiplayerController>().endgameManager.AddDictionaryEntry(placement, gameObject);
        }
    }

    [Command]
    private void CmdSetSpeedMagnitude(float speed)
    {
        lastSpeedMagnitude = speed;
    }

    [Command]
    private void CmdRolling(bool rolling)
    {
        rollingSync = rolling;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetTag(string tag)
    {
        RpcSetTag(tag);
    }

    [ClientRpc]
    private void RpcSetTag(string tag)
    {
        gameObject.tag = tag;
        spectating = true;
    }

    [Command]
    private void CmdSetUsername(string name)
    {
        RpcSetUsername(name);
    }

    [ClientRpc]
    private void RpcSetUsername(string name)
    {
        usernameText = name;
        username.text = name;
    }

    private void Jump(bool item)
    {
        if (currentState == PlayerStates.Grounded)
        {
            currentState = PlayerStates.Jumping;
            airborne = jumpHeight;
            SetLayer(8);
            sr.sortingLayerID = SortingLayer.NameToID("Foreground");
            if (isLocalPlayer)
            {
                HapticsManager.instance.RumbleLinear(0.1f, 0.2f, 0.5f);
            }
            CmdPlaySound("Jump" + Random.Range(1, 4));
        } else if (currentState == PlayerStates.Jumping && item)
        {
            if (airborne < 0) { airborne = 0; }
            airborne += jumpHeight;
            CmdPlaySound("Jump" + Random.Range(1, 4));
        }
    }

    private void Boost(Vector2 direction, float force)
    {
        bounceTime = Time.time;
        if (rolling) { force /= 2; }
        if (rb.velocity.magnitude < (rolling ? 20 : 40))
        {
            rb.velocity = direction * (rolling ? 30 : 60);
        }
        else
        {
            rb.velocity = direction * force + rb.velocity;
        }
        if (rb.velocity.magnitude > 130)
        {
            rb.velocity = direction * 130;
        }
        if (airborneVelocity > 0)
        {
            airborneVelocity = rb.velocity.magnitude;
        }
        else if (iceVelocity > 0)
        {
            iceVelocity = rb.velocity.magnitude;
        }
        if (isLocalPlayer)
        {
            HapticsManager.instance.RumbleLinear(0.2f, 0.4f, 1f);
            PostProcessingManager.instance.Boost(rb.velocity.magnitude);
        }
        PlaySound("Boost");
    }

    private void Boost(Vector2 direction, float force, float duration)
    {
        boostTime = (float)NetworkTime.time + duration;
        CmdBoost(boostTime);
        Boost(direction, force);
    }

    private void Boost(Vector2 direction, float force, float duration, bool setSpeed)
    {
        if (setSpeed)
        {
            if (rolling) { force /= 2; }
            if (rb.velocity.magnitude < force)
            {
                rb.velocity = direction * force;
            }
            boostTime = (float)NetworkTime.time + duration;
            CmdBoost(boostTime);
            if (isLocalPlayer)
            {
                HapticsManager.instance.RumbleLinear(0.2f, 0.4f, 1f);
                PostProcessingManager.instance.Boost(rb.velocity.magnitude);
            }
            PlaySound("Boost");
        }
        else
        {
            Boost(direction, force, duration);
        }
    }

    [Command]
    private void CmdPlaySound(string sound)
    {
        RpcPlaySound(sound);
    }

    [ClientRpc]
    private void RpcPlaySound(string sound)
    {
        vehicleAudioManager.Play(sound);
    }

    [Command]
    private void CmdStopSound(string sound)
    {
        RpcStopSound(sound);
    }

    [ClientRpc]
    private void RpcStopSound(string sound)
    {
        vehicleAudioManager.Stop(sound);
    }

    private void SetStats(float speed, float accel, float weight, float handling, int spriteValue)
    {
        if (currentState != PlayerStates.Starting)
        {
            currentState = PlayerStates.Grounded;
        }
        this.maxSpeed = (speed * 1.9f);
        this.acceleration = accel;
        this.weight = weight;
        this.rotationSpeed = handling + 0.2f;
        this.spriteValue = spriteValue;

        maxHealth = 20 + weight * 40;

        health = maxHealth;
        CmdSetStats(speed, accel, weight, handling, spriteValue);
    }

    [Command]
    private void CmdSetStats(float speed, float accel, float weight, float handling, int spriteValue)
    {
        if (currentState != PlayerStates.Starting)
        {
            currentState = PlayerStates.Grounded;
        }
        this.maxSpeed = (speed * 1.9f);
        this.acceleration = accel;
        this.weight = weight;
        this.rotationSpeed = handling + 0.2f;
        this.spriteValue = spriteValue;

        maxHealth = 20 + weight * 40;

        health = maxHealth;
        CmdChangeHealth(maxHealth);
    }

    [Command]
    private void CmdSetColor(float speed, float acceleration, float weight, float handling)
    {
        float tempWeight = Mathf.Round(weight * 5);
        float tempHandling = Mathf.Round(handling * 10);

        if (speed > acceleration)
        {
            if (speed > tempWeight)
            {
                if (speed > tempHandling)
                {
                    vehicleColor = Color.red;
                }
                else if (speed < tempHandling)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = Color.magenta;
                }
            }
            else if (tempWeight > speed)
            {
                if (tempWeight > tempHandling)
                {
                    vehicleColor = Color.yellow;
                }
                else if (tempHandling > tempWeight)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(0.45f, 0.9f, 0.45f, 1);
                }
            }
            else
            {
                if (tempHandling > tempWeight)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(0.6f, 0f, 1f, 1);
                }
            }
        }
        else if (acceleration > speed)
        {
            if (acceleration > tempWeight)
            {
                if (acceleration > tempHandling)
                {
                    vehicleColor = Color.green;
                }
                else if (tempHandling > acceleration)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(0, 1, 0.7f, 1);
                }
            }
            else if (acceleration < tempWeight)
            {
                if (tempWeight > tempHandling)
                {
                    vehicleColor = Color.yellow;
                }
                else if (tempHandling > tempWeight)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(0.45f, 0.9f, 0.45f, 1);
                }
            }
            else
            {
                if (tempHandling > tempWeight)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(1f, 0.5f, 0.5f, 1);
                }
            }
        }
        else
        {
            if (speed > tempWeight)
            {
                if (speed > tempHandling)
                {
                    vehicleColor = new Color(1f, 0.6f, 0, 1);
                }
                else if (speed < tempHandling)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(0.5f, 0.9f, 0.9f, 1);
                }
            }
            else if (speed < tempWeight)
            {
                if (tempWeight > tempHandling)
                {
                    vehicleColor = Color.yellow;
                }
                else if (tempHandling > tempWeight)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = new Color(0.45f, 0.9f, 0.45f, 1);
                }
            }
            else
            {
                if (speed > tempHandling)
                {
                    vehicleColor = new Color(0.9f, 0.5f, 0.5f, 1);
                }
                else if (speed < tempHandling)
                {
                    vehicleColor = Color.cyan;
                }
                else
                {
                    vehicleColor = Color.white;
                }
            }
        }
    }

    [Command]
    private void CmdShield()
    {
        shielded = (float)NetworkTime.time + shieldDuration;
    }

    [Command]
    private void CmdTrap(Vector2 pos, PlayerStates state, float airborne, bool thrown, Vector2 velocity)
    {
        RpcTrap(pos, state, airborne, thrown, velocity);
    }

    [ClientRpc]
    private void RpcTrap(Vector2 pos, PlayerStates state, float airborne, bool thrown, Vector2 velocity)
    {
        GameObject trp = Instantiate(trapPrefab, pos, Quaternion.identity);
        trp.GetComponent<Trap>().Initialize(state, airborne, thrown, velocity, bc, usernameText);
    }

    [Command]
    private void CmdRebounder(Vector3 pos, Vector2 initialVelocity, PlayerStates state, float currentScale, float airborne)
    {
        RpcRebounder(pos, initialVelocity, state, currentScale, airborne);
    }

    [ClientRpc]
    private void RpcRebounder(Vector3 pos, Vector2 initialVelocity, PlayerStates state, float currentScale, float airborne)
    {
        GameObject reb = Instantiate(rebounderPrefab, pos, Quaternion.identity);
        reb.GetComponent<Rebounder>().InitializeRebounder(initialVelocity, state, currentScale, airborne, bc, usernameText);
    }

    [Command]
    private void CmdRebounderHit(Vector3 pos1, Vector3 pos2, Vector3 contactPoint, float velocity)
    {
        RpcRebounderHit(pos1, pos2, contactPoint, velocity);
    }

    [ClientRpc]
    private void RpcRebounderHit(Vector3 pos1, Vector3 pos2, Vector3 contactPoint, float velocity)
    {
        VFXManager.instance.Rebounder(pos1, pos2, contactPoint, velocity);
        CmdPlaySound("ItemHit");
    }

    [Command]
    private void CmdShockwave(string username)
    {
        RpcShockwave(username);
    }

    [ClientRpc]
    private void RpcShockwave(string username)
    {
        StartCoroutine(Shockwave(username));
    }

    private IEnumerator Shockwave(string username)
    {
        for (int i = 3; i >= 0; i--)
        {
            GameObject reb = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            reb.GetComponent<Shockwave>().InitializeShockwave(bc, username);
            if (isLocalPlayer)
            {
                HapticsManager.instance.Rumble(0.5f, 0.3f, 0.05f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    [Command]
    private void CmdEqualizer(GameObject parentPlayer, string username)
    {
        RpcEqualizer(parentPlayer, Random.Range(10, 50), username);
    }

    [ClientRpc]
    private void RpcEqualizer(GameObject parentPlayer, int ranVal, string username)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != parentPlayer)
            {
                GameObject eq = Instantiate(equalizerPrefab, player.transform.position, Quaternion.identity);
                eq.GetComponent<Equalizer>().Initialize(player, ranVal, username);
            }
        }
    }

    #region LASER

    [Command]
    private void CmdLaser()
    {
        laserCountdown = (float)NetworkTime.time + 0.3f;
        firingLaser = true;
    }

    [Command]
    private void CmdLaserHit(Vector3 pos, Vector2 dir, Vector3 laserPos, Quaternion laserRotation, string username)
    {
        float distance = 10000;
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 10000, layerMask);
        if (Physics2D.Raycast(pos, dir, 10000, layerMask))
        {
            RpcLaserHit(hit.collider.gameObject, username);
            distance = hit.distance * 10;
        }
        RpcShootLaser(laserPos, laserRotation, distance);
        firingLaser = false;
    }

    [ClientRpc]
    private void RpcLaserHit(GameObject hit, string username)
    {
        if (NetworkClient.localPlayer.gameObject == hit)
        {
            hit.GetComponent<MultiplayerController>().Hit(90, 20, 4, username, Items.Laser);
            CmdPlaySound("LaserHit");
        }
    }

    [ClientRpc]
    private void RpcShootLaser(Vector3 laserPos, Quaternion rotation, float distance)
    {
        GameObject l = Instantiate(laserVFX, laserPos, rotation);
        l.GetComponent<VisualEffect>().SetFloat("Length", distance);
        l.GetComponent<VisualEffect>().SetFloat("Position", distance / 10);
        l.GetComponent<DestroyDelay>().DestroyAfterDelay();
    }

    private void LaserUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(direction.x, direction.y, 0) * 2, direction, 10000, layerMask);
        Vector2 position;
        if (hit.collider != null)
        {
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.green;
            position = hit.point;
            CmdSpawnLaserPointer(position);
            CmdLaserUpdate(firingLaser, true, transform.position, position);
        }
        else
        {
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.red;
            position = transform.position + new Vector3(direction.x, direction.y, 0) * 1000;
            CmdLaserUpdate(firingLaser, false, transform.position, position);
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, position);
    }

    [Command]
    private void CmdLaserUpdate(bool firingLaser, bool hit, Vector3 pos1, Vector3 pos2)
    {
        RpcLaserUpdate(firingLaser, hit, pos1, pos2);
    }

    [ClientRpc]
    private void RpcLaserUpdate(bool firingLaser, bool hit, Vector3 pos1, Vector3 pos2)
    {
        float countdown = laserCountdown - (float)NetworkTime.time;
        if (hit)
        {
            if (firingLaser)
            {
                laserCountdown = 0;
            }
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.green;
            lineRenderer.material.SetColor("_Color", firingLaser ? new Vector4(Mathf.Lerp(0, 0, countdown - 0.1f / 0.4f), Mathf.Lerp(150, 50, countdown - 0.1f / 0.4f), 200, 2) : new Vector4(0, 150, 0, 2));
        }
        else
        {
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.red;
            lineRenderer.material.SetColor("_Color", firingLaser ? new Vector4(Mathf.Lerp(100, 0, (countdown - 0.1f) / 0.2f), Mathf.Lerp(50, 100, (countdown - 0.1f) / 0.2f), 200, 2) : new Vector4(150, 0, 0, 2));
            if (firingLaser)
            {
                lineRenderer.widthMultiplier = Mathf.Lerp(0, 3, countdown / 0.3f);
            }
            else
            {
                lineRenderer.widthMultiplier = 1f;
            }
        }
        lineRenderer.SetPosition(0, pos1);
        lineRenderer.SetPosition(1, pos2);
    }

    [Command]
    private void CmdSpawnLaserPointer(Vector2 pos)
    {
        RpcSpawnLaserPointer(pos);
    }

    [ClientRpc]
    private void RpcSpawnLaserPointer(Vector2 pos)
    {
        Instantiate(laserPoint, pos, Quaternion.Euler(0, 0, Time.time * 100));
    }

    #endregion

    #region Flash

    private void Flash()
    {
        Vector2 parentPosition = transform.position;
        Vector2 targetPosition = flashActive.transform.position;
        RaycastHit2D[] hits = Physics2D.LinecastAll(parentPosition, targetPosition, 1 << 14);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("FinishLine"))
            {

                spawnPos = hit.collider.transform.position;
                spawnRotation = hit.collider.transform.eulerAngles.z;
                if (lapManager == null)
                {
                    lapManager = GameObject.FindGameObjectWithTag("LapManager").GetComponent<LapManager>();
                }
                if (lapManager.CheckNextLap(checkpointCount))
                {
                    currentLap++;
                    CmdSetLap(currentLap, placement);
                    if (currentLap <= lapManager.lapCount)
                    {
                        PlaySound("Lap");
                    }
                }
                checkpointCount = 0;
                CmdSetCheckpoint(0);
            }
            else if (hit.collider.CompareTag("FrontLine"))
            {
                checkpointCount = 0;
                lapManager.CheckNextLap(0);
            }
            else if (hit.collider.CompareTag("Checkpoint"))
            {
                if (hit.collider.gameObject.GetComponent<Checkpoint>().respawn)
                {
                    spawnPos = hit.collider.transform.position;
                    spawnRotation = hit.collider.transform.eulerAngles.z;
                }
                int c = hit.collider.gameObject.GetComponent<Checkpoint>().checkpointNumber;
                if (c > currentCheckpoint)
                {
                    checkpointCount++;
                    currentCheckpoint = c;
                    CmdSetCheckpoint(currentCheckpoint);
                }
                else if (c < currentCheckpoint)
                {
                    checkpointCount--;
                    currentCheckpoint = c;
                    CmdSetCheckpoint(currentCheckpoint);
                }
            }
        }
        CmdFlash(this, parentPosition, targetPosition, usernameText);
        transform.position = flashActive.transform.position;
        flashActive.GetComponent<Flash>().Destroy();
        HapticsManager.instance.Rumble(0.8f, 0.6f, 0.2f);
        CmdPlaySound("Flash" + Random.Range(1, 4));
    }

    [Command]
    private void CmdFlash(MultiplayerController parent, Vector2 parentPosition, Vector2 targetPosition, string username)
    {
        RpcFlash(parent, parentPosition, targetPosition, username);
    }

    [ClientRpc]
    private void RpcFlash(MultiplayerController parent, Vector2 parentPosition, Vector2 targetPosition, string username)
    {
        VFXManager.instance.Flash(parentPosition, targetPosition);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(targetPosition, 3, Vector3.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.GetComponent<MultiplayerController>() == localPlayer && hit.collider.GetComponent<MultiplayerController>() != parent)
            {
                hit.collider.GetComponent<MultiplayerController>().Hit(60, 20, 4, username, Items.Flash);
                hit.collider.GetComponent<MultiplayerController>().Launch(new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y) - targetPosition, 3000);
                HapticsManager.instance.Rumble(0.9f, 0.6f, 0.5f);
                return;
            }
        }

        hits = Physics2D.LinecastAll(parentPosition, targetPosition);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.GetComponent<MultiplayerController>() == localPlayer && hit.collider.GetComponent<MultiplayerController>() != parent)
            {
                hit.collider.GetComponent<MultiplayerController>().Hit(20, 10, 3, username, Items.Flash);
                HapticsManager.instance.Rumble(0.3f, 0.3f, 0.3f);
            }
        }
    }

    private void FlashUpdate()
    {
        if (flashActive == null)
        {
            flashActive = Instantiate(flashPrefab);
            flashActive.GetComponent<Flash>().Instantiate(this);
        }
    }

    #endregion

    [Command]
    private void CmdBoost(float boost)
    {
        boostTime = boost;
    }

    [Command]
    private void CmdMissile(Vector2 pos, string username)
    {
        GameObject mis = Instantiate(missilePrefab, pos, Quaternion.identity);
        mis.GetComponent<Missile>().username = username;
        NetworkServer.Spawn(mis);
    }

    public void Hit(float damage)
    {
        if (shielded > (float)NetworkTime.time || !isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                CmdPlaySound("ShieldBash");
            }
            return;
        }
        health = health - damage;
        if (health <= 0)
        {
            AudioManager.instance.Play("Death");
            if (isLocalPlayer)
            {
                HapticsManager.instance.RumbleLinear(1, 1, 1f);
                PostProcessingManager.instance.Hit(health / maxHealth);
            }
            CmdChangeHealth(0);
        }
        else
        {
            HapticsManager.instance.Rumble(Mathf.Lerp(0.4f, 1, damage / 80), Mathf.Lerp(0.4f, 1, damage / 80), Mathf.Lerp(0.1f, 1, (damage - 20) / 60));
            PostProcessingManager.instance.Hit(health / maxHealth);
            CmdChangeHealth(health);
        }
        if (health > 15 && health <= 40)
        {
            fireVFX.SendEvent("StartSmoke");
        }
        else if (health > 0 && health <= 15)
        {
            fireVFX.SendEvent("StopSmoke");
            fireVFX.SendEvent("StartSmoke");
            fireVFX.SendEvent("StartFire");
        }
        
    }

    public void Hit(float damage, float slow, float duration)
    {
        if (shielded > (float)NetworkTime.time || !isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                CmdPlaySound("ShieldBash");
            }
            return; 
        }
        Hit(damage);
        currentMaxSpeed -= slow;
        if (currentMaxSpeed < 10)
        {
            currentMaxSpeed = 10;
        }
        Debug.Log(Mathf.Lerp(duration, duration / 3, (acceleration / 10) - 1));
        slowTimer = slowTimer > Time.time + Mathf.Lerp(duration, duration / 3, (acceleration / 10) - 1) ? slowTimer : Mathf.Lerp(duration, duration / 3, (acceleration / 10) - 1);
        rb.velocity /= 3;
    }

    public void Hit(float damage, float slow, float duration, string enemyUsername, Items item)
    {
        if (shielded > (float)NetworkTime.time || !isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                CmdPlaySound("ShieldBash");
            }
            return;
        }
        CmdCallHit(enemyUsername, DataManager.username, item);
        Hit(damage, slow, duration);
    }

    [Command]
    private void CmdCallHit(string hitter, string hit, Items item)
    {
        RpcCallHit(hitter, hit, item);
    }

    [ClientRpc]
    private void RpcCallHit(string hitter, string hit, Items item)
    {
        HitManager.instance.ShowHit(hitter, hit, item);
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.AddForce(direction.normalized * force);
        bounceTime = Time.time + bounceDuration;
    }

    [Command]
    private void CmdRemoveItem(GameObject item)
    {
        NetworkServer.Destroy(item);
    }

    private void UseItem()
    {
        if (currentState != PlayerStates.Grounded && currentState != PlayerStates.Jumping) { return; }
        switch (currentItem)
        {
            case Items.None:
                return;

            case Items.Shield:
                CmdShield();
                CmdPlaySound("Shield");
                if (isLocalPlayer)
                {
                    HapticsManager.instance.RumbleLinear(0.1f, 0.5f, 0.75f);
                }
                break;

            case Items.Jump:
                Jump(true);
                break;

            case Items.Trap:
                if (verticalValue > 0.4f)
                {
                    itemPosition = transform.position + new Vector3(direction.x, direction.y, 0);
                }
                else
                {
                    itemPosition = transform.position - new Vector3(direction.x, direction.y, 1) * 3;
                }
                itemPosition = transform.position - new Vector3(direction.x, direction.y, 1) * 3;
                CmdTrap(itemPosition, currentState, currentAirborne, verticalValue > 0.5f, direction * maxSpeed + rb.velocity);
                break;

            case Items.Rebounder:
                Vector3 velocity;
                if (verticalValue < -0.4f)
                {
                    itemPosition = transform.position - new Vector3(direction.x, direction.y, 1) * 3;
                    velocity = -direction * maxSpeed + rb.velocity;
                }
                else
                {
                    itemPosition = transform.position + new Vector3(direction.x, direction.y, 0);
                    velocity = direction * maxSpeed + rb.velocity;
                }
                itemPosition = transform.position + new Vector3(direction.x, direction.y, 0);
                CmdRebounder(itemPosition, velocity, currentState, currentScale, airborne);
                break;

            case Items.Laser:
                CmdStopSound("LaserIdle");
                CmdPlaySound("LaserFire");
                CmdLaser();
                return;

            case Items.Boost:
                Boost(direction, 30, 1.5f);
                break;

            case Items.Missile:
                itemPosition = transform.position + new Vector3(direction.x, direction.y, 0) * 5;
                CmdMissile(itemPosition, usernameText);
                break;

            case Items.Shockwave:
                CmdShockwave(usernameText);
                break;

            case Items.Equalizer:
                CmdEqualizer(gameObject, usernameText);
                break;

            case Items.Flash:
                if (flashActive.GetComponent<Flash>().canUse)
                {
                    Flash();
                }
                else
                {
                    return;
                }
                break;
        }
        currentItem = Items.None;
        itemSprite.sprite = none;
        playerItemSprite.sprite = none;
        CmdUseItem();
    }

    private void SetItemSprite(int item)
    {
        switch (item)
        {
            case 0:
                playerItemSprite.sprite = none;
                break;

            case 1:
                playerItemSprite.sprite = shield;
                break;

            case 2:
                playerItemSprite.sprite = jump;
                break;

            case 3:
                playerItemSprite.sprite = trap;
                break;

            case 4:
                playerItemSprite.sprite = rebounder;
                break;

            case 5:
                playerItemSprite.sprite = laser;
                break;

            case 6:
                playerItemSprite.sprite = boost;
                break;

            case 7:
                playerItemSprite.sprite = missile;
                break;

            case 8:
                playerItemSprite.sprite = shockwave;
                break;

            case 9:
                playerItemSprite.sprite = equalizer;
                break;

            case 10:
                playerItemSprite.sprite = flash;
                break;
        }
    }

    public void GetItem(string item, bool force) 
    {
        if (currentItem != Items.None || gettingItem) { return; }
        if (force)
        {
            CmdGetItem(item);
            PlaySound("ItemGet");
            if (item == "Laser")
            {
                CmdPlaySound("LaserGet");
                CmdPlaySound("LaserIdle");
            }
            if (item == "Flash")
            {
                flashActive = Instantiate(flashPrefab);
                flashActive.GetComponent<Flash>().Instantiate(this);
            }
        }
        else
        {
            gettingItem = true;
            StartCoroutine(GetItemAnimation(item));
        }
    }

    private IEnumerator GetItemAnimation(string item)
    {
        int oldRan = 0;
        for (int i = 0; i < 10; i++)
        {
            int random = Random.Range(1, 9);
            while (random == oldRan)
            {
                random = Random.Range(1, 9);
            }
            oldRan = random;
            SetItemSprite(random);
            PlaySound("ItemTing");
            yield return new WaitForSeconds(0.6f / (10 - i));
        }
        CmdGetItem(item);
        PlaySound("ItemGet");
        if (item == "Laser")
        {
            CmdPlaySound("LaserGet");
            CmdPlaySound("LaserIdle");
        }
        if (item == "Flash")
        {
            flashActive = Instantiate(flashPrefab);
            flashActive.GetComponent<Flash>().Instantiate(this);
        }
        gettingItem = false;
    }

    private void PlaySound(string sound)
    {
        if (isLocalPlayer)
        {
            FindObjectOfType<AudioManager>().Play(sound);
        }
    }

    private void StopSound(string sound)
    {
        AudioManager.instance.Stop(sound);
    }

    private void PlaySound(string sound, bool global)
    {
        if (global || isLocalPlayer)
        {
            FindObjectOfType<AudioManager>().Play(sound);
        }
    }

    private IEnumerator RestoreCollision(Collider2D c1, Collider2D c2)
    {
        yield return new WaitForSeconds(1);
        Physics2D.IgnoreCollision(c1, c2, false);
    }

    [Command(requiresAuthority = false)]
    public void CmdGetItem(string item)
    {
        RpcGetItem(item);
    }

    [ClientRpc]
    private void RpcGetItem(string item)
    {
        if (currentItem != Items.None) { return; }
        switch (item)
        {
            case "Shield":
                currentItem = Items.Shield;
                itemSprite.sprite = shield;
                playerItemSprite.sprite = shield;
                break;

            case "Jump":
                currentItem = Items.Jump;
                itemSprite.sprite = jump;
                playerItemSprite.sprite = jump;
                break;

            case "Trap":
                currentItem = Items.Trap;
                itemSprite.sprite = trap;
                playerItemSprite.sprite = trap;
                break;

            case "Rebounder":
                currentItem = Items.Rebounder;
                itemSprite.sprite = rebounder;
                playerItemSprite.sprite = rebounder;
                break;

            case "Laser":
                currentItem = Items.Laser;
                itemSprite.sprite = laser;
                playerItemSprite.sprite = laser;
                break;

            case "Boost":
                currentItem = Items.Boost;
                itemSprite.sprite = boost;
                playerItemSprite.sprite = boost;
                break;

            case "Missile":
                currentItem = Items.Missile;
                itemSprite.sprite = missile;
                playerItemSprite.sprite = missile;
                break;

            case "Shockwave":
                currentItem = Items.Shockwave;
                itemSprite.sprite = shockwave;
                playerItemSprite.sprite = shockwave;
                break;

            case "Equalizer":
                currentItem = Items.Equalizer;
                itemSprite.sprite = equalizer;
                playerItemSprite.sprite = equalizer;
                break;

            case "Flash":
                currentItem = Items.Flash;
                itemSprite.sprite = flash;
                playerItemSprite.sprite = flash;
                break;
        }
    }

    [Command]
    private void CmdUseItem()
    {
        currentItem = Items.None;
        itemSprite.sprite = none;
        playerItemSprite.sprite = none;
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeHealth(float newHealth)
    {
        RpcChangeHealth(newHealth);
    }
    
    [ClientRpc]
    private void RpcChangeHealth(float newHealth)
    {
        if (newHealth > maxHealth) { newHealth = maxHealth; }
        if (newHealth < 0) { newHealth = 0; }
        //float oldHealth = health;
        if (health > newHealth)
        {
            if (newHealth > 40)
            {
                fireVFX.SendEvent("StopSmoke");
                fireVFX.SendEvent("StopFire");
                smoking = false;
                onFire = false;
            }
            else if (newHealth > 15 && newHealth <= 40)
            {
                fireVFX.SendEvent("StopFire");
                onFire = false;
            }
        }

        else
        {
            if (newHealth > 15 && newHealth <= 40)
            {
                fireVFX.SendEvent("StartSmoke");
                smoking = true;
            }
            else if (newHealth > 0 && newHealth <= 15)
            {
                fireVFX.SendEvent("StopSmoke");
                fireVFX.SendEvent("StartSmoke");
                fireVFX.SendEvent("StartFire");
                smoking = true;
                onFire = true;
            }
        }
        health = newHealth;
        healthBar.SetHealth((maxHealth - newHealth) / maxHealth);
        playerHealthBar.SetHealth((maxHealth - newHealth) / maxHealth);
        
    }

    [Command]
    private void CmdSetName(string name)
    {
        usernameText = name;
    }

    [Command]
    private void CmdSetScale(float scale)
    {
        currentScale = scale;
        currentAirborne = scale;
    }

    private void SetLayer(int layer)
    {
        gameObject.layer = layer;
        CmdSetLayer(layer);
    }

    [Command]
    private void CmdSetLayer(int layer)
    {
       currentLayer = layer;
    }

    [Command]
    private void CmdSetLastSpeed(Vector2 lastSpeed)
    {
        this.lastSpeed = lastSpeed;
    }

    [Command]
    private void CmdSetState(PlayerStates state)
    {
        currentState = state;
        RpcSetState(state);
    }

    [ClientRpc]
    private void RpcSetState(PlayerStates state)
    {
        currentState = state;
    }

    [Command]
    private void CmdSetPlacement(int placement)
    {
        this.placement = placement;
    }

    [Command]
    private void CmdSetCheckpoint(int checkpoint)
    {
        currentCheckpoint = checkpoint;
        RpcCheckPosition();
    }

    [ClientRpc]
    private void RpcCheckPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<MultiplayerController>().CheckPosition();
        }
    }

    public void CheckPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int tempPlacement = 1;

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            tempPlacement = 4;
        }
        else
        {
            foreach (GameObject player in players)
            {
                if (player != gameObject)
                {
                    MultiplayerController controller = player.GetComponent<MultiplayerController>();
                    if (controller.currentLap == currentLap)
                    {
                        if (controller.currentCheckpoint > currentCheckpoint)
                        {
                            tempPlacement++;
                        }
                        else if (controller.currentCheckpoint == currentCheckpoint && controller.placement < placement)
                        {
                            tempPlacement++;
                        }
                    }
                    else if (controller.currentLap > currentLap)
                    {
                        tempPlacement++;
                    }
                }
            }
        }
        placement = tempPlacement;
        if (isLocalPlayer)
        {
            CmdSetPlacement(tempPlacement);
        }
        SetPlacementImage();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer) { return; }
        if (collision.collider.tag == "Wall")
        {
            if (!rolling)
            {
                if (bounceTime > Time.time)
                {
                    bounceTime = Time.time + bounceDuration / 2;
                }
                else
                {
                    bounceTime = Time.time + bounceDuration;
                }
            }
            boostTime = 0f;
            rb.velocity = Vector2.Reflect(lastSpeed * 0.75f, collision.contacts[0].normal);
            if (rb.velocity.magnitude < 10)
            {
                rb.AddForce(collision.contacts[0].normal * 500);
            }
            Hit(10);
            VFXManager.instance.WallSparks(collision.contacts[0].point, collision.contacts[0].normal);
            vehicleAudioManager.Play("Hit" + Random.Range(1, 6));
            //rb.rotation = Vector2.SignedAngle(Vector2.right, rb.velocity);
        }
        else if (collision.collider.tag == "Bouncer")
        {
            rb.rotation = -Vector2.SignedAngle(Vector2.Reflect(lastSpeed, collision.contacts[0].normal), Vector2.right);
            rb.velocity = Vector2.Reflect(lastSpeed, collision.contacts[0].normal);
            VFXManager.instance.BounceRipple(collision.contacts[0].point, collision.contacts[0].normal);
            vehicleAudioManager.Play("Bounce" + Random.Range(1, 5));
        }
        else if (collision.collider.tag == "Player" || collision.collider.tag == "Spectator")
        {
            MultiplayerController mc = collision.gameObject.GetComponent<MultiplayerController>();
            float weightDifference = mc.weight - weight;
            if (weightDifference < 0.5f)
            {
                weightDifference = 0.5f;
            }
            float weightDifferenceEnemy = weight / mc.weight;
            if (weightDifferenceEnemy < 0.5f)
            {
                weightDifferenceEnemy = 0.5f;
            }
            if ((boostTime > NetworkTime.time && mc.boostTime > NetworkTime.time) || ((boostTime < NetworkTime.time && mc.boostTime < NetworkTime.time))) {

                if (currentState == PlayerStates.Jumping)
                {
                    bounceTime = Time.time + bounceDuration;
                    float damage = 10 + (6 * weightDifference);
                    if (health - damage <= 0)
                    {
                        Hit(damage, 0, 0, mc.usernameText, Items.Eliminator);
                    }
                    else
                    {
                        Hit(damage);
                    }
                    rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                }
                else if (currentState == PlayerStates.Grounded)
                {
                    bounceTime = Time.time + bounceDuration;
                    float damage = 5 + (3 * weightDifference);
                    if (health - damage <= 0)
                    {
                        Hit(damage, 0, 0, mc.usernameText, Items.Eliminator);
                    }
                    else
                    {
                        Hit(damage);
                    }
                    rb.AddForce(collision.contacts[0].normal * 500 * weightDifference);
                }
            }
            else
            {
                if (boostTime > NetworkTime.time)
                {
                    if (currentState == PlayerStates.Jumping)
                    {
                        float damage = 6 + (2 * weightDifference);
                        if (health - damage <= 0)
                        {
                            Hit(damage, 0, 0, mc.usernameText, Items.Eliminator);
                        }
                        else
                        {
                            Hit(damage);
                            if (mc.health - (30 + (40 * weightDifferenceEnemy)) <= 0)
                            {
                                rb.velocity = lastSpeed;
                                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), mc.GetComponent<Collider2D>(), true);
                                StartCoroutine(RestoreCollision(GetComponent<Collider2D>(), mc.GetComponent<Collider2D>()));
                            }
                            else
                            {
                                //bounceTime = Time.time + bounceDuration;
                                //rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                                boostTime = 0.5f;
                            }
                        }
                    }
                    else if (currentState == PlayerStates.Grounded)
                    {
                        float damage = 3 + (1 * weightDifference);
                        if (health - damage <= 0)
                        {
                            Hit(damage, 0, 0, mc.usernameText, Items.Eliminator);
                        }
                        else
                        {
                            Hit(damage);
                            if (mc.health - (25 + (35 * weightDifferenceEnemy)) <= 0)
                            {
                                rb.velocity = lastSpeed;
                                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), mc.GetComponent<Collider2D>(), true);
                                StartCoroutine(RestoreCollision(GetComponent<Collider2D>(), mc.GetComponent<Collider2D>()));
                            }
                            else
                            {
                                //bounceTime = Time.time + bounceDuration;
                                //rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                                boostTime = 0.5f;
                            }
                        }
                    }
                }
                else
                {
                    if (currentState == PlayerStates.Jumping)
                    {
                        bounceTime = Time.time + bounceDuration;
                        Hit(30 + (40 * weightDifference), 0, 0, mc.usernameText, Items.Boost);
                        rb.AddForce(collision.contacts[0].normal * 1600 * weightDifference);
                    }
                    else if (currentState == PlayerStates.Grounded)
                    {
                        bounceTime = Time.time + bounceDuration;
                        Hit(25 + (35 * weightDifference), 0, 0, mc.usernameText, Items.Boost);
                        rb.AddForce(collision.contacts[0].normal * 1000 * weightDifference);
                    }
                }
            }
            VFXManager.instance.CarCrash(collision.gameObject.transform.position, transform.position, collision.contacts[0].point, (collision.gameObject.GetComponent<MultiplayerController>().lastSpeed - lastSpeed).magnitude);
            vehicleAudioManager.Play("VehicleHit");
        }
        else if (collision.collider.tag == "Rebounder")
        {
            string enemyUsername = collision.collider.GetComponent<Rebounder>().username;
            if (currentState == PlayerStates.Jumping)
            {
                //CmdRemoveItem(collision.gameObject);
                bounceTime = Time.time + bounceDuration;
                Hit(40, 10, 5, enemyUsername, Items.Rebounder);
                rb.AddForce(collision.contacts[0].normal * 1400);
            }
            else if (currentState == PlayerStates.Grounded)
            {
                //CmdRemoveItem(collision.gameObject);
                bounceTime = Time.time + bounceDuration;
                Hit(40, 10, 5, enemyUsername, Items.Rebounder);
                rb.AddForce(collision.contacts[0].normal * 1000);
            }
            CmdRebounderHit(transform.position, collision.gameObject.transform.position, collision.contacts[0].point, (collision.gameObject.GetComponent<Rebounder>().lastSpeed - lastSpeed).magnitude);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            if (isLocalPlayer)
            {
                collision.gameObject.GetComponent<ItemBox>().Use(this, placement, Random.Range(0f, 1f));
                HapticsManager.instance.Rumble(0.1f, 0.3f, 0.1f);
            }
            VFXManager.instance.ItemTake(collision.transform.position);
        }
        else if (collision.tag == "Trap")
        {
            if (currentState == PlayerStates.Jumping || !isLocalPlayer) { return; }
            Hit(20, 70, 8, collision.GetComponent<Trap>().username, Items.Trap);
            CmdPlaySound("ItemHit");
        }
        else if (collision.tag == "Laser")
        {
            NetworkServer.Destroy(collision.gameObject);
            if (isLocalPlayer)
            {
                Hit(90, 20, 4, collision.GetComponent<Owner>().username, Items.Laser);
                CmdPlaySound("LaserHit");
            }
        }
        else if (collision.tag == "Booster")
        {
            Boost boost = collision.GetComponent<Boost>();
            rb.rotation = Mathf.Asin(collision.gameObject.transform.rotation.z) * Mathf.Rad2Deg * 2 * Mathf.Sign(collision.gameObject.transform.rotation.w);
            Boost(new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)), boost.boostForce, boost.boostDuration, boost.setSpeed);
        }
        else if (collision.tag == "FrontLine")
        {
            checkpointCount = 0;
            lapManager.CheckNextLap(0);
        }
        else if ((collision.tag == "Checkpoint" || collision.tag == "FinishLine"))
        {
            if (isLocalPlayer)
            {

                Vector2 collisionNormal = (collision.ClosestPoint(transform.position) - new Vector2(transform.position.x, transform.position.y)).normalized;
                collisionNormal = Vector2.Reflect(collisionNormal, Vector2.up);
                float normalFloat = Mathf.Round(Vector2.SignedAngle(collisionNormal, Vector2.right));
                if (normalFloat < 0)
                {
                    normalFloat += 360;
                }

                if (collision.tag == "FinishLine")
                {
                    spawnPos = collision.transform.position;
                    spawnRotation = collision.transform.eulerAngles.z;
                    if (lapManager == null)
                    {
                        lapManager = GameObject.FindGameObjectWithTag("LapManager").GetComponent<LapManager>();
                    }
                    if (lapManager.CheckNextLap(checkpointCount))
                    {
                        checkpointCount = 0;
                        CmdSetCheckpoint(0);
                        currentLap++;
                        CmdSetLap(currentLap, placement);
                        if (currentLap <= lapManager.lapCount)
                        {
                            PlaySound("Lap");
                        }
                    }
                    checkpointCount = 0;
                    CmdSetCheckpoint(0);
                }
                else
                {
                    if (collision.gameObject.GetComponent<Checkpoint>().respawn)
                    {
                        spawnPos = collision.transform.position;
                        spawnRotation = collision.transform.eulerAngles.z;
                    }
                    int c = collision.gameObject.GetComponent<Checkpoint>().checkpointNumber;
                    if (c > currentCheckpoint)
                    {
                        checkpointCount++;
                        currentCheckpoint = c;
                        CmdSetCheckpoint(currentCheckpoint);
                    }
                    else if (c < currentCheckpoint)
                    {
                        checkpointCount--;
                        currentCheckpoint = c;
                        CmdSetCheckpoint(currentCheckpoint);
                    }
                    //lapManager.DisableCheckpoints(currentCheckpoint);
                    //collision.gameObject.SetActive(false);
                }
            }
            SetLap();
        }
        else if (collision.tag == "Ice")
        {
            currentDrag = 0;
            currentRotationSpeed = rotationSpeed + (1 - dataManager.handling);
            iceVelocity = rb.velocity.magnitude;
            if (isLocalPlayer)
            {
                HapticsManager.instance.ice = true;
            }
            AudioManager.instance.Play("Ice");
        }
        else if (collision.tag == "Ground" || collision.tag == "Ramp" || collision.tag == "Fan")
        {
            grounded++;
        }
        else if (collision.tag == "StartPosition")
        {
            placement = collision.GetComponent<StartPosition>().GetPosition();
            SetPlacementImage();
            CmdSetCheckpoint(-placement);
        }
        else if (collision.tag == "Missile" || collision.tag == "Bomb")
        {
            if (!isLocalPlayer) { return; }
            rb.velocity = Vector2.zero;
            iceVelocity = 0;
            airborneVelocity = 0;
            if (collision.tag == "Missile")
            {
                collision.gameObject.GetComponent<Missile>().CmdExplode(new Vector3(collision.ClosestPoint(transform.position).x, collision.ClosestPoint(transform.position).y, transform.position.z));
            }
            else
            {
                collision.gameObject.GetComponent<Bomb>().CmdExplode();
            }
        }
        else if (collision.tag == "Explosion")
        {
            if (!isLocalPlayer || collision.gameObject == explosion) { return; }
            explosion = collision.gameObject;
            Vector2 collisionNormal = (new Vector2(transform.position.x, transform.position.y) - (Vector2)collision.transform.position).normalized;
            Debug.DrawRay(transform.position, collisionNormal * 30, Color.red, 10);
            if (collision.gameObject.GetComponentInParent<Explosion>() != null)
            {
                if (collision.gameObject.GetComponentInParent<Explosion>().username != "")
                {
                    Hit(Mathf.Lerp(20, collision.gameObject.GetComponentInParent<Explosion>().maxDamage, collision.gameObject.GetComponentInParent<Explosion>().Damage()), 30, 4, collision.gameObject.GetComponentInParent<Explosion>().username, Items.Missile);
                }
                else
                {
                    Hit(Mathf.Lerp(20, collision.gameObject.GetComponentInParent<Explosion>().maxDamage, collision.gameObject.GetComponentInParent<Explosion>().Damage()), 30, 4);
                }
                Launch(collisionNormal, Mathf.Lerp(500, 3000, collision.gameObject.GetComponentInParent<Explosion>().Damage()));
            }
            else if (collision.gameObject.GetComponentInParent<Shockwave>() != null)
            {
                Hit(Mathf.Lerp(20, collision.gameObject.GetComponentInParent<Shockwave>().maxDamage, collision.gameObject.GetComponentInParent<Shockwave>().Damage()), 70, 5, collision.gameObject.GetComponentInParent<Shockwave>().username, Items.Shockwave);
                Launch(collisionNormal, Mathf.Lerp(1000, 3500, collision.gameObject.GetComponentInParent<Shockwave>().Damage()));
            }
            else if (collision.gameObject.GetComponentInParent<Equalizer>() != null)
            {
                Hit(collision.gameObject.GetComponentInParent<Equalizer>().maxDamage / placement, 100 / placement, 5, collision.gameObject.GetComponentInParent<Equalizer>().username, Items.Equalizer);
                rb.velocity = rb.velocity / 10;
            }
        }
        else if (collision.tag == "Blaster")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.fire = true;
            }
            AudioManager.instance.Play("Fire");
        }
        else if (collision.tag == "Gravel")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.gravel = true;
            }
        }
        else if (collision.tag == "Heal")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.heal = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ramp")
        {
            Jump(false);
        } else if (collision.tag == "Heal" && currentState == PlayerStates.Grounded)
        {
            if (health > 15)
            {
                fireVFX.SendEvent("StopFire");
            }
            if (health > 40)
            {
                fireVFX.SendEvent("StopSmoke");
            }
            if (health <= 0 || !isLocalPlayer) { return; }
            health = health + maxHealth / 150;
            if (health > maxHealth) { health = maxHealth; }
            playerHealthBar.SetHealth((maxHealth - health) / maxHealth);
            healthBar.SetHealth((maxHealth - health) / maxHealth);
            PlaySound("Heal");
        }
        else if (collision.tag == "Booster")
        {
            rb.rotation = Mathf.Asin(collision.gameObject.transform.rotation.z) * Mathf.Rad2Deg * 2 * Mathf.Sign(collision.gameObject.transform.rotation.w);
            rb.velocity = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)) * rb.velocity.magnitude;
        }
        else if (collision.tag == "Gravel")
        {
            if (boostTime > NetworkTime.time) { return;}
            PlaySound("Gravel");
            AudioManager.instance.SetPitch("Gravel", Mathf.Lerp(0f, 1f, currentSpeed / 40));
            if (isLocalPlayer)
            {
                PostProcessingManager.instance.Gravel();
            }
            if (rb.velocity.magnitude > currentMaxSpeed / 2)
            {
                rb.AddForce(rb.velocity.normalized * -(currentDrag * 8));
            }
        }
        else if (collision.tag == "Blaster" && currentState == PlayerStates.Grounded)
        {
            if (shielded > (float)NetworkTime.time) { return; }
            health = health - 0.5f; if (health > 15 && health <= 40)
            {
                fireVFX.SendEvent("StartSmoke");
                smoking = true;
            }
            else if (health > 0 && health <= 15)
            {
                fireVFX.SendEvent("StopSmoke");
                fireVFX.SendEvent("StartSmoke");
                fireVFX.SendEvent("StartFire");
                smoking = true;
                onFire = true;
            }
            if (isLocalPlayer)
            {
                if (health < 0) { health = 0; }
                playerHealthBar.SetHealth((maxHealth - health) / maxHealth);
                healthBar.SetHealth((maxHealth - health) / maxHealth);
                CmdChangeHealth(health);
                PostProcessingManager.instance.Hit(health / maxHealth);
            }
        }
        else if (collision.tag == "Magnet" && currentState == PlayerStates.Jumping)
        {
            if (airborne > 0) { airborne = 0; }
            airborne -= gravity * 4;
        }
        else if (collision.tag == "Fan")
        {
            if (collision.GetComponent<Fan>().stopped) 
            { 
                if (currentAirborne < 1 && isLocalPlayer)
                {
                    CmdChangeHealth(0);
                    HapticsManager.instance.RumbleLinear(1, 1, 1f);
                    PlaySound("Fall");
                }
                return;
            }
            float force = collision.GetComponent<Fan>().force;
            if (currentState == PlayerStates.Grounded) { currentState = PlayerStates.Jumping; }
            if (currentState == PlayerStates.Grounded) { currentState = PlayerStates.Jumping; }
            if (currentAirborne <= 1 && force < 0)
            {
                collision.GetComponent<Fan>().Stop();
                if (!isLocalPlayer){ return; }
                Hit(Mathf.Abs(force * 2));
                airborne = 0.05f;
                if(!(shielded > (float)NetworkTime.time))
                {
                    CmdPlaySound("ItemHit");
                    VFXManager.instance.FanCrash(collision.transform.position, transform.position, force);
                }
            }
            else
            {
                if (!isLocalPlayer) { return; }
                if (force == 0)
                {
                    if (currentAirborne < 2f)
                    {
                        if (currentAirborne < 1.1f && airborne < 0) { airborne = 0; }
                        airborne += gravity * 5;
                        if (airborne > 0.08f)
                        {
                            airborne = 0.08f;
                        }
                        HapticsManager.instance.Rumble(0.1f, 0.2f, 0.1f);
                    }
                }
                if (force > 0)
                {
                    if (currentAirborne < 2f)
                    {
                        if (currentAirborne < 1.1f && airborne < 0) { airborne = 0; }
                        if (airborne < force / 100)
                        {
                            airborne += gravity * 3;
                        }
                        if (currentAirborne > 1.9f && airborne > 0) 
                        {
                            airborne = force / 100;
                        }
                        HapticsManager.instance.Rumble(0.1f, 0.3f, 0.1f);
                    }
                }
                if (force < 0)
                {
                    airborne += force / 1000;
                    HapticsManager.instance.Rumble(0.5f, 0.2f, 0.1f);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Ramp")
        {
            grounded--;
            /*if (currentState == PlayerStates.Grounded && grounded <= 0 && airborne <= 0 && !fallProtection && isLocalPlayer)
            {
                CmdChangeHealth(0);
                HapticsManager.instance.RumbleLinear(1, 1, 1f);
                PlaySound("Fall");
            }*/
        }
        else if (collision.tag == "Heal")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.heal = false;
            }
            StopSound("Heal");
            CmdChangeHealth(health);
        }
        else if (collision.tag == "Ice")
        {
            currentDrag = drag;
            currentRotationSpeed = rotationSpeed;
            iceVelocity = 0;
            if (isLocalPlayer)
            {
                HapticsManager.instance.ice = false;
            }
            AudioManager.instance.Stop("Ice");
        }
        else if (collision.tag == "Heal")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.heal = false;
            }
            StopSound("Heal");
        }
        else if (collision.tag == "Gravel")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.gravel = false;
            }
            StopSound("Gravel");
        }
        else if (collision.tag == "Blaster")
        {
            if (isLocalPlayer)
            {
                HapticsManager.instance.fire = false;
            }
            AudioManager.instance.Stop("Fire");
        }
        else if (collision.tag == "Fan")
        {
            grounded--;
            if (collision.GetComponent<Fan>().stopped) { return; }
            float force = collision.GetComponent<Fan>().force;
            if (currentState == PlayerStates.Grounded) { currentState = PlayerStates.Jumping; }
            if (force > 0)
            {
                if (currentAirborne < 4f)
                {
                    if (airborne < force / 100)
                    {
                        airborne = force / 100;
                    }
                }
            }
        }
    }
}
