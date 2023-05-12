using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Trail : MonoBehaviour
{
    [SerializeField] MultiplayerController controller;
    [SerializeField] Material material;
    [SerializeField] TrailRenderer smoke;
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
        float widthMultiplier = 0.6f;

        if (controller.rollingSync)
        {
            red = 255;
            green = 0;
            blue = 255;
            widthMultiplier = 0.6f;
        }
        else if (currentSpeed < 30)
        {
            red = 200;
            blue = 0;

            green = Mathf.Lerp(50, 100, currentSpeed / 30);
            widthMultiplier = 0.6f;
        }
        else if (currentSpeed < 50)
        {
            green = 100;
            blue = 0;


            red = Mathf.Lerp(200, 0, (currentSpeed - 30) / 20);
            widthMultiplier = 0.6f;
        }
        else if (currentSpeed < 70)
        {
            green = 100;
            red = 0;

            blue = Mathf.Lerp(0, 100, (currentSpeed - 50) / 20);
            widthMultiplier = 0.6f;
        }
        else if (controller.maxSpeed < currentSpeed)
        {
            green = Mathf.Lerp(100, 150, (currentSpeed - controller.maxSpeed) / 40);
            blue = Mathf.Lerp(100, 200, (currentSpeed - controller.maxSpeed) / 40);

            red = Mathf.Lerp(0, 150, (currentSpeed - controller.maxSpeed) / 40);

            widthMultiplier = Mathf.Lerp(0.6f, 1.5f, (currentSpeed - controller.maxSpeed) / 40);
        }

        if (controller.boostTime > NetworkTime.time)
        {
            red = Mathf.Lerp(red, 255, (controller.boostTime - (float)NetworkTime.time) / 1.5f);
            widthMultiplier = Mathf.Lerp(widthMultiplier, 3f,  (controller.boostTime - (float)NetworkTime.time) / 1.5f);
        }

        currentMaterial.SetColor("_TrailColor", new Vector4(red, green, blue, 0.2f));
        GetComponent<TrailRenderer>().widthMultiplier = widthMultiplier;

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

        if (controller.currentState == PlayerStates.Dead)
        {
            GetComponent<TrailRenderer>().sortingLayerName = "Background";
            smoke.sortingLayerName = "Background";
        }
        else
        {
            GetComponent<TrailRenderer>().sortingLayerName = "Players";
            smoke.sortingLayerName = "Players";
        }
    }
}
