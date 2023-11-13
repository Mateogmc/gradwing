using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] bool destroyOnStart;

    private void Start()
    {
        if (destroyOnStart)
        {
            DestroyAfterDelay();
        }
    }

    public void DestroyAfterDelay()
    {
        StartCoroutine(Destroy(delay));
    }

    public void DestroyAfterDelay(float delay)
    {
        StartCoroutine(Destroy(delay));
    }

    private IEnumerator Destroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
