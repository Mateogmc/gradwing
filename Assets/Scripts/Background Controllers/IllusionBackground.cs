using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] Material material;
    [SerializeField] float parallaxValue;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }


    void Update()
    {
        transform.position = new Vector3(camera.position.x, camera.position.y, 10);

        material.SetVector("_Displacement", new Vector4(camera.position.x / parallaxValue, camera.position.y / parallaxValue, 0, 0));
    }
}
