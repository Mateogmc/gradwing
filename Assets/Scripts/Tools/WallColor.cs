using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallColor : MonoBehaviour
{
    [SerializeField] Material wallMaterial;
    [SerializeField] Material bouncerMaterial;
    [SerializeField] Material neonArrow;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("LevelData") != null)
        {
            Color color = wallMaterial.GetColor("_Color");
            wallMaterial.SetColor("_Color", GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().GetColor());
            color = GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().GetColor() * 4;
            bouncerMaterial.SetColor("_Color2", new Color(color.g, color.b, color.r));
            neonArrow.SetColor("_NeonColor", new Color(color.g, color.b, color.r));
            color /= 4;
            bouncerMaterial.SetColor("_Color1", color);
            float rat  = color.r > color.g ? (color.r > color.b ? color.r : color.b) : (color.g > color.b ? color.g : color.b);
            color /= rat;
            neonArrow.SetColor("_BaseColor", color);
        }
    }
}
