using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallColor : MonoBehaviour
{
    [SerializeField] Material wallMaterial;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("LevelData") != null)
        {
            Color color = wallMaterial.GetColor("_Color");
            wallMaterial.SetColor("_Color", GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelData>().GetColor());
        }
    }
}
