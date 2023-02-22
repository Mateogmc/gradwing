using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.current;
    }
}
