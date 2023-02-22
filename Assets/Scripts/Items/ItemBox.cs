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
    [SerializeField] CircleCollider2D cc;
    [SerializeField] float respawnDelay;
    [SerializeField] Transform icon;
    [SerializeField] SpriteRenderer iconRenderer;
    float respawnTimer = 0f;

    private string[] items = { "Shield", "Jump", "Trap", "Rebounder", "Laser" };

    private float[] itemWeights = { 0.2f, 0.15f, 0.3f, 0.2f, 0.15f };

    private void Update()
    {
        if (respawnTimer < Time.time && !sr.enabled)
        {
            sr.enabled = true;
            cc.enabled = true;
            iconRenderer.enabled = true;
        }
        Rotate();
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(0, 0, Time.time * 100);
        icon.rotation = Quaternion.Euler(0, 0, Time.time * -60);
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
        cc.enabled = false;
        iconRenderer.enabled = false;
    }
}
