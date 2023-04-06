using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColor : MonoBehaviour
{
    [SerializeField] Material wallMaterial;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("LevelData") != null)
        {
            Color color = wallMaterial.GetColor("_Color");
            Debug.Log(color.r + " " + color.g + " " + color.b);
            wallMaterial.SetColor("_Color", GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().wallColor);
        }
    }
}