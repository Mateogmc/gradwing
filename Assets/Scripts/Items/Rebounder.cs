using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebounder : MonoBehaviour
{
    Vector2 lastSpeed;
    public PlayerStates currentState;
    float airborne;
    bool grounded = true;
    [SerializeField] int bouncesLeft;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    float currentScale = 1;

    public void InitializeRebounder(Vector2 velocity, PlayerStates playerState, float airborne)
    {
        rb.velocity = velocity;
        currentState = playerState;
        if (playerState == PlayerStates.Jumping)
        {
            this.airborne = airborne;
            gameObject.layer = 9;
        }
    }

    private void Update()
    {
        lastSpeed = rb.velocity;
        rb.angularVelocity = 500f;
    }

    private void FixedUpdate()
    {
        StateCheck();
    }

    private void StateCheck()
    {
        if (currentState == PlayerStates.Dead)
        {
            if (sr.transform.localScale.x > 0)
            {
                currentScale -= 0.05f;
                transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            } else
            {
                Destroy(gameObject);
            }
        } else
        {
            if (airborne > 0)
            {
                airborne -= 0.05f;
                currentScale = 1 + Mathf.Sin(airborne);
                transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }
            else
            {
                if (!grounded)
                {
                    currentState = PlayerStates.Dead;
                }
                else
                {
                    sr.sortingLayerID = SortingLayer.NameToID("Players");
                    currentState = PlayerStates.Grounded;
                    gameObject.layer = 7;
                    currentScale = 1;
                }
            }
        }
    }

    private void Jump()
    {
        if (currentState == PlayerStates.Grounded)
        {
            sr.sortingLayerID = SortingLayer.NameToID("Foreground");
            currentState = PlayerStates.Jumping;
            airborne = Mathf.PI;
            gameObject.layer = 9;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Wall")
        {
            if (bouncesLeft > 0)
            {
                bouncesLeft--;

                transform.position += new Vector3(collision.contacts[0].normal.x, collision.contacts[0].normal.y, 0);
                rb.velocity = Vector2.Reflect(lastSpeed, collision.contacts[0].normal);
            } else
            {
                Destroy(gameObject);
            }
        } 
        else if (collision.gameObject.tag == "Rebounder")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Trap")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.tag == "Ramp")
        {
            Jump();
        }
        else if (collision.tag == "Ground")
        {
            grounded = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground" && !grounded)
        {
            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            grounded = false;
            if (currentState == PlayerStates.Grounded)
            {
                Destroy(gameObject);
            }
        }
    }
}
