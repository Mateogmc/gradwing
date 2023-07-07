using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] Material material;
    [SerializeField] Material roadMaterial;
    [SerializeField] float parallaxValue;
    [SerializeField] float roadParallaxValue;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }


    void Update()
    {
        transform.position = new Vector3(camera.position.x, camera.position.y, 10);

        //material.SetVector("_Displacement", new Vector4(camera.position.x / parallaxValue, camera.position.y / parallaxValue, 0, 0));
        roadMaterial.SetVector("_Displacement", new Vector4(-camera.position.x / roadParallaxValue, -camera.position.y / roadParallaxValue, 0, 0));
        roadMaterial.SetVector("_ColorDisplacement", new Vector4(-camera.position.x / roadParallaxValue, -camera.position.y / roadParallaxValue, 0, 0));
    }
}
