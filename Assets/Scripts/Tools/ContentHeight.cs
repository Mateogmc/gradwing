using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentHeight : MonoBehaviour
{
    RectTransform rt;
    [SerializeField] float itemHeight;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SetHeight(itemHeight * transform.childCount);
    }

    public void SetHeight(float height)
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }
}
