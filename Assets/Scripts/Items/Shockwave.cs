using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] GameObject shockwaveRadius;
    [SerializeField] GameObject shockwaveBlast;
    [SerializeField] public float maxDamage;
    [SerializeField] Material shockwaveMaterial;
    Material currentMaterial;

    float initialTime;
    float t;
    Color tmp;
    Color tmp2;

    public string username;

    public void InitializeShockwave(Collider2D parentPlayer, string parentUsername)
    {
        Physics2D.IgnoreCollision(shockwaveBlast.GetComponent<CircleCollider2D>(), parentPlayer, true);
        username = parentUsername;
    }

    private void Start()
    {
        GetComponent<AudioSource>().volume = DataManager.soundVolume;
        initialTime = Time.time + 0.2f;
        t = 0.2f;
        shockwaveBlast.transform.localScale = Vector3.zero;
        tmp = shockwaveBlast.GetComponent<SpriteRenderer>().color;
        tmp2 = shockwaveRadius.GetComponent<SpriteRenderer>().color;
        shockwaveBlast.GetComponent<CircleCollider2D>().radius = 1;
        currentMaterial = new Material(shockwaveMaterial);
        shockwaveBlast.GetComponent<SpriteRenderer>().material = currentMaterial;
        shockwaveRadius.GetComponent <SpriteRenderer>().material = currentMaterial;
    }

    private void Update()
    {
        t = initialTime - Time.time;
        if (t < -0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        shockwaveRadius.transform.Rotate(new Vector3(0, 0, 3));
        tmp2.a = Mathf.Lerp(0, 1, PowLerp(t * 5f));
        shockwaveRadius.GetComponent<SpriteRenderer>().color = tmp2;
        float scale = Mathf.Lerp(1, 0, PowLerp(t * 5f));
        shockwaveBlast.transform.localScale = new Vector3(scale, scale, scale);
        shockwaveBlast.GetComponent<CircleCollider2D>().radius = Mathf.Lerp(11f, 1, PowLerp(t * 5f));
        tmp.a = Mathf.Lerp(0, 1, SqrLerp(t * 5f));
        shockwaveBlast.GetComponent<SpriteRenderer>().color = tmp;
        currentMaterial.SetFloat("_StepValue", Mathf.Lerp(1, 0, SqrLerp(t * 5f)));
    }

    public float Damage()
    {
        return Mathf.Lerp(0.5f, 1, t * 5);
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
