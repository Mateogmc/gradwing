using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Mirror;

public class Trail : MonoBehaviour
{
    [SerializeField] MultiplayerController controller;
    [SerializeField] Material material;
    [SerializeField] TrailRenderer smoke;
    [SerializeField] VisualEffect trailElectricity;
    Material currentMaterial;
    [SerializeField] Material speedometerMaterial;
    [SerializeField] Image speedometer;

    float currentSpeed;
    float red;
    float green;
    float blue;

    private void Awake()
    {
        currentMaterial = new Material(material);
        GetComponent<TrailRenderer>().material = currentMaterial;
        speedometer.material = new Material(speedometerMaterial);
    }

    private void Update()
    {
        currentSpeed = controller.lastSpeedMagnitude;
        float widthMultiplier = 0.6f;
        int particleCount = 0;

        if (currentSpeed < 30)
        {
            red = Mathf.Lerp(100, 0, currentSpeed / 30);
            blue = 0;

            green = Mathf.Lerp(0, 5, currentSpeed / 30);
            widthMultiplier = 0.6f;
            particleCount = 0;
        }
        else if (currentSpeed < 50)
        {
            green = Mathf.Lerp(5, 100, currentSpeed / 30);
            blue = 0;


            red = 0;
            widthMultiplier = 0.6f;
            particleCount = 0;
        }
        else if (currentSpeed < 70)
        {
            green = 100;
            red = 0;

            blue = Mathf.Lerp(0, 5, (currentSpeed - 50) / 20);
            widthMultiplier = 0.6f;
            particleCount = 0;
        }
        else if (controller.maxSpeed < currentSpeed)
        {
            green = Mathf.Lerp(100, 150, (currentSpeed - controller.maxSpeed) / 40);
            blue = Mathf.Lerp(5, 200, (currentSpeed - controller.maxSpeed) / 40);

            red = Mathf.Lerp(0, 150, (currentSpeed - controller.maxSpeed) / 40);

            widthMultiplier = Mathf.Lerp(0.6f, 1.5f, (currentSpeed - controller.maxSpeed) / 40);

            particleCount = (int)Mathf.Lerp(0, 300, (currentSpeed - 100) / 30);
            
        }

        if (controller.boostTime > NetworkTime.time)
        {
            red = Mathf.Lerp(red, 255, (controller.boostTime - (float)NetworkTime.time) / 1.5f);
            widthMultiplier = Mathf.Lerp(widthMultiplier, 3f,  (controller.boostTime - (float)NetworkTime.time) / 1.5f);
            particleCount = (int)Mathf.Lerp(particleCount, 300, (controller.boostTime - (float)NetworkTime.time) / 1.5f);
        }

        speedometer.material.SetFloat("_Value", Mathf.Lerp(speedometer.material.GetFloat("_Value"), currentSpeed / 120, (Mathf.Abs(speedometer.material.GetFloat("_Value") - (currentSpeed / 120)) * 5 + 5) * Time.deltaTime));
        speedometer.material.SetFloat("_SegmentRotationSpeed", -1 - Mathf.Floor(currentSpeed / controller.maxSpeed));
        speedometer.material.SetColor("_Color", new Vector4(red / 3, green / 3, blue / 3, 0.2f));

        if (controller.rollingSync)
        {
            widthMultiplier = 0.6f;
            particleCount = 0;
            currentMaterial.SetColor("_TrailColor", new Vector4(255, 0, 255, 0.2f));
        }
        else if (controller.maxSpeed < currentSpeed)
        {
            green = Mathf.Lerp(100, 150, (currentSpeed - controller.maxSpeed) / 40);
            blue = Mathf.Lerp(100, 200, (currentSpeed - controller.maxSpeed) / 40);

            red = Mathf.Lerp(0, 150, (currentSpeed - controller.maxSpeed) / 40);

            widthMultiplier = Mathf.Lerp(0.6f, 1.5f, (currentSpeed - controller.maxSpeed) / 40);

            particleCount = (int)Mathf.Lerp(0, 300, (currentSpeed - 100) / 30);

            currentMaterial.SetColor("_TrailColor", new Vector4(red, green, blue, 0.2f));

        }
        else
        {
            currentMaterial.SetColor("_TrailColor", new Vector4(red, green, blue, 0.2f));
        }

        GetComponent<TrailRenderer>().widthMultiplier = widthMultiplier;
        trailElectricity.SetInt("ParticleCount", particleCount);

        if (controller.currentAirborne > 1)
        {
            currentMaterial.SetColor("_TrailColor", new Vector4(0, 0, 200, 0.2f));
            GetComponent<TrailRenderer>().time -= 0.3f * Time.deltaTime;
            GetComponent<TrailRenderer>().widthMultiplier = 0.6f;
        }
        else if (controller.currentAirborne< 1)
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
