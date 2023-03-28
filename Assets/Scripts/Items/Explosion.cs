using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] GameObject explosionRadius;
    [SerializeField] GameObject explosionBlast;
    [SerializeField] public float maxDamage;

    float initialTime;
    float t;
    Color tmp;

    private void Start()
    {
        initialTime = Time.time + 1;
        t = 1;
        explosionBlast.transform.localScale = Vector3.zero;
        tmp = explosionBlast.GetComponent<SpriteRenderer>().color;
        explosionBlast.GetComponent<CircleCollider2D>().radius = 1;
    }

    private void Update()
    {
        t = initialTime - Time.time;
        if (t < 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        explosionRadius.transform.Rotate(new Vector3(0, 0, 3));
        float scale = Mathf.Lerp(1, 0, PowLerp(t));
        explosionBlast.transform.localScale = new Vector3(scale, scale, scale);
        explosionBlast.GetComponent<CircleCollider2D>().radius = Mathf.Lerp(11f, 1, PowLerp(t));
        tmp.a = Mathf.Lerp(0, 1, SqrLerp(t));
        explosionBlast.GetComponent<SpriteRenderer>().color = tmp;
    }

    public float Damage()
    {
        return t;
    }

    private float PowLerp(float t)
    {
        return Mathf.Pow(t, 3);
    }

    private float SqrLerp(float t)
    {
        return Mathf.Sqrt(t);
    }
}
