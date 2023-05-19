using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windshield : MonoBehaviour
{
    [SerializeField] MultiplayerController controller;
    [SerializeField] Material vehicleMaterial;
    [SerializeField] SpriteRenderer vehicleRenderer;
    Material currentMaterial;
    public Color color = new Color(0, 0, 0);
    Color colorAlt = new Color(0, 0, 0);

    private void Start()
    {
        currentMaterial = new Material(vehicleMaterial);
        vehicleRenderer.material = currentMaterial;
        Color wallColor = GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().GetColor() / 60;
        color.r = wallColor.r;
        color.g = wallColor.g;
        color.b = wallColor.b;
        wallColor *= 80;
        colorAlt.r = wallColor.g;
        colorAlt.g = wallColor.b;
        colorAlt.b = wallColor.r;
        currentMaterial.SetColor("_WindowColor", color);
    }

    private void Update()
    {
        currentMaterial.SetFloat("_ReflectionSpeed", controller.lastSpeedMagnitude / 80);
        currentMaterial.SetColor("_WindowColor", color);
        currentMaterial.SetColor("_LineColor", colorAlt);
    }

    public void SetColor()
    {
        currentMaterial.SetColor("_BodyColor", controller.vehicleColor);
    }
}
