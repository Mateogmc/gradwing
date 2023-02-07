using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float weight;
    [SerializeField] float turnSpeed;
    [SerializeField] float drag;
    [SerializeField] float rotationSpeed;
    [SerializeField] float strafingAngle;
    [SerializeField] GameObject followDummy;

    float currentSpeed = 0;
    float bounceDuration = 0.5f;
    public float bounceTime = 0f;
    Vector2 direction;
    public Vector2 lastSpeed;

    bool accelerating = false;
    bool braking = false;
    public bool rotatingRight = false;
    public bool rotatingLeft = false;
    public bool strafingRight = false;
    public bool strafingLeft = false;
    bool backwards = false;
    public bool rolling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckInput();
        currentSpeed = rb.velocity.magnitude;
        lastSpeed = rb.velocity;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick2Button7))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Joystick2Button0))
        {
            accelerating = true;
        } else
        {
            accelerating = false;
        }

        if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.Joystick2Button1))
        {
            braking = true;
        } else
        {
            braking = false;
        }

        if (accelerating && braking)
        {
            rolling = true;
        } else
        {
            rolling = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal2") < -0.2)
        {
            rotatingLeft = true;
        } else
        {
            rotatingLeft = false;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal2") > 0.2)
        {
            rotatingRight = true;
        } else
        {
            rotatingRight = false;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetAxis("Strafe2") < -0.1)
        {
            strafingLeft = true;
        }
        else
        {
            strafingLeft = false;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetAxis("Strafe2") > 0.1)
        {
            strafingRight = true;
        }
        else
        {
            strafingRight = false;
        }
    }

    void Move()
    {
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
            if (currentSpeed > 1)
            {
                rb.AddForce(direction * -(drag * 3));
            } else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            if (currentSpeed > 0.05)
            {
                rb.AddForce(direction * -drag);
            }
        }

        if (!rolling && bounceTime < Time.time)
        {

            if (strafingLeft && !strafingRight)
            {
                float newAngle = rb.rotation + strafingAngle;
                Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                rb.velocity = tempDirection * rb.velocity.magnitude;
            }
            else if (strafingRight && !strafingLeft)
            {
                float newAngle = rb.rotation - strafingAngle;
                Vector2 tempDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
                rb.velocity = tempDirection * rb.velocity.magnitude;
            }
            else
            {
                rb.velocity = direction * rb.velocity.magnitude;
            }
        } else if (bounceTime > Time.time)
        {
            if (currentSpeed > 1)
            {
                rb.AddForce(rb.velocity.normalized * -drag * 4);
            }
        }

        if (rotatingLeft && !rotatingRight)
        {
            Debug.Log(2 + Input.GetAxis("Strafe1"));
            rb.rotation += (rolling ? 4f : (3f - Input.GetAxis("Strafe2") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal2"));
        } else if (rotatingRight && !rotatingLeft)
        {
            Debug.Log(2 + Input.GetAxis("Strafe1"));
            rb.rotation -= (rolling ? 4f : (3f + Input.GetAxis("Strafe2") * 2f)) * rotationSpeed * Mathf.Abs(Input.GetAxis("Horizontal2"));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceTime = Time.time + bounceDuration;
        Debug.Log(collision.contacts[0].normal);
        rb.velocity = Vector2.Reflect(lastSpeed, collision.contacts[0].normal);
        //rb.rotation = Vector2.SignedAngle(Vector2.right, rb.velocity);
        Debug.Log(rb.rotation);
    }
}
