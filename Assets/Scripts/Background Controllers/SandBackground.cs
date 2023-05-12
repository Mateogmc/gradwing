using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] Material material;
    [SerializeField] Material materialForeground;
    [SerializeField] float parallaxValue;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        AudioManager.instance.Play("Sandstorm");
    }

    void Update()
    {
        transform.position = new Vector3(camera.position.x, camera.position.y, 50);

        material.SetVector("_Displacement", new Vector4(camera.position.x / parallaxValue, (camera.position.y / parallaxValue) / 3, 0, 0));
        materialForeground.SetVector("_Displacement", new Vector4(camera.position.x / parallaxValue, (camera.position.y / parallaxValue) * 10, 0, 0));
    }
}
