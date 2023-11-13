using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    float airborne = 0;
    float grounded = 0;
    float currentScale = 1;
    [SerializeField] float gravity;
    [SerializeField] float launchForce;
    Collider2D parentPlayer;
    [SerializeField] TrailRenderer tr;
    [SerializeField] Rigidbody2D rb;
    public string username;

    public void Initialize(PlayerStates state, float airborne, bool thrown, Vector2 velocity, Collider2D parentPlayer, string parentUsername)
    {
        if (thrown)
        {
            rb.velocity = velocity;
            this.airborne = launchForce;
            gameObject.layer = 9;
        }
        else
        {
            if (state == PlayerStates.Jumping)
            {
                this.airborne = launchForce;
                currentScale = airborne;
                gameObject.layer = 9;
            }
        }

        username = parentUsername;

        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), parentPlayer, true);
        this.parentPlayer = parentPlayer;
        StartCoroutine(StartCollision());
    }
    
    private IEnumerator StartCollision()
    {
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), parentPlayer, false);
    }

    private void FixedUpdate()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (airborne <= 0  && currentScale <= 1 && grounded == 0)
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
        else if (currentScale > 1 || airborne > 0)
        {
            airborne -= gravity;
            currentScale += airborne;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            Debug.Log(currentScale);
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Fan"))
        {
            grounded++;
        }
        if (collision.CompareTag("Player") || collision.CompareTag("Rebounder") || collision.CompareTag("Explosion") || collision.CompareTag("Rebounder"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Bouncer"))
        {
            rb.velocity = Vector2.Reflect(rb.velocity, (collision.ClosestPoint(transform.position) - new Vector2(transform.position.x, transform.position.y)).normalized);
            VFXManager.instance.BounceRipple(collision.ClosestPoint(transform.position), collision.ClosestPoint(transform.position) - new Vector2(transform.position.x, transform.position.y));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fan"))
        {
            if (currentScale <= 1)
            {
                collision.GetComponent<Fan>().Stop();
                tr.gameObject.transform.parent = null;
                tr.gameObject.GetComponent<DestroyDelay>().DestroyAfterDelay(1);
                Destroy(gameObject);
            }
            else if (collision.gameObject.GetComponent<Fan>().force < 0)
            {
                airborne += collision.gameObject.GetComponent<Fan>().force / 1000;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Fan"))
        {
            grounded--;
        }
    }
}
