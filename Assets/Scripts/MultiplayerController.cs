using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public enum PlayerStates
{
    None, Grounded, Jumping, Dead
}

public class MultiplayerController : FSM
{

    public PlayerStates currentState = PlayerStates.Grounded;

    [Header("Inspector variables")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask playerLayer;
    DataManager dataManager;
    bool xboxController = true;


    [Header("Player variables")]
    [SerializeField] [SyncVar(hook = nameof(OnMaxSpeedChange))] float maxSpeed;
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

    float dead;
    float currentSpeed = 0;
    float bounceDuration = 0.5f;
    [HideInInspector] public float bounceTime = 0f;
    float airborne = 0f;
    int grounded = 1;
    [SyncVar(hook = nameof(OnAirborneChange))] float currentScale = 1;
    public Vector2 direction;
    [HideInInspector] public Vector2 lastSpeed;
    [SyncVar(hook = nameof(OnLayerChange))]int currentLayer = 6;

    [HideInInspector]  bool accelerating = false;
    [HideInInspector] bool braking = false;
    [HideInInspector] public bool rotatingRight = false;
    [HideInInspector] public bool rotatingLeft = false;
    [HideInInspector] public bool strafingRight = false;
    [HideInInspector] public bool strafingLeft = false;
    [HideInInspector] public float strafeValue;
    [HideInInspector] public bool rolling;

    [SerializeField][SyncVar(hook = nameof(OnMaxHealthChange))] float maxHealth;
    [SerializeField] [SyncVar(hook = nameof(OnHealthChange))] float health;

    [Header("Item Sprites")]
    [SerializeField] Sprite none;
    [SerializeField] Sprite shield;
    [SerializeField] Sprite jump;
    [SerializeField] Sprite trap;
    [SerializeField] Sprite rebounder;
    [SerializeField] Sprite laser;
    [SerializeField] Image itemSprite;

    [SyncVar(hook = nameof(OnShieldChange))] float shielded = 0f;
    float slowTimer = 0f;

    [SerializeField] GameObject trapPrefab;

    [SerializeField] GameObject rebounderPrefab;
    Vector3 itemPosition;

    [SerializeField] GameObject laserHit;
    [SerializeField] LineRenderer lineRenderer;
    [SyncVar(hook = nameof(OnFiringLaserChange))] bool firingLaser = false;
    [SyncVar(hook = nameof(OnLaserCountdownChange))] float laserCountdown = 0f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject laserPoint;

    [SerializeField] [SyncVar(hook = nameof(OnItemChange))] Items currentItem = Items.None;

    [SyncVar(hook = nameof(OnUsernameChange))] string usernameText;

    // Lap Manager
    [SerializeField] GameObject lapTextRenderer;
    int currentLap = 1;
    LapManager lapManager;
    int checkpointCount = 0;
    Vector2 spawnPos = Vector2.zero;
    float spawnRotation = 0;

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
        currentState = PlayerStates.Grounded;
        spawnPos = transform.position;
        grounded = 0;
        rb.rotation = 90;
        currentRotationSpeed = rotationSpeed;
        currentDrag = drag;
        dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Initialize(gameObject);
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
    }

    protected override void FSMFixedUpdate()
    {
        Rendering();
        if (!isLocalPlayer) { return; }
        Move();
    }

