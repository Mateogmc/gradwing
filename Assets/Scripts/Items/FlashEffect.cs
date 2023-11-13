using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] VisualEffect laser;

    public void Initialize(float length)
    {
        laser.SetFloat("Length", length * 10);
        Debug.Log(length * 10);
    }
}
