using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBackground : MonoBehaviour
{
    Transform camera;
    [SerializeField] float parallaxValue;

    [SerializeField] Material cloudMaterial;
    [SerializeField] Material roadMaterial;

    [SerializeField] SpriteRenderer[] layers;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        float i = 0;
        foreach (SpriteRenderer layer in layers)
        {
            layer.material = new Material(cloudMaterial);
            layer.material.SetVector("_Offset", new Vector4(i, i, 0, 0));
            layer.material.SetFloat("_NoiseSpeed", 0); //(i + 0.1f)
            layer.material.SetFloat("_NoisePower", 1 + (i * 4));
            layer.material.SetColor("_Color", Color.gray * (1 -(4 * i / 5)));
            i += 0.25f;
        }
        StartCoroutine(LightningRoutine());
    }

    void Update()
    {
        transform.position = new Vector3(camera.position.x, camera.position.y, 0);

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].material.SetVector("_Displacement", new Vector4(camera.position.x / (parallaxValue * (5 - i)), camera.position.y / (parallaxValue * (5 - i)), 0, 0));
        }
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            AudioManager.instance.Play("Thunder" + Random.Range(1, 8));
            StartCoroutine(LightningCloud(layers[0]));
            Color temp = roadMaterial.GetColor("_Color1");
            roadMaterial.SetColor("_Color1", Color.white);
            yield return new WaitForSeconds(0.005f);
            roadMaterial.SetColor("_Color1", temp);
            yield return new WaitForSeconds(0.05f);
            roadMaterial.SetColor("_Color1", Color.white);
            yield return new WaitForSeconds(0.01f);
            roadMaterial.SetColor("_Color1", temp);
            StartCoroutine(LightningCloud(layers[1]));
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(LightningCloud(layers[2]));
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(LightningCloud(layers[3]));
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(LightningCloud(layers[4]));
            yield return new WaitForSeconds(0.4f);
        }
    }

    IEnumerator LightningCloud(SpriteRenderer layer)
    {
        Color temp = layer.material.GetColor("_Color");
        layer.material.SetColor("_Color", Color.white);
        float i = 1;
        while (i > 0)
        {
            yield return new WaitForEndOfFrame();
            layer.material.SetColor("_Color", Color.Lerp(temp, Color.white, i));
            i -= 0.01f;
        }
        layer.material.SetColor("_Color", temp);
    }
}
