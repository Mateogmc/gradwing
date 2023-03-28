using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SetElectricityPosition : MonoBehaviour
{
    public GameObject electricity;
    private float t;

    private void Start()
    {
        t = Time.time + 1;
    }

    void Update()
    {
        electricity.transform.localPosition = new Vector3(GetComponent<VisualEffect>().GetFloat("Position"), 0, 0);
        //electricity.transform.rotation = transform.rotation;

        electricity.GetComponent<VisualEffect>().SetInt("ParticleCount", (int)Mathf.Lerp(0, 500, Mathf.Sqrt(t - Time.time)));
    }
}
