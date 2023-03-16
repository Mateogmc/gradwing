using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour
{
    [SerializeField] private int position;
    [SerializeField] private BoxCollider2D bc;

    public int GetPosition()
    {
        bc.enabled = false;
        return position;
    }

    private void Start()
    {
        StartCoroutine(Remove());
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
