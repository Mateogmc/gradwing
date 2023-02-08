using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBox : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] float respawnDelay;
    float respawnTimer = 0f;

    public enum Items
    {
        Shield,
        Jump,
        Trap,
        Rebounder,
        Laser
    }

    private void Update()
    {
        if (respawnTimer < Time.time && !sr.enabled)
        {
            sr.enabled = true;
            bc.enabled = true;
        }
    }

    private void GiveItem(MultiplayerController player)
    {
        if (player == null)
        {

        }
    }

    public void Use(MultiplayerController player)
    {
        Debug.Log("Item");
        GiveItem(player);
        respawnTimer = Time.time + respawnDelay;
        sr.enabled = false;
        bc.enabled = false;
    }
}
