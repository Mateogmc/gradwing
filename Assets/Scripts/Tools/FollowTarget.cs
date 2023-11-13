using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    Transform target;
    public void Initialize(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        transform.position = target.position;
    }
}