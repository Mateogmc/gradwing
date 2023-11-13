using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager instance;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] TextMeshProUGUI languageText;
    [SerializeField] Transform background;
    float backgroundBaseScale = 150;
    [HideInInspector] public bool langEn = false;
    [HideInInspector] public bool ready = false;
    bool zoomReady = true;
    GameObject currentTrigger = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (InputManager.instance.controls.Buttons.SetLang.WasPressedThisFrame())
        {
            ready = false;
            ChangeLanguage();
        }
    }

    public void ChangeLanguage()
    {
        langEn = !langEn;
        if (langEn)
        {
            languageText.text = "English. Pulsa Select o \"i\" para cambiar de idioma. ";
        }
        else
        {
            languageText.text = "Español. Press Select or \"i\" to change language. ";
        }
        ready = true;
    }

    public void TriggerText(GameObject trigger, string text)
    {
        tutorialText.enabled = true;
        tutorialText.text = text;
    }

    private IEnumerator ResizeRoutine(float oldSize, float newSize, float time)
    {
        while (!zoomReady)
        {
            yield return null;
        }
        zoomReady = false;
        float elapsed = 0;
        while (elapsed <= time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            background.localScale = new Vector3(Mathf.Lerp(oldSize, newSize, t), Mathf.Lerp(oldSize, newSize, t), Mathf.Lerp(oldSize, newSize, t));
            yield return null;
        }
        zoomReady = true;
    }

    public void TriggerZone(GameObject trigger, string text, Vector2 position, float zoom)
    {
        try
        {
            TriggerText(trigger, text);
        }
        catch
        { Debug.LogWarning("No text"); }
        StartCoroutine(ResizeRoutine(background.localScale.x, backgroundBaseScale * ((zoom * 100 / 30) / 100), 0.5f));
        CameraMovement.instance.EnableCinematic(new Vector3(position.x, position.y, -10), zoom);
        currentTrigger = trigger;
    }

    public void CloseZone(GameObject trigger)
    {
        if (currentTrigger != trigger) { return; }
        try
        {
            tutorialText.enabled = false;
        }
        catch
        { Debug.LogWarning("No text"); }
        StartCoroutine(ResizeRoutine(background.localScale.x, backgroundBaseScale, 0.5f));
        CameraMovement.instance.DisableCinematic();
    }
}
