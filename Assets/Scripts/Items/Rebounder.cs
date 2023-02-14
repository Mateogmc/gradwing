using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Rebounder : NetworkBehaviour
{
    Vector2 lastSpeed;
    public PlayerStates currentState;
    float airborne;
    [SerializeField] int bouncesLeft;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SyncVar(hook = nameof(OnAirborneChange))] float currentScale = 1;

    public void InitializeRebounder(Vector2 velocity, PlayerStates playerState, float airborne)
    {
        rb.velocity = velocity;
        currentState = playerState;
        if (playerState == PlayerStates.Jumping)
        {
            this.airborne = airborne;
            currentScale = 1 + Mathf.Sin(airborne);
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
            }
            else
            {
                if (CheckOffRoad(transform.position, 0))
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

    private void OnAirborneChange(float oldAir, float newAir)
    {
        sr.transform.localScale = new Vector3(newAir, newAir, newAir);
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
        }
        else
        {
            if (iterations % 2 == 0)
            {
                return true;
            }
        }
        return false;
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

    [Command]
    private void CmdSetScale(float scale)
    {
        currentScale = scale;
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
    }
}
