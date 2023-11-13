using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Missile : NetworkBehaviour
{
    GameObject target;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxExtraSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxSpeedDistance;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject targetAim;
    [SerializeField] private GameObject trail;

    private GameObject sight;
    private float distance;
    public string username;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        cc.enabled = false;


        StartCoroutine(Activate());

        StartCoroutine(Beep());
        sight = Instantiate(targetAim);
        sight.GetComponent<Sight>().missile = gameObject;
    }

    private IEnumerator Activate()
    {
        if (isServer)
        {
            CmdFindFirstPlayer();
            RpcSetUsername(username);
        }
        yield return new WaitForSeconds(0.5f);
        cc.enabled = true;
        while (isServer)
        {
            CmdFindFirstPlayer();
            yield return new WaitForSeconds(3);
        }
    }

    private IEnumerator Beep()
    {
        while (true)
        {
            if (target == NetworkClient.localPlayer.gameObject)
            {
                AudioManager.instance.Play("MissileBeep");
            }
            yield return new WaitForSeconds(Mathf.Lerp(0.05f, 0.5f, Mathf.Sqrt(distance)));
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdFindFirstPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<string> users = new List<string>();

        for (int i = 1; i <= 8; i++)
        {
            foreach (GameObject p in players)
            {
                if (p.GetComponent<MultiplayerController>().placement == i && p.GetComponent<MultiplayerController>().currentState != PlayerStates.Finish)
                {
                    users.Add(p.GetComponent<MultiplayerController>().usernameText);
                }
            }
            if (users.Count > 0)
            {
                break;
            }
        }
        RpcFindFirstPlayer(users[Random.Range(0, users.Count)]);
    }

    [ClientRpc]
    private void RpcFindFirstPlayer(string targetUsername)
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<MultiplayerController>().usernameText == targetUsername)
            {
                target = p;
            }
        }
    }

    [ClientRpc]
    private void RpcSetUsername(string user)
    {
        username = user;
    }

    private void FixedUpdate()
    {
        GetComponent<AudioSource>().volume = DataManager.soundVolume;
        Vector2 vectorDirection = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized;
        float rotateAmount = Vector3.Cross(vectorDirection, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotationSpeed;

        distance = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).magnitude;
        distance = distance / maxSpeedDistance;
        if (distance > 1) { distance = 1; }
        float currentSpeed = minSpeed + Mathf.Lerp(0, maxExtraSpeed, distance);
        if (cc.enabled)
        {
            rb.velocity = transform.up * (minSpeed + Mathf.Lerp(0, maxExtraSpeed, distance));
        }
        else
        {
            rb.velocity = transform.up * (minSpeed + maxExtraSpeed);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdExplode(Vector3 pos)
    {
        GameObject ins = Instantiate(explosion, pos, Quaternion.identity);
        ins.GetComponent<Explosion>().username = username;
        NetworkServer.Spawn(ins);
        trail.transform.parent = null;
        trail.GetComponent<DestroyDelay>().DestroyAfterDelay();
        NetworkServer.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            CmdExplode(transform.position);
        }
    }
}
