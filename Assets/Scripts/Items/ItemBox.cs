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
    Laser,
    Boost,
    Missile
}

public class ItemBox : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] CircleCollider2D cc;
    [SerializeField] float respawnDelay;
    [SerializeField] Transform icon;
    [SerializeField] SpriteRenderer iconRenderer;
    float respawnTimer = 0f;

    private string[] items = { "Shield", "Jump", "Trap", "Rebounder", "Laser", "Boost", "Missile"};

    //private float[] itemWeights = { 0.2f, 0.1f, 0.15f, 0.15f, 0.10f, 0.2f, 0.1f };

    private float[,] itemWeights = new float[,]
    {
        { 0.25f, 0f, 0.45f, 0.2f, 0f, 0.1f, 0f },
        { 0.2f, 0.05f, 0.3f, 0.25f, 0.1f, 0.1f, 0f },
        { 0.3f, 0.05f, 0.2f, 0.2f, 0.1f, 0.15f, 0f },
        { 0.3f, 0.1f, 0.1f, 0.2f, 0.1f, 0.15f, 0.05f },
        { 0.1f, 0.15f, 0.1f, 0.2f, 0.2f, 0.2f, 0.05f },
        { 0.1f, 0.20f, 0.1f, 0.15f, 0.2f, 0.2f, 0.05f },
        { 0f, 0.25f, 0f, 0f, 0.30f, 0.2f, 0.1f },
        { 0f, 0.30f, 0f, 0f, 0.30f, 0.2f, 0.05f }
    };

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

    private void GiveItem(MultiplayerController player, int placement)
    {
        if (player != null)
        {
            int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
            if (playerCount <= 4 && placement != 1)
            {
                placement *= 2;
            }
            placement--;

            float random = Random.value;
            for (int i = 0; i < items.Length; i++)
            {
                random -= itemWeights[placement, i];
                if (random <= 0)
                {
                    player.CmdGetItem(items[i]);
                    break;
                }
            }
        }
    }

    public void Use(MultiplayerController player, int placement)
    {
        GiveItem(player, placement);
        respawnTimer = Time.time + respawnDelay;
        sr.enabled = false;
        cc.enabled = false;
        iconRenderer.enabled = false;
    }
}
