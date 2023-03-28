using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [SerializeField] Material material;

    [SerializeField] float force;
    float value;
    private void FixedUpdate()
    {
        if (value < 0f)
        {
            value = 0.8f;
        }
        value -= 0.01f;

        material.SetFloat("_TimeVal", value);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Rebounder")
        {
            collision.transform.position += (transform.rotation * Vector2.right / 100) * force;
            Debug.Log(transform.rotation * Vector2.right);
        }
    }
}
