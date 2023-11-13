using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Login : MonoBehaviour
{
    [SerializeField] Button loginButton;
    [SerializeField] GameObject errorMessage;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;

    private void OnEnable()
    {
        email.Select();
    }

    private void OnDisable()
    {
        loginButton.Select();
    }

    public void LogIn()
    {
        errorMessage.SetActive(false);
        StartCoroutine(ServerDataManager.Login(email.text, password.text, this));
    }

    public void Error()
    {
        errorMessage.SetActive(true);
        StopCoroutine(HideErrorRoutine());
        StartCoroutine(HideErrorRoutine());
    }

    IEnumerator HideErrorRoutine()
    {
        yield return new WaitForSeconds(3);
        errorMessage.SetActive(false);
    }

    public void Back()
    {
        errorMessage.SetActive(false);
        email.text = "";
        password.text = "";
        gameObject.SetActive(false);
    }
}
