using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebounder : MonoBehaviour
{
    Vector2 lastSpeed;
    public PlayerStates currentState;
    float airborne;
    [SerializeField] int bouncesLeft;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] TrailRenderer tr;
    [SerializeField] Material trMaterial;
    [SerializeField] float minVelocity;
    Material currentMaterial;
    float currentScale = 1;
    int grounded;
    Collider2D parentPlayer;

    public void InitializeRebounder(Vector2 velocity, PlayerStates playerState, float airborne, Collider2D parentPlayer)
    {
        rb.velocity = velocity;
        if (rb.velocity.magnitude < minVelocity)
        {
            rb.velocity = rb.velocity.normalized * minVelocity;
        }
        currentState = playerState;
        if (playerState == PlayerStates.Jumping)
        {
            this.airborne = airborne;
            gameObject.layer = 9;
        }

        this.parentPlayer = parentPlayer;
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), parentPlayer, true);

        Physics2D.IgnoreLayerCollision(7, 6, false);
        Physics2D.IgnoreLayerCollision(9, 8, false);
    }

    private void Start()
    {
        GetComponent<AudioSource>().volume = DataManager.soundVolume;
        currentMaterial = new Material(trMaterial);
        currentMaterial.SetColor("_TrailColor", new Vector4(100, 20, 35, 0.1f));
        tr.material = currentMaterial;
    }

    private void Update()
    {
        lastSpeed = rb.velocity;
        rb.angularVelocity = 500f;
        tr.widthMultiplier = currentScale;
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
                tr.gameObject.transform.parent = null;
                tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
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
                if (grounded < 0)
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

                rb.velocity = Vector2.Reflect(lastSpeed, collision.contacts[0].normal);
            } else
            {
                tr.gameObject.transform.parent = null;
                tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
                Destroy(gameObject);
            }
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), parentPlayer, false);
        } 
        else if (collision.gameObject.tag == "Rebounder")
        {
            tr.gameObject.transform.parent = null;
            tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
            tr.gameObject.transform.parent = null;
            tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Trap" || collision.tag == "Explosion")
        {
            Destroy(collision.gameObject);
            tr.gameObject.transform.parent = null;
            tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
            Destroy(gameObject);
        }
        else if (collision.tag == "Ramp")
        {
            Jump();
        }
        else if (collision.tag == "Ground")
        {
            grounded++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            grounded--;
            if (currentState == PlayerStates.Grounded && grounded <= 0 && airborne <= 0)
            {
                currentState = PlayerStates.Dead;
            }
        }
    }
}
