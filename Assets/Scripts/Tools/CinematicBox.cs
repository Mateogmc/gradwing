using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CinematicBox : MonoBehaviour
{
    [SerializeField] bool cinematic = false;
    [SerializeField] string textEnglish = "";
    [SerializeField] string textSpanish = "";
    [SerializeField] float zoom = 30;
    bool onZone = false;

    private void LateUpdate()
    {
        if (InputManager.instance.controls.Buttons.SetLang.WasPressedThisFrame() && onZone)
        {
            TriggerText();
        }
    }

    private void TriggerText()
    {
        if (cinematic)
        {
            CinematicManager.instance.TriggerZone(gameObject, CinematicManager.instance.langEn ? textEnglish : textSpanish, transform.position, zoom);
        }
        else
        {
            CinematicManager.instance.TriggerText(gameObject, CinematicManager.instance.langEn ? textEnglish : textSpanish);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NetworkClient.localPlayer.gameObject)
        {
            TriggerText();
            onZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == NetworkClient.localPlayer.gameObject)
        {
            CinematicManager.instance.CloseZone(gameObject);
        }
        onZone = false;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(zoom * 3.58f, zoom * 2, zoom));
#endif
    }
}
