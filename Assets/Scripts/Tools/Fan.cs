using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] public float force;
    SpriteRenderer sr;
    [SerializeField] Sprite fan;
    [SerializeField] Sprite reverseFan;
    [SerializeField] Material fanMaterial;
    AudioSource audioSource;
    public bool stopped;
    [SerializeField] float stopTime;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.material = new Material(fanMaterial);
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1;
        SetForce();
        StartCoroutine(MakeSound());
    }

    public void Stop()
    {
        StartCoroutine(StopRoutine());
    }

    IEnumerator MakeSound()
    {
        yield return new WaitForSeconds(1);
        while(true)
        {
            audioSource.pitch = Mathf.Lerp(1, 1.5f, Mathf.Abs(force / 10));
            audioSource.Play();
            yield return new WaitForSeconds(stopped ? (1f/3f) : (1 / (3 + (Mathf.Lerp(0, 3, Mathf.Abs(force) / 8)))));
        }
    }

    IEnumerator StopRoutine()
    {
        stopped = true;
        sr.material.SetColor("_HighlightColor", new Vector4(10, 10, 0, 1));
        sr.material.SetFloat("_FanSpeed", 3 * Mathf.Sign(force));
        if (Mathf.Sign(force) > 0)
        {
            sr.sprite = fan;
        }
        else
        {
            sr.sprite = reverseFan;
        }

        bool switchColor = true;
        float t = Time.time + stopTime;
        while (t > Time.time)
        {
            if (switchColor)
            {
                sr.material.SetColor("_HighlightColor", Vector4.Lerp(sr.material.GetColor("_HighlightColor"), new Vector4(20, 5, 0, 1), 0.1f));
                if (sr.material.GetColor("_HighlightColor").g > 4.85f)
                {
                    switchColor = false;
                }
            }
            else
            {
                sr.material.SetColor("_HighlightColor", Vector4.Lerp(sr.material.GetColor("_HighlightColor"), new Vector4(20, 0, 0, 1), 0.1f));
                if (sr.material.GetColor("_HighlightColor").g < 0.05f)
                {
                    switchColor = true;
                }
            }
            yield return new WaitForFixedUpdate();
        }
        SetForce();
        stopped = false;
    }

    public void SetForce()
    {
        if (force < 0)
        {
            sr.sprite = reverseFan;
            sr.material.SetColor("_HighlightColor", new Vector4(20, 0, 0, 1));
            sr.material.SetFloat("_FanSpeed", -5 + force / 8);
        }
        else if (force == 0)
        {
            sr.sprite = fan;
            sr.material.SetColor("_HighlightColor", new Vector4(10, 0, 10, 1));
            sr.material.SetFloat("_FanSpeed", 5);
        }
        else if (force > 0)
        {
            sr.sprite = fan;
            sr.material.SetColor("_HighlightColor", new Vector4(0, 20, 0, 1));
            sr.material.SetFloat("_FanSpeed", 5 + force / 8);
        }
        audioSource.volume = 0.3f + Mathf.Lerp(0, 0.7f, Mathf.Pow(Mathf.Abs(force) / 20, 2));
        GetComponent<AudioDistance>().maxDistance = Mathf.Lerp(20, 60, audioSource.volume);
    }
}