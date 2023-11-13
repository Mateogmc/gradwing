using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUICameraSet : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Canvas>().planeDistance = 10;
    }

    private void Update()
    {
        if (GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            GetComponent<Canvas>().sortingLayerID = SortingLayer.NameToID("UI");
        }
    }
}
