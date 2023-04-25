using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] Material material;
    [SerializeField] float parallaxValue;
    [SerializeField] Vector2 sunOffset;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        transform.position = new Vector3(camera.position.x, camera.position.y, 50);

        material.SetVector("_Displacement", new Vector4(camera.position.x / parallaxValue, camera.position.y / parallaxValue, 0, 0));
        material.SetVector("_SunDisplacement", new Vector4(camera.position.x / (parallaxValue * 100) + sunOffset.x, camera.position.y / (parallaxValue * 100) + sunOffset.y, 0, 0));
    }
}