    private void CheckState()
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
            CmdLaserHit(transform.position + new Vector3(direction.x, direction.y, 0) * 2, direction);
            firingLaser = false;
            lineRenderer.enabled = false;
            currentItem = Items.None;
            itemSprite.sprite = none;
            CmdUseItem();
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
            SetLap();
            if (!lapTextRenderer.activeSelf)
            {
                lapTextRenderer.SetActive(true);
            }
            enemyInterface.enabled = false;
        }
        if (isLocalPlayer && GameStateManager.GetInstance().gameState != GameStateManager.GameState.Running)
        {
            if (lapTextRenderer.activeSelf)
            {
                lapTextRenderer.SetActive(false);
            }
            enemyInterface.enabled = false;
        }
    }

    public void Restart()
    {
        transform.position = spawnPos;
        transform.eulerAngles = new Vector3(0, 0, spawnRotation);
        CmdSetStats(DataManager.MAX_SPEED + dataManager.speed, DataManager.ACCELERATION + dataManager.acceleration, DataManager.WEIGHT + dataManager.weight, DataManager.HANDLING + dataManager.handling);
        healthBar.value = 0f;
        playerHealthBar.value = 0f;
        rb.velocity = Vector3.zero;
        rb.rotation = 90;
        currentState = PlayerStates.Grounded;
        sr.transform.localScale = Vector3.one;
        CmdSetScale(sr.transform.localScale.x);
        xboxController = DataManager.GetInstance().xboxController;
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
                Restart();
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

            if (Input.GetKey(KeyCode.S) || Input.GetAxis("Strafe1") < -0.1)
            {
                strafingLeft = true;
                strafeValue = Input.GetAxis("Strafe1");
            }
            else
            {
                strafingLeft = false;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetAxis("Strafe1") > 0.1)
            {
                strafingRight = true;
                strafeValue = Input.GetAxis("Strafe1");
            }
            else
            {
                strafingRight = false;
            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
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
                Restart();
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

            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                UseItem();
            }
        }
    }

    private void Move()
    {
        switch (currentState)
        {
            case PlayerStates.None:

                break;

            case PlayerStates.Grounded:
                SetLayer(6);
                sr.sortingLayerID = SortingLayer.NameToID("Players");
                direction = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
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
                        rb.AddForce(rb.velocity.normalized * -(currentDrag * 3));
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

                    if (strafingLeft && !strafingRight)
                    {
                        currentMaxSpeed = maxSpeed + (15 * rotationSpeed);
                        float newAngle = rb.rotation - (strafingAngle * strafeValue);
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;
                    }
                    else if (strafingRight && !strafingLeft)
                    {
                        currentMaxSpeed = maxSpeed + (15 * rotationSpeed);
                        float newAngle = rb.rotation - (strafingAngle * strafeValue);
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;
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
                    rb.rotation += (rolling ? 6f : (3f - strafeValue * 2f)) * currentRotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
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
                    float newAngle = rb.rotation + (60 * strafeValue);
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    //rb.velocity = tempDirection * rb.velocity.magnitude;
                    rb.AddForce(tempDirection * (10 + rb.velocity.magnitude / 5) * strafeValue * rotationSpeed);
                }
                else if (strafingRight && !strafingLeft)
                {
                    float newAngle = rb.rotation - (60 * strafeValue);
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    //rb.velocity = tempDirection * rb.velocity.magnitude;
                    rb.AddForce(tempDirection * (10 + rb.velocity.magnitude / 5) * strafeValue * rotationSpeed);
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

    [Command]
    private void CmdSetStats(float speed, float accel, float weight, float handling)
    {
        this.maxSpeed = (speed * 1.5f);
        this.acceleration = accel;
        this.weight = weight;
        this.rotationSpeed = handling;

        maxHealth = 20 + weight * 40;

        health = maxHealth;
        CmdChangeHealth(maxHealth);
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
        laserCountdown = (float)NetworkTime.time + 0.5f;
        firingLaser = true;
    }

    [Command]
    private void CmdLaserHit(Vector3 pos, Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 10000, ~6);
        if (hit != null && hit.collider.tag == "Player")
        {
            RpcLaserHit(hit.point);
        }
        firingLaser = false;
    }

    [ClientRpc]
    private void RpcLaserHit(Vector2 hitPoint)
    {
        GameObject las = Instantiate(laserHit, hitPoint, Quaternion.identity);
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
        }
        else
        {
            lineRenderer.startColor = firingLaser ? Color.cyan : Color.red;
            position = transform.position + new Vector3(direction.x, direction.y, 0) * 1000;
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, position);
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

    public void Hit(float damage)
    {
        if (shielded > Time.time) { return; }
        CmdChangeHealth(health - damage);
    }

    public void Hit(float damage, float slow, float duration)
    {
        if (shielded > Time.time) { return; }
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
                    itemPosition = transform.position - new Vector3(rb.velocity.normalized.x, rb.velocity.normalized.y, 1) * 4;
                }
                else
                {
                    itemPosition = transform.position - new Vector3(direction.x, direction.y, 1) * 3;
                }
                CmdTrap(itemPosition);
                break;

            case Items.Rebounder:
                itemPosition = transform.position + new Vector3(direction.x, direction.y, 0) * 5;
                CmdRebounder(itemPosition, direction * 40 + rb.velocity, currentState, airborne);
                break;

            case Items.Laser:
                CmdLaser();
                return;
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
            float weightDifference = collision.gameObject.GetComponent<MultiplayerController>().weight - weight;
            if (weightDifference < 0.5f)
            {
                weightDifference = 0.5f;
            }
            if (currentState == PlayerStates.Jumping)
            {
                bounceTime = Time.time + bounceDuration;
                rb.AddForce(collision.contacts[0].normal * 800 * weightDifference);
                CmdChangeHealth(health - 10 - (6 * weightDifference));
            } else if (currentState == PlayerStates.Grounded)
            {
                bounceTime = Time.time + bounceDuration;
                rb.AddForce(collision.contacts[0].normal * 500 * weightDifference);
                CmdChangeHealth(health - 5 - (3 * weightDifference));
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
            collision.gameObject.GetComponent<ItemBox>().Use(this);
        }
        else if (collision.tag == "Trap")
        {
            if (currentState == PlayerStates.Jumping) { return; }
            NetworkServer.Destroy(collision.gameObject);
            Hit(20, 30, 6);
        }
        else if (collision.tag == "Laser")
        {
            NetworkServer.Destroy(collision.gameObject);
            Hit(90, 40, 6);
        }
        else if (collision.tag == "Booster")
        {
            bounceTime = Time.time;
            rb.rotation = Mathf.Asin(collision.gameObject.transform.rotation.z) * Mathf.Rad2Deg * 2 * Mathf.Sign(collision.gameObject.transform.rotation.w);
            rb.velocity = new Vector2(collision.gameObject.transform.rotation.w, collision.gameObject.transform.rotation.z) * 20 + new Vector2(collision.gameObject.transform.rotation.w, collision.gameObject.transform.rotation.z) * rb.velocity.magnitude;
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
                if (lapManager.CheckNextLap(checkpointCount))
                {
                    if (collision.transform.eulerAngles.z == normalFloat)
                    {
                        currentLap++;
                    }
                }
                checkpointCount = 0;
            } else
            {
                collision.gameObject.SetActive(false);
                if (Mathf.Round(collision.transform.eulerAngles.z) == normalFloat)
                {
                    checkpointCount++;
                }
            }
        }
        else if (collision.tag == "Ice")
        {
            currentDrag = 0;
            currentRotationSpeed = rotationSpeed + 1;
        }
        else if (collision.tag == "Ground")
        {
            Debug.Log("Enter");
            grounded++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ramp")
        {
            Jump(false);
            Debug.Log("Jump");
        } else if (collision.tag == "Heal" && currentState == PlayerStates.Grounded)
        {
            health = health + 0.1f * rb.velocity.magnitude / 3;
            CmdChangeHealth(health);
        }
        else if (collision.tag == "Booster")
        {
            rb.rotation = Mathf.Asin(collision.gameObject.transform.rotation.z) * Mathf.Rad2Deg * 2 * Mathf.Sign(collision.gameObject.transform.rotation.w);
            rb.velocity = new Vector2(collision.gameObject.transform.rotation.w, collision.gameObject.transform.rotation.z) * rb.velocity.magnitude;
        }
        else if (collision.tag == "Gravel")
        {
            if (rb.velocity.magnitude > currentMaxSpeed / 2)
            {
                rb.AddForce(rb.velocity.normalized * -(currentDrag * 6));
            }
        }
        else if (collision.tag == "Blaster" && currentState == PlayerStates.Grounded)
        {
            health = health - 0.3f;
            CmdChangeHealth(health);
        }
        else if (collision.tag == "Ground")
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
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
