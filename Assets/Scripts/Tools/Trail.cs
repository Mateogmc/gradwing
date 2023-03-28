using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] MultiplayerController controller;
    [SerializeField] Material material;
    Material currentMaterial;

    float currentSpeed;
    float red;
    float green;
    float blue;

    private void Awake()
    {
        currentMaterial = new Material(material);
        GetComponent<TrailRenderer>().material = currentMaterial;
    }

    private void Update()
    {
        currentSpeed = controller.lastSpeedMagnitude;

        if (controller.rollingSync)
        {
            red = 255;
            green = 0;
            blue = 255;
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else if (currentSpeed < 30)
        {
            red = 200;
            blue = 0;

            green = Mathf.Lerp(50, 150, currentSpeed / 30);
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else if (currentSpeed < 50)
        {
            green = 150;
            blue = 0;

            red = Mathf.Lerp(200, 0, (currentSpeed - 30) / 20);
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else if (currentSpeed < 70)
        {
            green = 150;
            red = 0;

            blue = Mathf.Lerp(0, 150, (currentSpeed - 50) / 20);
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else if (controller.maxSpeed < currentSpeed)
        {
            green = Mathf.Lerp(150, 200, (currentSpeed - controller.maxSpeed) / 40);
            blue = Mathf.Lerp(150, 250, (currentSpeed - controller.maxSpeed) / 40);

            red = Mathf.Lerp(0, 200, (currentSpeed - controller.maxSpeed) / 40);

            GetComponent<TrailRenderer>().widthMultiplier = Mathf.Lerp(0.6f, 2.5f, (currentSpeed - controller.maxSpeed) / 40);
        }

        currentMaterial.SetColor("_TrailColor", new Vector4(red, green, blue, 0.2f));

        if (controller.currentScale > 1)
        {
            currentMaterial.SetColor("_TrailColor", new Vector4(0, 0, 200, 0.2f));
            GetComponent<TrailRenderer>().time -= 0.3f * Time.deltaTime;
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else if (controller.currentScale < 1)
        {
            currentMaterial.SetColor("_TrailColor", new Vector4(200, 0, 0, 0.2f));
            GetComponent<TrailRenderer>().time -= 0.3f * Time.deltaTime;
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else
        {
            GetComponent<TrailRenderer>().time = 0.5f;
        }
    }
}
