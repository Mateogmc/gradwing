using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementShaderScript : MonoBehaviour
{
    [SerializeField] Material material;
    Material imageMaterial;
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
        image.material = new Material(material);
        imageMaterial = image.material;
    }

    private void Update()
    {
        int placement = int.Parse(image.sprite.name[image.sprite.name.Length - 1].ToString());

        float rMain = 0, gMain = 0, bMain = 0, rGlow = 0, gGlow = 0, bGlow = 0;

        switch (placement)
        {
            case 1:
                rMain = 1;
                gMain = 1;
                bMain = 1;
                rGlow = 1900;
                gGlow = 1900;
                bGlow = 1900;
                break;

            case 2:
                rMain = 0.52f;
                gMain = 0.95f;
                bMain = 1;
                rGlow = 0;
                gGlow = 1400;
                bGlow = 1100;
                break;

            case 3:
                rMain = 0.33f;
                gMain = 1;
                bMain = 0.46f;
                rGlow = 0;
                gGlow = 2000;
                bGlow = 256;
                break;

            case 4:
                rMain = 0.8f;
                gMain = 1;
                bMain = 0.26f;
                rGlow = 1310;
                gGlow = 940;
                bGlow = 230;
                break;

            case 5:
                rMain = 0.62f;
                gMain = 0.38f;
                bMain = 0;
                rGlow = 1310;
                gGlow = 340;
                bGlow = 230;
                break;

            case 6:
                rMain = 0.69f;
                gMain = 0.17f;
                bMain = 0.22f;
                rGlow = 1310;
                gGlow = 230;
                bGlow = 980;
                break;

            case 7:
                rMain = 0.69f;
                gMain = 0.17f;
                bMain = 0.62f;
                rGlow = 120;
                gGlow = 140;
                bGlow = 630;
                break;

            case 8:
                rMain = 0.33f;
                gMain = 0.33f;
                bMain = 0.33f;
                rGlow = 400;
                gGlow = 70;
                bGlow = 1000;
                break;
        }

        imageMaterial.SetColor("_MainColor", new Vector4(rMain, gMain, bMain));
        imageMaterial.SetColor("_GlowColor", new Vector4(rGlow, gGlow, bGlow));
    }
}
