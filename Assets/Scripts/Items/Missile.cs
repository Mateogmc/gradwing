using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Missile : NetworkBehaviour
{
    GameObject target;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        cc.enabled = false;

        target = FindFirstPlayer();
        
        StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(0.2f);
        cc.enabled = true;
        while (true)
        {
            target = FindFirstPlayer();
            yield return new WaitForSeconds(3);
        }
    }

    private GameObject FindFirstPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<MultiplayerController>().placement == 1)
            {
                return p;
            }
        }
        return null;
    }

    private void FixedUpdate()
    {
        Vector2 vectorDirection = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized;
        float targetDirection = Vector2.SignedAngle(vectorDirection, Vector2.right);

        float rotateAmount = Vector3.Cross(vectorDirection, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotationSpeed;

        rb.velocity = transform.up * speed;
    }
}
