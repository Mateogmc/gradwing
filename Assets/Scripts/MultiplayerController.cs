using System.Collections;
using System.Collections.Generic;
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

    [SyncVar(hook = nameof(OnStateChange))]public PlayerStates currentState = PlayerStates.Grounded;

    [Header("Inspector variables")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D bc;
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
    [SerializeField] [SyncVar(hook = nameof(OnHandlingChange))] float rotationSpeed;
    float currentRotationSpeed;
    [SerializeField] float strafingAngle;
    [SerializeField] float shieldDuration;
    [SerializeField] float deathTimer;
    [SerializeField][SyncVar(hook = nameof(OnSpriteChange))] int spriteValue;

    float dead;
    float currentSpeed = 0;
    float bounceDuration = 0.5f;
    [HideInInspector] public float bounceTime = 0f;
    [SyncVar(hook = nameof(OnBoostChange))]public float boostTime = 0f;
    float initialBoost = 0;
    private GameObject explosion;
    float airborne = 0f;
    int grounded = 1;
    [SyncVar(hook = nameof(OnAirborneChange))] public float currentScale = 1;
    public Vector2 direction;
    [HideInInspector] [SyncVar(hook = nameof(LastSpeedVectorChange))] public Vector2 lastSpeed;
    [SyncVar(hook = nameof(LastSpeedChange))] public float lastSpeedMagnitude;
    [SyncVar(hook = nameof(OnLayerChange))]int currentLayer = 6;

    [HideInInspector]  bool accelerating = false;
    [HideInInspector] bool braking = false;
    [HideInInspector] public bool rotatingRight = false;
    [HideInInspector] public bool rotatingLeft = false;
    [HideInInspector] public bool strafingRight = false;
    [HideInInspector] public bool strafingLeft = false;
    [HideInInspector] public float strafeValue;
    [HideInInspector] public bool rolling;
    [HideInInspector] [SyncVar(hook = nameof(OnRollingChange))] public bool rollingSync;

    [SerializeField] [SyncVar(hook = nameof(OnMaxHealthChange))] float maxHealth;
    [SerializeField] [SyncVar(hook = nameof(OnHealthChange))] float health;

    [Header("Item Sprites")]
    [SerializeField] Sprite none;
    [SerializeField] Sprite shield;
    [SerializeField] Sprite jump;
    [SerializeField] Sprite trap;
    [SerializeField] Sprite rebounder;
    [SerializeField] Sprite laser;
    [SerializeField] Sprite boost;
    [SerializeField] Sprite missile;
    [SerializeField] Image itemSprite;

    [SyncVar(hook = nameof(OnShieldChange))] float shielded = 0f;
    float slowTimer = 0f;

    [SerializeField] GameObject trapPrefab;
    [SerializeField] GameObject rebounderPrefab;
    [SerializeField] GameObject missilePrefab;
    Vector3 itemPosition;

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

    [SerializeField] [SyncVar(hook = nameof(OnItemChange))] Items currentItem = Items.None;

    [SyncVar(hook = nameof(OnUsernameChange))] string usernameText;

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

    // Interface
    [SerializeField] TextMeshProUGUI lapRenderer;
    [SerializeField] Canvas playerInterface;
    [SerializeField] Canvas enemyInterface;
    [SerializeField] Slider healthBar;
    [SerializeField] Slider playerHealthBar;
    [SerializeField] Image playerItemSprite;
    [SerializeField] Canvas ui;
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] SpriteRenderer shieldRenderer;
    [SerializeField] Image placementNumber;
    [SerializeField] Canvas endgameInterface;
    [SerializeField] Image endgamePlacementImage;
    [SerializeField] Canvas startingCanvas;
    [SerializeField] TextMeshProUGUI startingText;

    public override void OnStartLocalPlayer()
    {
        if (LobbyManager.GetInstance().localPlayer == null)
        {
            LobbyManager.GetInstance().localPlayer = this;
        }
    }

    protected override void Initialize()
    {
        if (!isLocalPlayer) { return; }
        usernameText = DataManager.username;
        username.text = usernameText;
        CmdSetName(usernameText);
        playerInterface.gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            currentState = PlayerStates.Grounded;
        }
        else
        {
            GameStateManager.GetInstance().gameState = GameStateManager.GameState.Starting;
            currentState = PlayerStates.Starting;
            startingCanvas.enabled = true;
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
        startingCanvas.transform.parent = null;
        endgameInterface.transform.parent = null;
        lineRenderer.material = new Material(laserMaterial);
        Restart();
    }

    protected override void FSMUpdate()
    {
        ui.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        if (!isLocalPlayer) { return; }
        CheckInput();
        CheckState();
        if (currentItem == Items.Laser)
        {
            LaserUpdate();
        }

        currentSpeed = rb.velocity.magnitude;
        lastSpeed = rb.velocity;
        lastSpeedMagnitude = lastSpeed.magnitude;
        CmdSetLastSpeed(lastSpeed);
        CmdSetSpeedMagnitude(lastSpeedMagnitude);
    }

    protected override void FSMFixedUpdate()
    {
        Rendering();
        if (!isLocalPlayer) { return; }
        Move();
    }

    private void CheckState()
    {
        if (currentState == PlayerStates.Finish)
        {
            SetLayer(13);

            if (isLocalPlayer && (playerInterface.enabled == true || GameStateManager.GetInstance().gameState == GameStateManager.GameState.GameOver))
            {
                playerInterface.enabled = false;
                endgameInterface.enabled = true;
                endgamePlacementImage.sprite = Resources.Load<Sprite>("UI/placement" + endgameManager.GetPlacement(usernameText));
            }
        }
        else if (currentState == PlayerStates.Starting)
        {
            int currentCountdown = Mathf.FloorToInt(raceCountdown - Time.time);
            if (accelerating && currentCountdown < 4)
            {
                initialBoost += 1 * Time.deltaTime;
                Debug.Log(initialBoost);
            }
            if (currentCountdown > 0 && currentCountdown < 4)
            {
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
                GameStateManager.GetInstance().gameState = GameStateManager.GameState.Running;
                StartCoroutine(CloseStartingScreen());
            }
        }
        else
        {
            if (currentState == PlayerStates.Jumping && airborne <= 0/* && !CheckOffRoadUp(new Vector2(transform.position.x, transform.position.y), 0)*/)
            {
                if (grounded > 0)
                {
                    currentState = PlayerStates.Grounded;
                }
                else
                {
                    CmdChangeHealth(0);
                }
            }
            if (health <= 0f && currentState != PlayerStates.Dead)
            {
                currentState = PlayerStates.Dead;
                dead = Time.time + deathTimer;
            }
            else if (health > 0f && currentState == PlayerStates.Dead)
            {
                currentState = PlayerStates.Grounded;
            }
            if (currentState == PlayerStates.Dead && dead < Time.time)
            {
                Restart();
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
            if (firingLaser && laserCountdown < Time.time)
            {
                CmdLaserHit(transform.position + new Vector3(direction.x, direction.y, 0) * 2, direction, laserPosition.position, transform.rotation);
                firingLaser = false;
                lineRenderer.enabled = false;
                CmdUseItem();
                currentItem = Items.None;
                itemSprite.sprite = none;
            }
        }
    }

    private void Rendering()
    {
        if (shielded > Time.time)
        {
            shieldRenderer.enabled = !shieldRenderer.enabled;
        }
        else if (shieldRenderer.enabled)
        {
            shieldRenderer.enabled = false;
        }
        if (currentItem == Items.Laser && laserCountdown < Time.time)
        {
            lineRenderer.enabled = true;
        }
        if (firingLaser && laserCountdown > Time.time)
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
        if (currentItem == Items.None)
        {
            itemSprite.sprite = none;
        }
        sr.sprite = Resources.Load<Sprite>("Vehicles/Vehicle" + this.spriteValue);
        vehicle.SetColor();
    }

    public void FollowPlayer()
    {
        playerInterface.gameObject.SetActive(true);
        enemyInterface.enabled = false;
    }
    public void UnfollowPlayer()
    {
        enemyInterface.enabled = true;
        playerInterface.gameObject.SetActive(false);
    }

    public void ToggleResults()
    {
        endgameInterface.enabled = !endgameInterface.enabled;
    }

    private void SetPlacementImage()
    {
        placementNumber.sprite = Resources.Load<Sprite>("UI/placement" + placement);
        endgamePlacementImage.sprite = Resources.Load<Sprite>("UI/placement" + placement);
    }

    public void Restart()
    {
        transform.position = spawnPos;
        transform.eulerAngles = new Vector3(0, 0, spawnRotation);
        CmdSetStats(DataManager.MAX_SPEED + dataManager.speed, DataManager.ACCELERATION + dataManager.acceleration, DataManager.WEIGHT + dataManager.weight, DataManager.HANDLING + dataManager.handling, dataManager.spriteValue);
        CmdSetColor(DataManager.GetInstance().speed, DataManager.GetInstance().acceleration, DataManager.GetInstance().weight, DataManager.GetInstance().handling);
        healthBar.value = 0f;
        playerHealthBar.value = 0f;
        rb.velocity = Vector3.zero;
        rb.rotation = 90;
        sr.transform.localScale = Vector3.one;
        CmdSetScale(sr.transform.localScale.x);
        xboxController = DataManager.GetInstance().xboxController;
        SetPlacementImage();
        SetLayer(6);
    }

    private void OnHealthChange(float oldHealth, float newHealth)
    {
        healthBar.value = maxHealth - newHealth;
        playerHealthBar.value = maxHealth - newHealth;
    }

    private void OnMaxHealthChange(float oldHealth, float newHealth)
    {
        maxHealth = newHealth;
        playerHealthBar.maxValue = maxHealth;
        healthBar.maxValue = maxHealth;
    }

    private void OnUsernameChange(string oldName, string newName)
    {
        username.text = newName;
    }

    private void OnAirborneChange(float oldAirborne, float newAirborne)
    {
        sr.transform.localScale = new Vector3(newAirborne, newAirborne, sr.transform.localScale.z);
    }

    private void OnShieldChange(float oldShield, float newShield)
    {
        shielded = Time.time + shieldDuration;
    }

    private void OnFiringLaserChange(bool oldBool, bool newBool)
    {
        firingLaser = newBool;
    }

    private void OnLaserCountdownChange(float oldCountdown, float newCountdown)
    {
        laserCountdown = Time.time + 0.5f;
    }

    private void OnLayerChange(int oldLayer, int newLayer)
    {
        gameObject.layer = newLayer;
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
        }
    }

    private void CheckInput()
    {
        if (xboxController)
        {

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                //Restart();
            }
            if (Input.GetKey(KeyCode.X) || Input.GetButton("Accel"))
            {
                accelerating = true;
            }
            else
            {
                accelerating = false;
            }

            if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.JoystickButton1))
            {
                braking = true;
            }
            else
            {
                braking = false;
            }

            if (accelerating && braking)
            {
                rolling = true;
            }
            else
            {
                rolling = false;
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

            strafeValue = Input.GetAxis("Strafe1");

            if (Input.GetKey(KeyCode.S) || Input.GetAxis("Strafe1") < -0.1)
            {
                strafingLeft = true;
            }
            else
            {
                strafingLeft = false;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetAxis("Strafe1") > 0.1)
            {
                strafingRight = true;
            }
            else
            {
                strafingRight = false;
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                UseItem();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                //Restart();
            }
            if (Input.GetKey(KeyCode.X) || Input.GetButton("Accel"))
            {
                accelerating = true;
            }
            else
            {
                accelerating = false;
            }

            if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.JoystickButton1))
            {
                braking = true;
            }
            else
            {
                braking = false;
            }

            if (accelerating && braking)
            {
                rolling = true;
            }
            else
            {
                rolling = false;
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

            strafeValue = Input.GetAxis("StrafeR") - Input.GetAxis("StrafeL");

            if (strafeValue < -0.1f)
            {
                strafingLeft = true;
                strafingRight = false;
            } 
            else if (strafeValue > 0.1f)
            {
                strafingLeft = false;
                strafingRight = true;
            }
            else
            {
                strafingLeft = false;
                strafingRight = false;
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                UseItem();
            }
        }
        if (rollingSync != rolling)
        {
            CmdRolling(rolling);
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
                SetLayer(6);
                CmdSetScale(1);
                sr.sortingLayerID = SortingLayer.NameToID("Players");
                if (rolling)
                {
                    if (currentSpeed > 1)
                    {
                        rb.AddForce(rb.velocity.normalized * -(currentDrag));
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
                        rb.AddForce(direction * acceleration);
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
                    rb.AddForce(rb.velocity.normalized * -(currentDrag * 5));
                }

                if (!rolling && bounceTime < Time.time)
                {

                    if (!DataManager.GetInstance().strafeMode && (strafingLeft || strafingRight))
                    {
                        currentMaxSpeed = (rotatingLeft || rotatingRight ? maxSpeed + (15 * rotationSpeed) : maxSpeed);
                        float newAngle = rb.rotation - (strafingAngle * strafeValue);
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;
                    }
                    else if (DataManager.GetInstance().strafeMode)
                    {
                        if (rotatingLeft)
                        {
                            currentMaxSpeed = (rotatingLeft || rotatingRight ? maxSpeed + (15 * rotationSpeed) : maxSpeed);
                            float newAngle = rb.rotation + (strafingAngle * strafeValue);
                            Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                            rb.velocity = tempDirection * rb.velocity.magnitude;
                        }
                        else if (DataManager.GetInstance().strafeMode && rotatingRight)
                        {
                            currentMaxSpeed = (rotatingLeft || rotatingRight ? maxSpeed + (15 * rotationSpeed) : maxSpeed);
                            float newAngle = rb.rotation - (strafingAngle * strafeValue);
                            Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                            rb.velocity = tempDirection * rb.velocity.magnitude;
                        }
                    }
                    else
                    {
                        currentMaxSpeed = maxSpeed;
                        rb.velocity = direction * rb.velocity.magnitude;
                    }
                }
                else if (bounceTime > Time.time)
                {
                    if (currentSpeed > 1)
                    {
                        rb.AddForce(rb.velocity.normalized * -currentDrag * 4);
                    }
                }

                if (rotatingLeft && !rotatingRight)
                {
                    rb.rotation += (rolling ? 6f : (3f + (DataManager.GetInstance().strafeMode ? strafeValue : - strafeValue) * 2f)) * currentRotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 6f : (3f + strafeValue * 2f)) * currentRotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                break;

            case PlayerStates.Jumping:
                if (airborne > 0)
                {
                    airborne -= (braking ? 0.1f : 0.05f);
                    sr.transform.localScale = new Vector3(1 + Mathf.Abs(Mathf.Sin(airborne)), 1 + Mathf.Abs(Mathf.Sin(airborne)), 1);
                    CmdSetScale(sr.transform.localScale.x);
                    //CmdSetScale(sr.transform.localScale);
                } else
                {
                    currentState = PlayerStates.Grounded;
                    sr.transform.localScale = new Vector3(1, 1, 1);
                }
                if (rotatingLeft && !rotatingRight)
                {
                    rb.rotation += (rolling ? 4f : (3f - strafeValue * 2f)) * currentRotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 4f : (3f + strafeValue * 2f)) * currentRotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                if (strafingLeft && !strafingRight)
                {
                    float newAngle = rb.rotation - (30 * strafeValue);
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    rb.velocity = tempDirection * rb.velocity.magnitude;
                    //rb.AddForce(tempDirection * 30 * strafeValue * rotationSpeed);
                }
                else if (strafingRight && !strafingLeft)
                {
                    float newAngle = rb.rotation - (30 * strafeValue);
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    rb.velocity = tempDirection * rb.velocity.magnitude;
                    //rb.AddForce(tempDirection * 30 * strafeValue * rotationSpeed);
                }
                break;

            case PlayerStates.Dead:
                if (sr.transform.localScale.x > 0)
                {
                    sr.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
                    CmdSetScale(sr.transform.localScale.x);
                }
                SetLayer(10);
                CmdChangeHealth(health);
                rb.velocity = Vector3.zero;
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
    }

    private void SetLap()
    {
        if (lapManager == null)
        {
            lapManager = GameObject.FindGameObjectWithTag("LapManager").GetComponent<LapManager>();
        }
        lapRenderer.text = string.Format("{0}/{1}", currentLap, lapManager.lapCount);
    }

    private IEnumerator CloseStartingScreen()
    {
        yield return new WaitForSeconds(1);
        startingCanvas.enabled = false;
    }

    public void StartRace()
    {
        raceCountdown = Time.time + 6;
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
            RpcEndRace();
        }
    }

    [ClientRpc]
    private void RpcEndRace()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<MultiplayerController>().endgameManager.AddDictionaryEntry(placement, usernameText);
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

    private void Jump(bool item)
    {
        if (currentState == PlayerStates.Grounded)
        {
            currentState = PlayerStates.Jumping;
            airborne = Mathf.PI;
            SetLayer(8);
            sr.sortingLayerID = SortingLayer.NameToID("Foreground");
        } else if (currentState == PlayerStates.Jumping && item)
        {
            airborne += Mathf.PI;
        }
    }

    private void Boost(Vector2 direction, float force)
    {
        bounceTime = Time.time;
        rb.velocity = direction * force + rb.velocity;
    }

    private void Boost(Vector2 direction, float force, float duration)
    {
        boostTime = Time.time + duration;
        CmdBoost(boostTime);
        Boost(direction, force);
    }

    [Command]
    private void CmdSetStats(float speed, float accel, float weight, float handling, int spriteValue)
    {
        if (currentState != PlayerStates.Starting)
        {
            currentState = PlayerStates.Grounded;
        }
        this.maxSpeed = (speed * 1.5f);
        this.acceleration = accel;
        this.weight = weight;
        this.rotationSpeed = handling + 0.2f;
        this.spriteValue = spriteValue;
        currentRotationSpeed = rotationSpeed;

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
    private void CmdTrap(Vector2 pos)
    {
        RpcTrap(pos);
    }

    [ClientRpc]
    private void RpcTrap(Vector2 pos)
    {
        GameObject trp = Instantiate(trapPrefab, pos, Quaternion.identity);
    }

    [Command]
    private void CmdRebounder(Vector3 pos, Vector2 initialVelocity, PlayerStates state, float airborne)
    {
        RpcRebounder(pos, initialVelocity, state, airborne);
    }

    [ClientRpc]
    private void RpcRebounder(Vector3 pos, Vector2 initialVelocity, PlayerStates state, float airborne)
    {
        GameObject reb = Instantiate(rebounderPrefab, pos, Quaternion.identity);
        reb.GetComponent<Rebounder>().InitializeRebounder(initialVelocity, state, airborne);
    }

    #region LASER

    [Command]
    private void CmdLaser()
    {
        laserCountdown = Time.time + 0.5f;
        firingLaser = true;
    }

    [Command]
    private void CmdLaserHit(Vector3 pos, Vector2 dir, Vector3 laserPos, Quaternion laserRotation)
    {
        float distance = 10000;
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 10000, layerMask);
        if (Physics2D.Raycast(pos, dir, 10000, layerMask))
        {
            RpcLaserHit(hit.point);
            distance = hit.distance * 10;
        }
        RpcShootLaser(laserPos, laserRotation, distance);
        firingLaser = false;
    }

    [ClientRpc]
    private void RpcLaserHit(Vector2 hitPoint)
    {
        GameObject las = Instantiate(laserHit, hitPoint, Quaternion.identity);
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
        float countdown = laserCountdown - Time.time;
        if (hit)
        {
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.green;
            lineRenderer.material.SetColor("_Color", firingLaser ? new Vector4(Mathf.Lerp(150, 0, countdown - 0.1f / 0.4f), Mathf.Lerp(150, 50, countdown - 0.1f / 0.4f), 200, 2) : new Vector4(0, 150, 0, 2));
        }
        else
        {
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.red;
            lineRenderer.material.SetColor("_Color", firingLaser ? new Vector4(Mathf.Lerp(150, 0, countdown - 0.1f / 0.4f), Mathf.Lerp(150, 50, countdown - 0.1f / 0.4f), 200, 2) : new Vector4(150, 0, 0, 2));
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

    [Command]
    private void CmdBoost(float boost)
    {
        boostTime = boost;
    }

    [Command]
    private void CmdMissile(Vector2 pos)
    {
        GameObject mis = Instantiate(missilePrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(mis);
    }

    #endregion

    public void Hit(float damage)
    {
        if (shielded > Time.time || !isLocalPlayer) { return; }
        health = health - damage;
        CmdChangeHealth(health);
    }

    public void Hit(float damage, float slow, float duration)
    {
        if (shielded > Time.time || !isLocalPlayer) { return; }
        Hit(damage);
        currentMaxSpeed -= slow;
        if (currentMaxSpeed < 10)
        {
            currentMaxSpeed = 10;
        }
        slowTimer = Time.time + duration;
        rb.velocity /= 3;
    }

    [Command]
    private void CmdRemoveItem(GameObject item)
    {
        NetworkServer.Destroy(item);
    }

    private void UseItem()
    {
        switch (currentItem)
        {
            case Items.None:

                break;

            case Items.Shield:
                CmdShield();
                break;

            case Items.Jump:
                Jump(true);
                break;

            case Items.Trap:
                if (currentSpeed > 1)
                {
                    itemPosition = transform.position - new Vector3(rb.velocity.normalized.x, rb.velocity.normalized.y, 1) * 5;
                }
                else
                {
                    itemPosition = transform.position - new Vector3(direction.x, direction.y, 1) * 3;
                }
                CmdTrap(itemPosition);
                break;

            case Items.Rebounder:
                itemPosition = transform.position + new Vector3(direction.x, direction.y, 0) * 5;
                CmdRebounder(itemPosition, direction * maxSpeed + rb.velocity, currentState, airborne);
                break;

            case Items.Laser:
                CmdLaser();
                return;

            case Items.Boost:
                Boost(direction, 30, 1.5f);
                break;

            case Items.Missile:
                itemPosition = transform.position + new Vector3(direction.x, direction.y, 0) * 5;
                CmdMissile(itemPosition);
                break;
        }
        currentItem = Items.None;
        itemSprite.sprite = none;
        playerItemSprite.sprite = none;
        CmdUseItem();
    }

    [Command(requiresAuthority = false)]
    public void CmdGetItem(string item)
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
        if (newHealth > maxHealth) { newHealth = maxHealth; }
        if (newHealth < 0) { newHealth = 0; }
        health = newHealth;
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
        placement = tempPlacement;
        if (isLocalPlayer)
        {
            CmdSetPlacement(tempPlacement);
            SetPlacementImage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer) { return; }
        if (collision.collider.tag == "Wall")
        {
            if (bounceTime > Time.time)
            {
                bounceTime = Time.time + bounceDuration / 2;
            }
            else
            {
                bounceTime = Time.time + bounceDuration;
            }
            boostTime = Time.time;
            rb.velocity = Vector2.Reflect(lastSpeed * 0.75f, collision.contacts[0].normal);
            if (rb.velocity.magnitude < 10)
            {
                rb.AddForce(collision.contacts[0].normal * 500);
            }
            CmdChangeHealth(health - 10);
            //rb.rotation = Vector2.SignedAngle(Vector2.right, rb.velocity);
        }
        else if (collision.collider.tag == "Player")
        {
            MultiplayerController mc = collision.gameObject.GetComponent<MultiplayerController>();
            float weightDifference = mc.weight - weight;
            if (weightDifference < 0.5f)
            {
                weightDifference = 0.5f;
            }
            if ((boostTime > Time.time && mc.boostTime > Time.time) || ((boostTime < Time.time && mc.boostTime < Time.time))) {

                if (currentState == PlayerStates.Jumping)
                {
                    bounceTime = Time.time + bounceDuration;
                    rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                    CmdChangeHealth(health - 10 - (6 * weightDifference));
                }
                else if (currentState == PlayerStates.Grounded)
                {
                    bounceTime = Time.time + bounceDuration;
                    rb.AddForce(collision.contacts[0].normal * 500 * weightDifference);
                    CmdChangeHealth(health - 5 - (3 * weightDifference));
                }
            }
            else
            {
                if (boostTime > Time.time)
                {
                    if (currentState == PlayerStates.Jumping)
                    {
                        bounceTime = Time.time + bounceDuration;
                        rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                        CmdChangeHealth(health - 6 - (2 * weightDifference));
                    }
                    else if (currentState == PlayerStates.Grounded)
                    {
                        bounceTime = Time.time + bounceDuration;
                        rb.AddForce(collision.contacts[0].normal * 500 * weightDifference);
                        CmdChangeHealth(health - 3 - (1 * weightDifference));
                    }
                    boostTime = Time.time;
                }
                else
                {
                    if (currentState == PlayerStates.Jumping)
                    {
                        bounceTime = Time.time + bounceDuration;
                        rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                        CmdChangeHealth(health - 25 - (20 * weightDifference));
                    }
                    else if (currentState == PlayerStates.Grounded)
                    {
                        bounceTime = Time.time + bounceDuration;
                        rb.AddForce(collision.contacts[0].normal * 500 * weightDifference);
                        CmdChangeHealth(health - 15 - (10 * weightDifference));
                    }
                }
            }
        }
        else if (collision.collider.tag == "Rebounder")
        {
            if (currentState == PlayerStates.Jumping)
            {
                //CmdRemoveItem(collision.gameObject);
                bounceTime = Time.time + bounceDuration;
                rb.AddForce(collision.contacts[0].normal * 1400);
                Hit(40, 10, 5);
            }
            else if (currentState == PlayerStates.Grounded)
            {
                //CmdRemoveItem(collision.gameObject);
                bounceTime = Time.time + bounceDuration;
                Hit(40, 10, 5);
                rb.AddForce(collision.contacts[0].normal * 1000);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            collision.gameObject.GetComponent<ItemBox>().Use(this, placement);
        }
        else if (collision.tag == "Trap")
        {
            if (currentState == PlayerStates.Jumping || !isLocalPlayer) { return; }
            Hit(20, 30, 6);
        }
        else if (collision.tag == "Laser")
        {
            NetworkServer.Destroy(collision.gameObject);
            Hit(90, 40, 6);
        }
        else if (collision.tag == "Booster")
        {
            rb.rotation = Mathf.Asin(collision.gameObject.transform.rotation.z) * Mathf.Rad2Deg * 2 * Mathf.Sign(collision.gameObject.transform.rotation.w);
            Boost(new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)), 20);
        }
        else if (collision.tag == "FrontLine")
        {
            checkpointCount = 0;
            lapManager.CheckNextLap(0);
        }
        else if ((collision.tag == "Checkpoint" || collision.tag == "FinishLine") && isLocalPlayer)
        {
            spawnPos = collision.transform.position;
            spawnRotation = collision.transform.eulerAngles.z;

            Vector2 collisionNormal = (collision.ClosestPoint(transform.position) - new Vector2(transform.position.x, transform.position.y)).normalized;
            collisionNormal = Vector2.Reflect(collisionNormal, Vector2.up);
            float normalFloat = Mathf.Round(Vector2.SignedAngle(collisionNormal, Vector2.right));
            if (normalFloat < 0)
            {
                normalFloat += 360;
            }

            if (collision.tag == "FinishLine")
            {
                if (lapManager == null)
                {
                    lapManager = GameObject.FindGameObjectWithTag("LapManager").GetComponent<LapManager>();
                }
                if (lapManager.CheckNextLap(checkpointCount))
                {
                    currentLap++;
                    CmdSetLap(currentLap, placement);
                    SetLap();
                }
                checkpointCount = 0;
                CmdSetCheckpoint(0);
            } else
            {
                checkpointCount++;
                currentCheckpoint = collision.gameObject.GetComponent<CheckpointValue>().checkpointNumber;
                CmdSetCheckpoint(currentCheckpoint);
                lapManager.DisableCheckpoints(currentCheckpoint);
                collision.gameObject.SetActive(false);
            }
        }
        else if (collision.tag == "Ice")
        {
            currentDrag = 0;
            currentRotationSpeed = rotationSpeed + (1 - dataManager.handling);
        }
        else if (collision.tag == "Ground" || collision.tag == "Ramp")
        {
            grounded++;
        }
        else if (collision.tag == "StartPosition")
        {
            placement = collision.GetComponent<StartPosition>().GetPosition();
            SetPlacementImage();
            CmdSetCheckpoint(-placement);
        }
        else if (collision.tag == "Missile")
        {
            if (!isLocalPlayer) { return; }
            rb.velocity = Vector2.zero;
            collision.gameObject.GetComponent<Missile>().CmdExplode(new Vector3(collision.ClosestPoint(transform.position).x, collision.ClosestPoint(transform.position).y, transform.position.z));
        }
        else if (collision.tag == "Explosion")
        {
            if (!isLocalPlayer || collision.gameObject == explosion) { return; }
            explosion = collision.gameObject;
            Vector2 collisionNormal = (new Vector2(transform.position.x, transform.position.y) - (Vector2)collision.transform.position).normalized;
            Debug.DrawRay(transform.position, collisionNormal * 30, Color.red, 10);
            Hit(Mathf.Lerp(20, 100, collision.gameObject.GetComponentInParent<Explosion>().Damage()), 40, 5);
            rb.AddForce(collisionNormal * Mathf.Lerp(500, 3000, collision.gameObject.GetComponentInParent<Explosion>().Damage()));
            bounceTime = Time.time + bounceDuration;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ramp")
        {
            Jump(false);
        } else if (collision.tag == "Heal" && currentState == PlayerStates.Grounded)
        {
            if (health <= 0 || !isLocalPlayer || bounceTime > Time.time) { return; }
            health = health + maxHealth / 200;
            playerHealthBar.value = maxHealth - health;
            healthBar.value = maxHealth - health;
            CmdChangeHealth(health);
        }
        else if (collision.tag == "Booster")
        {
            rb.rotation = Mathf.Asin(collision.gameObject.transform.rotation.z) * Mathf.Rad2Deg * 2 * Mathf.Sign(collision.gameObject.transform.rotation.w);
            rb.velocity = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)) * rb.velocity.magnitude;
        }
        else if (collision.tag == "Gravel")
        {
            if (boostTime > Time.time) { return;}
            if (rb.velocity.magnitude > currentMaxSpeed / 2)
            {
                rb.AddForce(rb.velocity.normalized * -(currentDrag * 6));
            }
        }
        else if (collision.tag == "Blaster" && currentState == PlayerStates.Grounded)
        {
            if (shielded > Time.time) { return; }
            health = health - 0.3f;
            playerHealthBar.value = maxHealth - health;
            healthBar.value = maxHealth - health;
            CmdChangeHealth(health);
        }
        else if (collision.tag == "Magnet" && currentState == PlayerStates.Jumping)
        {
            airborne -= 0.05f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Ramp")
        {
            grounded--;
            if (currentState == PlayerStates.Grounded && grounded <= 0 && airborne <= 0)
            {
                CmdChangeHealth(0);
            }
        }
        else if (collision.tag == "Ice")
        {
            currentDrag = drag;
            currentRotationSpeed = rotationSpeed;
        }
    }
}
