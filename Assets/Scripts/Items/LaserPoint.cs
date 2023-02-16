using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LaserPoint : NetworkBehaviour
{
    private float startTime;

    private void Start()
    {
        startTime = Time.time + 0.05f;
    }

    private void Update()
    {
        if (startTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
