using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bomb : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] CircleCollider2D cc;
    [SerializeField] private GameObject explosion;
    [SerializeField] float respawnDelay;
    float respawnTimer = 0f;

    private void Start()
    {
        explosion.GetComponent<Explosion>().maxDamage = 40;
    }

    void Update()
    {
        if (respawnTimer < Time.time && !sr.enabled)
        {
            sr.enabled = true;
            cc.enabled = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdExplode()
    {
        NetworkServer.Spawn(Instantiate(explosion, transform.position, Quaternion.identity));
        RpcExplode();
    }

    [ClientRpc]
    private void RpcExplode()
    {
        respawnTimer = Time.time + respawnDelay;
        sr.enabled = false;
        cc.enabled = false;
    }
}
