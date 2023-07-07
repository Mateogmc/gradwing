using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Transform camera;
    [SerializeField] float parallaxValue;
    [SerializeField] Material material;

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        transform.position = new Vector3(camera.position.x, camera.position.y, transform.position.z);
        material.SetVector("_Displacement", new Vector3(camera.position.x / parallaxValue, camera.position.y / parallaxValue, transform.position.z));
    }
}
