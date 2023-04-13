using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AxisScript : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.text = "LAxis: " + Input.GetAxis("StrafeL") + "/nRAxis: " + Input.GetAxis("StrafeL");
    }
}
