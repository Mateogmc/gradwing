using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPoint : MonoBehaviour
{
    private float startTime;

    private void Start()
    {
        startTime = Time.time + 0.03f;
    }

    private void Update()
    {
        if (startTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
