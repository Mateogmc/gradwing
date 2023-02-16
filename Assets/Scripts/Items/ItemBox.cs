using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public enum Items
{
    None,
    Shield,
    Jump,
    Trap,
    Rebounder,
    Laser
}

public class ItemBox : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] float respawnDelay;
    float respawnTimer = 0f;

    private string[] items = { "Shield", "Jump", "Trap", "Rebounder", "Laser" };

    private float[] itemWeights = { 0.2f, 0.15f, 0.3f, 0.2f, 0.15f };

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
        if (player != null)
        {
            float random = Random.value;
            int i = 0;
            foreach (float item in itemWeights)
            {
                random -= item;
                if (random <= 0)
                {
                    player.CmdGetItem(items[i]);
                    break;
                }
                i++;
            }
        }
    }

    public void Use(MultiplayerController player)
    {
        GiveItem(player);
        respawnTimer = Time.time + respawnDelay;
        sr.enabled = false;
        bc.enabled = false;
    }
}
