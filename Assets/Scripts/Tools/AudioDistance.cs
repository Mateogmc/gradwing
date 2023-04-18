using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioDistance : MonoBehaviour
{
    private AudioSource[] sources;
    public float maxDistance;
    public float minDistance;

    void Awake()
    {
        sources = GetComponents<AudioSource>();
    }

    void Update()
    {
        float distance = (NetworkClient.localPlayer.gameObject.transform.position - transform.position).magnitude;

        foreach (AudioSource source in sources)
        {
            source.volume = Mathf.Lerp(DataManager.soundVolume, 0f, (distance + minDistance) / maxDistance);
        }
    }
}
