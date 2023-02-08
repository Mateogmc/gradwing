using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MultiplayerController : FSM
{
    public enum PlayerStates
    {
        None, Grounded, Jumping, Dead
    }

    public PlayerStates currentState = PlayerStates.Grounded;

    [Header("Inspector variables")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask playerLayer;


    [Header("Player variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float weight;
    [SerializeField] float turnSpeed;
    [SerializeField] float drag;
    [SerializeField] float rotationSpeed;
    [SerializeField] float strafingAngle;

    float currentSpeed = 0;
    float bounceDuration = 0.5f;
    [HideInInspector] public float bounceTime = 0f;
    float airborne = 0f;
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

    protected override void Initialize()
    {
        if (!isLocalPlayer) { return; }
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
        if (!isLocalPlayer) { return; }
        Move();
    }

    private void CheckState()
    {
        if (health <= 0f)
        {
            currentState = PlayerStates.Dead;
        }
    }

    private void Restart()
    {
        transform.position = new Vector3(75, -40, 2);
        health = 100f;
        rb.velocity = Vector3.zero;
        rb.rotation = 0;
        currentState = PlayerStates.Grounded;
        sr.transform.localScale = Vector3.one;
    }

    private void OnHealthChange(float oldHealth, float newHealth)
    {
        healthBar.value = 100 - newHealth;
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
    }

    private void Move()
    {
        switch (currentState)
        {
            case PlayerStates.None:

                break;

            case PlayerStates.Grounded:
                direction = new Vector2(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad));
                if (rolling)
                {
                    if (currentSpeed > 1)
                    {
                        rb.AddForce(rb.velocity.normalized * -(drag * 2));
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (accelerating && !braking)
                {
                    if (rb.velocity.magnitude < maxSpeed)
                    {
                        rb.AddForce(direction * acceleration);
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

                if (!rolling && bounceTime < Time.time)
                {

                    if (strafingLeft && !strafingRight)
                    {
                        float newAngle = rb.rotation - (strafingAngle * Input.GetAxis("Strafe1"));
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;
                    }
                    else if (strafingRight && !strafingLeft)
                    {
                        float newAngle = rb.rotation - (strafingAngle * Input.GetAxis("Strafe1"));
                        Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                        rb.velocity = tempDirection * rb.velocity.magnitude;
                    }
                    else
                    {
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
                    rb.rotation += (rolling ? 4f : (3f - Input.GetAxis("Strafe1") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                else if (rotatingRight && !rotatingLeft)
                {
                    rb.rotation -= (rolling ? 4f : (3f + Input.GetAxis("Strafe1") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal1"));
                }
                break;

            case PlayerStates.Jumping:
                if (airborne > 0)
                {
                    airborne -= 0.05f;
                    sr.transform.localScale = new Vector3(1 + Mathf.Sin(airborne), 1 + Mathf.Sin(airborne), 1);
                } else
                {
                    currentState = PlayerStates.Grounded;
                    Physics2D.IgnoreLayerCollision(3, 6, false);
                    sr.transform.localScale = new Vector3(1, 1, 1);

                    CheckOffRoad(new Vector2(transform.position.x, transform.position.y), 0);
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
                    float newAngle = rb.rotation - (strafingAngle * Input.GetAxis("Strafe1"));
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    rb.velocity = tempDirection * rb.velocity.magnitude;
                }
                else if (strafingRight && !strafingLeft)
                {
                    float newAngle = rb.rotation - (strafingAngle * Input.GetAxis("Strafe1"));
                    Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                    rb.velocity = tempDirection * rb.velocity.magnitude;
                }
                break;

            case PlayerStates.Dead:
                if (sr.transform.localScale.x > 0)
                {
                    sr.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
                    rb.velocity = Vector3.zero;
                }
                break;
        }
    }

    private void CheckOffRoad(Vector2 start, int iterations)
    {
        GetComponent<Collider2D>().enabled = false;
        RaycastHit2D onRoad = Physics2D.Raycast(start, Vector2.up, 10000f);
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
                Debug.Log("Fuera");
            }
        }
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
    private void CmdChangeHealth(float newHealth)
    {
        health = newHealth;
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
            bounceTime = Time.time + bounceDuration;
            rb.AddForce(collision.contacts[0].normal * 500);
            CmdChangeHealth(health - 5);
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
            Debug.Log("Item");
        }
    }
}
