using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] Material blackHoleMaterial;
    [SerializeField] float parallaxValue;
    [SerializeField] GameObject core;
    [SerializeField] Vector2 offset;

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        blackHoleMaterial.SetVector("_Position", new Vector4(camera.position.x / (parallaxValue * 2) + offset.x, camera.position.y / (parallaxValue * 4) + offset.y, 0, 0));

        core.transform.localPosition = new Vector3(blackHoleMaterial.GetVector("_Position").x - 0.5f, blackHoleMaterial.GetVector("_Position").y - 0.5f, core.transform.localPosition.z);
    }
}
