using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorHider : MonoBehaviour
{
    private Coroutine co_HideCursor;

    private void Start()
    {
        if (FindObjectsOfType<CursorHider>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    void Update()
    {
        if (Input.GetAxis("Mouse X") == 0 && (Input.GetAxis("Mouse Y") == 0) && !Input.GetKey(KeyCode.Mouse0) && SceneManager.GetActiveScene().name != "Loader" && SceneManager.GetActiveScene().name != "Login" && SceneManager.GetActiveScene().name != "Join Lobby")
        {
            if (co_HideCursor == null)
            {
                co_HideCursor = StartCoroutine(HideCursor());
            }
        }
        else
        {
            if (co_HideCursor != null)
            {
                StopCoroutine(co_HideCursor);
                co_HideCursor = null;
                Cursor.visible = true;
            }
        }
    }

    private IEnumerator HideCursor()
    {
        yield return new WaitForSeconds(3);
        Cursor.visible = false;
    }
}
