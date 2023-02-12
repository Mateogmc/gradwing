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


    [Header("Player variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float currentMaxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float weight;
    [SerializeField] float turnSpeed;
    [SerializeField] float drag;
    [SerializeField] float rotationSpeed;
    [SerializeField] float strafingAngle;
    [SerializeField] float shieldDuration;

    float currentSpeed = 0;
    float bounceDuration = 0.5f;
    [HideInInspector] public float bounceTime = 0f;
    float airborne = 0f;
    [SyncVar(hook = nameof(OnAirborneChange))] float currentScale = 1;
    Vector2 direction;
    [HideInInspector] public Vector2 lastSpeed;

    [HideInInspector]  bool accelerating = false;
    [HideInInspector] bool braking = false;
    [HideInInspector] public bool rotatingRight = false;
    [HideInInspector] public bool rotatingLeft = false;
    [HideInInspector] public bool strafingRight = false;
    [HideInInspector] public bool strafingLeft = false;
    [HideInInspector] public bool rolling;


    [SerializeField] [SyncVar(hook = nameof(OnHealthChange))] float health;
    [SerializeField] Slider healthBar;
    [SerializeField] Canvas ui;
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] SpriteRenderer shieldRenderer;

    [Header("Item Sprites")]
    [SerializeField] Sprite none;
    [SerializeField] Sprite shield;
    [SerializeField] Sprite jump;
    [SerializeField] Sprite trap;
    [SerializeField] Sprite rebounder;
    [SerializeField] Sprite laser;
    [SerializeField] Image itemSprite;

    [SyncVar(hook = nameof(OnShieldChange))]float shielded = 0f;
    float slowTimer = 0f;

    [SerializeField] GameObject trapPrefab;

    [SerializeField] GameObject rebounderPrefab;
    Vector3 itemPosition;

    [SerializeField] [SyncVar(hook = nameof(OnItemChange))] Items currentItem = Items.None;

    [SyncVar(hook = nameof(OnUsernameChange))] string usernameText;

    protected override void Initialize()
    {
        if (!isLocalPlayer) { return; }
        usernameText = DataManager.username;
        username.text = usernameText;
        CmdSetName(usernameText);
        currentState = PlayerStates.Grounded;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().Initialize(gameObject);
    }

    protected override void FSMUpdate()
    {
        ui.transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        if (!isLocalPlayer) { return; }
        CheckInput();
        CheckState();
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
        if (health <= 0f)
        {
            currentState = PlayerStates.Dead;
        }
        if (username.text != usernameText)
        {
            username.text = usernameText;
        }
        if (currentState == PlayerStates.Jumping && airborne <= 0 && !CheckOffRoad(new Vector2(transform.position.x, transform.position.y), 0))
        {
            currentState = PlayerStates.Grounded;
        }
        if (currentMaxSpeed < maxSpeed && slowTimer < Time.time)
        {
            currentMaxSpeed = maxSpeed;
        }
        if (currentState == PlayerStates.Dead) {
            Physics2D.IgnoreLayerCollision(6, 6, true);
        } else
        {
            Physics2D.IgnoreLayerCollision(6, 6, false);
        }
    }

    private void Rendering()
    {
        if (shielded > Time.time)
        {
            shieldRenderer.enabled = !shieldRenderer.enabled;
            Debug.Log(shieldDuration);
        }
        else if (shieldRenderer.enabled)
        {
            shieldRenderer.enabled = false;
            Debug.Log(shielded - Time.time);
        }
    }

    private void Restart()
    {
        transform.position = new Vector3(75, -40, 2);
        health = 100f;
        CmdChangeHealth(100);
        healthBar.value = 0f;
        rb.velocity = Vector3.zero;
        rb.rotation = 0;
        currentState = PlayerStates.Grounded;
        sr.transform.localScale = Vector3.one;
        CmdSetScale(sr.transform.localScale.x);
    }

    private void OnHealthChange(float oldHealth, float newHealth)
    {
        healthBar.value = 100 - newHealth;
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

    private void OnItemChange(Items oldItem, Items newItem)
    {
        switch (newItem)
        {
            case Items.None:
                itemSprite.sprite = none;
                currentItem = Items.None;
                break;

            case Items.Shield:
                itemSprite.sprite = shield;
                currentItem = Items.Shield;
                break;

            case Items.Jump:
                itemSprite.sprite = jump;
                currentItem = Items.Jump;
                break;

            case Items.Trap:
                itemSprite.sprite = trap;
                currentItem = Items.Trap;
                break;

            case Items.Rebounder:
                itemSprite.sprite = rebounder;
                currentItem = Items.Rebounder;
                break;

            case Items.Laser:
                itemSprite.sprite = laser;
                currentItem = Items.Laser;
                break;
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            if (currentState == PlayerStates.Dead)
            {
                Restart();
            } else
            {
                Application.Quit();
            }
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

        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            UseItem();
        }
    }

    private void Move()
    {
        switch (currentState)
        {
            case PlayerStates.None:

                break;

            case PlayerStates.Grounded:
                Physics2D.IgnoreLayerCollision(3, 6, false);
                direction = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
                if (rolling)
                {
                    if (currentSpeed > 1)
                    {
                        rb.AddForce(rb.velocity.normalized * -(drag));
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
                        rb.AddForce(rb.velocity.normalized * -(drag / 3));
                    }
                }
                else if (braking)
                {
                    if (currentSpeed > 0.05)
                    {
                        rb.AddForce(rb.velocity.normalized * -(drag * 3));
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
                        rb.AddForce(rb.velocity.normalized * -drag);
                    }
                }

                if (currentMaxSpeed < maxSpeed)
                {
                    rb.AddForce(rb.velocity.normalized * -(drag * 5));
                }

                if (!rolling && bounceTime < Time.time)
                {

                    if (strafingLeft && !strafingRight)
                    {
                        currentMaxSpeed = maxSpeed + 30;
                        float newAngle = rb.rotation - (strafingAngle * Input.GetAxis("Strafe1"));
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;
                    }
                    else if (strafingRight && !strafingLeft)
                    {
                        currentMaxSpeed = maxSpeed + 30;
                        float newAngle = rb.rotation - (strafingAngle * Input.GetAxis("Strafe1"));
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
                        rb.AddForce(rb.velocity.normalized * -drag * 4);
                    }
                }

                if (rotatingLeft && !rotatingRight)
                {
                    rb.rotation += (rolling ? 6f : (3f - Input.GetAxis("Strafe1") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 6f : (3f + Input.GetAxis("Strafe1") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                break;

            case PlayerStates.Jumping:
                if (airborne > 0)
                {
                    airborne -= 0.05f;
                    sr.transform.localScale = new Vector3(1 + Mathf.Sin(airborne), 1 + Mathf.Sin(airborne), 1);
                    CmdSetScale(sr.transform.localScale.x);
                    //CmdSetScale(sr.transform.localScale);
                } else
                {
                    currentState = PlayerStates.Grounded;
                    Physics2D.IgnoreLayerCollision(3, 6, false);
                    sr.transform.localScale = new Vector3(1, 1, 1);
                }
                if (rotatingLeft && !rotatingRight)
                {
                    rb.rotation += (rolling ? 4f : (3f - Input.GetAxis("Strafe1") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 4f : (3f + Input.GetAxis("Strafe1") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                if (strafingLeft && !strafingRight)
                {
                    float newAngle = rb.rotation + (60 * Input.GetAxis("Strafe1"));
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    //rb.velocity = tempDirection * rb.velocity.magnitude;
                    rb.AddForce(tempDirection * rb.velocity.magnitude * Input.GetAxis("Strafe1"));
                }
                else if (strafingRight && !strafingLeft)
                {
                    float newAngle = rb.rotation - (60 * Input.GetAxis("Strafe1"));
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    //rb.velocity = tempDirection * rb.velocity.magnitude;
                    rb.AddForce(tempDirection * (10 + rb.velocity.magnitude / 5) * Input.GetAxis("Strafe1"));
                }
                break;

            case PlayerStates.Dead:
                if (sr.transform.localScale.x > 0)
                {
                    sr.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
                    CmdSetScale(sr.transform.localScale.x);
                }
                CmdChangeHealth(health);
                rb.velocity = Vector3.zero;
                break;
        }
    }

    private bool CheckOffRoad(Vector2 start, int iterations)
    {
        GetComponent<Collider2D>().enabled = false;
        RaycastHit2D onRoad = Physics2D.Raycast(start, Vector2.up, 10000f, ~3);
        GetComponent<Collider2D>().enabled = true;
        if (onRoad.collider != null && iterations < 41)
        {
            iterations++;
            CheckOffRoad(onRoad.point + new Vector2(0, 0.01f), iterations);
        } else
        {
            if (iterations % 2 == 0)
            {
                health = 0;
                return true;
            }
        }
        return false;
    }

    private void Jump()
    {
        if (currentState == PlayerStates.Grounded)
        {
            currentState = PlayerStates.Jumping;
            airborne = Mathf.PI;
            Physics2D.IgnoreLayerCollision(3, 6, true);
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
        GameObject trp = Instantiate(trapPrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(trp);
    }

    [Command]
    private void Rebounder(Vector3 pos, Vector2 initialVelocity, PlayerStates state, float airborne)
    {
        GameObject reb = Instantiate(rebounderPrefab, pos, Quaternion.identity);
        reb.GetComponent<Rebounder>().InitializeRebounder(initialVelocity, state, airborne);
        NetworkServer.Spawn(reb);
    }

    private void Laser()
    {

    }

    private void Hit(float damage)
    {
        if (shielded > Time.time) { return; }
        CmdChangeHealth(health - damage);
    }

    private void Hit(float damage, float slow, float duration)
    {
        if (shielded > Time.time) { return; }
        Hit(damage);
        currentMaxSpeed -= slow;
        slowTimer = Time.time + duration;
        rb.AddForce(-rb.velocity);
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
                Jump();
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
                itemPosition = transform.position + new Vector3(direction.x, direction.y, 0) * ((rb.velocity.magnitude / 5) + 1);
                Rebounder(itemPosition, direction * 30 + rb.velocity, currentState, airborne);
                break;

            case Items.Laser:

                break;
        }
        currentItem = Items.None;
        itemSprite.sprite = none;
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
                break;

            case "Jump":
                currentItem = Items.Jump;
                itemSprite.sprite = jump;
                break;

            case "Trap":
                currentItem = Items.Trap;
                itemSprite.sprite = trap;
                break;

            case "Rebounder":
                currentItem = Items.Rebounder;
                itemSprite.sprite = rebounder;
                break;

            case "Laser":
                currentItem = Items.Laser;
                itemSprite.sprite = laser;
                break;
        }
    }

    [Command]
    private void CmdUseItem()
    {
        currentItem = Items.None;
        itemSprite.sprite = none;
    }

    [Command]
    private void CmdChangeHealth(float newHealth)
    {
        if (newHealth > 100) { newHealth = 100; }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer) { return; }
        if (collision.collider.tag == "Wall")
        {
            bounceTime = Time.time + bounceDuration;
            rb.velocity = Vector2.Reflect(lastSpeed, collision.contacts[0].normal);
            CmdChangeHealth(health - 10);
            //rb.rotation = Vector2.SignedAngle(Vector2.right, rb.velocity);
        }
        else if (collision.collider.tag == "Player")
        {
            if (currentState == collision.gameObject.GetComponent<MultiplayerController>().currentState)
            {
                bounceTime = Time.time + bounceDuration;
                rb.AddForce(collision.contacts[0].normal * 500);
                CmdChangeHealth(health - 5);
            } else if (currentState == PlayerStates.Grounded)
            {
                bounceTime = Time.time + bounceDuration;
                rb.AddForce(collision.contacts[0].normal * 500);
                CmdChangeHealth(health - 15);
            }
        }
        else if (collision.collider.tag == "Rebounder")
        {
            if (currentState == collision.gameObject.GetComponent<Rebounder>().currentState)
            {
                CmdRemoveItem(collision.gameObject);
                bounceTime = Time.time + bounceDuration;
                rb.AddForce(collision.contacts[0].normal * 1000);
                Hit(40, 10, 5);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ramp")
        {
            Jump();
        }
        else if (collision.tag == "Item")
        {
            collision.gameObject.GetComponent<ItemBox>().Use(this);
        }
        else if (collision.tag == "Trap")
        {
            if (currentState == PlayerStates.Jumping) { return; }
            NetworkServer.Destroy(collision.gameObject);
            Hit(20, 30, 6);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Heal" && currentState == PlayerStates.Grounded)
        {
            CmdChangeHealth(health + 0.1f * rb.velocity.magnitude / 3);
        }
    }
}
