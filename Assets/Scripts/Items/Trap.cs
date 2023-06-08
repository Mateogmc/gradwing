using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    float airborne = 0;
    float grounded = 0;
    float currentScale = 1;
    Collider2D parentPlayer;
    [SerializeField] TrailRenderer tr;
    [SerializeField] Rigidbody2D rb;

    public void Initialize(PlayerStates state, float airborne, bool thrown, Vector2 velocity, Collider2D parentPlayer)
    {
        if (thrown)
        {
            rb.velocity = velocity;
            this.airborne = Mathf.PI;
            gameObject.layer = 9;
        }
        else
        {
            if (state == PlayerStates.Jumping)
            {
                this.airborne = airborne;
                gameObject.layer = 9;
            }
        }
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), parentPlayer, true);
        this.parentPlayer = parentPlayer;
    }

    private void FixedUpdate()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (airborne <= 0 && grounded == 0)
        {
            currentScale -= 0.1f;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            if (currentScale <= 0)
            {
                tr.gameObject.transform.parent = null;
                tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
                Destroy(gameObject);
            }
        }
        else if (airborne > 0)
        {
            airborne -= 0.1f;
            currentScale = 1 + Mathf.Sin(airborne);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
        else
        {
            if (currentScale != 1)
            {
                currentScale = 1;
            }
            if (rb.velocity != Vector2.zero)
            {
                rb.velocity = Vector2.zero;
            }
            gameObject.layer = 7;
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), parentPlayer, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded++;
        }
        if (collision.CompareTag("Player") || collision.CompareTag("Rebounder") || collision.CompareTag("Explosion") || collision.CompareTag("Rebounder"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded--;
        }
    }
}
