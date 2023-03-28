using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayEffect : MonoBehaviour
{
    VisualEffect effect;
    void Start()
    {
        effect = GetComponent<VisualEffect>();
    }

    IEnumerator Play()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            effect.Play();
        }
    }
}
