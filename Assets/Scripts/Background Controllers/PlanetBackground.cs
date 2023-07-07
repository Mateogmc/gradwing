using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] float parallaxValue = 1;
    [SerializeField] Vector3 initialPos;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        transform.localPosition = new Vector3(camera.position.x / (parallaxValue * 2), camera.position.y / (parallaxValue * 4), transform.localPosition.z) + initialPos;
    }
}
