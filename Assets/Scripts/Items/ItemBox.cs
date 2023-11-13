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
    Missile,
    Shockwave,
    Equalizer,
    Flash,
    Eliminator
}

public class ItemBox : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr1;
    [SerializeField] SpriteRenderer sr2;
    [SerializeField] SpriteRenderer sr3;
    [SerializeField] CircleCollider2D cc;
    [SerializeField] float respawnDelay;
    [SerializeField] Transform icon;
    [SerializeField] SpriteRenderer iconRenderer;
    [SerializeField] AudioSource itemTake;
    [SerializeField] AudioSource itemSpawn;
    [SerializeField] bool forcedItem = false;
    [SerializeField] Items containedItem = Items.None;
    float respawnTimer = 0f;

    private string[] items = { "Shield", "Jump", "Trap", "Rebounder", "Laser", "Boost", "Missile", "Shockwave", "Equalizer", "Flash"};

    //private float[] itemWeights = { 0.2f, 0.1f, 0.15f, 0.15f, 0.10f, 0.2f, 0.1f };

    private float[,] itemWeights = new float[,]
    {
        { 0.25f, 0f, 0.45f, 0.2f, 0f, 0.1f, 0f, 0.1f, 0f, 0f },
        { 0.2f, 0.05f, 0.3f, 0.25f, 0.1f, 0.1f, 0f, 0.1f, 0f, 0f },
        { 0.3f, 0.05f, 0.2f, 0.2f, 0.1f, 0.15f, 0.1f, 0.3f, 0f, 0f },
        { 0.25f, 0.1f, 0.1f, 0.2f, 0.1f, 0.15f, 0.2f, 0.3f, 0f, 0.05f },
        { 0.1f, 0.15f, 0.1f, 0.2f, 0.2f, 0.2f, 0.1f, 0.2f, 0f, 0.1f },
        { 0.1f, 0.20f, 0.1f, 0.15f, 0.2f, 0.2f, 0.1f, 0.2f, 0.05f, 0.2f },
        { 0f, 0.25f, 0f, 0f, 0.30f, 0.2f, 0.2f, 0f, 0.05f, 0.2f },
        { 0f, 0.30f, 0f, 0f, 0.30f, 0.2f, 0.1f, 0f, 0.1f,  0.2f }
    };

    private void Awake()
    {
        itemTake.volume = DataManager.soundVolume;
        itemSpawn.volume = DataManager.soundVolume;
    }

    private void Update()
    {
        if (respawnTimer < Time.time && !sr1.enabled)
        {
            itemSpawn.Play();
            sr1.enabled = true;
            sr2.enabled = true;
            sr3.enabled = true;
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

    private void GiveItem(MultiplayerController player, int placement, float value)
    {
        if (player != null)
        {
            if (forcedItem)
            {
                player.GetItem(containedItem.ToString(), true);
            }
            int playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
            if (playerCount <= 4 && placement != 1)
            {
                placement *= 2;
            }
            placement--;

            float maxWeight = 0;

            for (int i = 0; i < items.Length; i++)
            {
                maxWeight += itemWeights[placement, i];
            }

            float random = value * maxWeight;
            for (int i = 0; i < items.Length; i++)
            {
                random -= itemWeights[placement, i];
                if (random <= 0)
                {
                    player.GetItem(items[i], false);
                    break;
                }
            }
        }
    }

    public void Use(MultiplayerController player, int placement, float value)
    {
        GiveItem(player, placement, value);
        itemTake.Play();
        respawnTimer = Time.time + respawnDelay;
        sr1.enabled = false;
        sr2.enabled = false;
        sr3.enabled = false;
        cc.enabled = false;
        iconRenderer.enabled = false;
        CmdUse();
    }

    [Command(requiresAuthority = false)]
    private void CmdUse()
    {
        RpcUse();
    }

    [ClientRpc]
    private void RpcUse()
    {
        itemTake.Play();
        respawnTimer = Time.time + respawnDelay;
        sr1.enabled = false;
        sr2.enabled = false;
        sr3.enabled = false;
        cc.enabled = false;
        iconRenderer.enabled = false;
    }
}
