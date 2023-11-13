using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] Image panel;

    void Start()
    {
        StartCoroutine(SplashScreen());
    }

    private void Update()
    {
        if (InputManager.instance.controls.General.Submit.WasPressedThisFrame())
        {
            SceneManager.LoadScene("Login");
        }
    }

    private IEnumerator SplashScreen()
    {
        float t = Time.time + 1;

        while (t > Time.time)
        {
            panel.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, Mathf.Pow(t - Time.time, 2)));
            yield return new WaitForEndOfFrame();
        }

        panel.color = new Color(0, 0, 0, 0);

        yield return new WaitForSeconds(0.5f);

        t = Time.time + 1;

        while (t > Time.time)
        {
            panel.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, Mathf.Pow(t - Time.time, 2)));
            yield return new WaitForEndOfFrame();
        }

        panel.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Login");
    }
}
