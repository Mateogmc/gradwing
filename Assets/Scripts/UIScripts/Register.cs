using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class Register : MonoBehaviour
{
    [SerializeField] Button loginButton;
    [SerializeField] TMP_InputField email;
    [SerializeField] TextMeshProUGUI emailError;
    [SerializeField] TMP_InputField username;
    [SerializeField] TextMeshProUGUI usernameError;
    [SerializeField] TMP_InputField password;
    [SerializeField] TMP_InputField confirm;
    [SerializeField] TextMeshProUGUI passwordError;
    [SerializeField] TextMeshProUGUI show;

    bool passwordCorrect = false;
    bool emailCorrect = false;

    private void OnEnable()
    {
        email.Select();
    }

    private void OnDisable()
    {
        loginButton.Select();
    }

    public void Show()
    {
        if (password.contentType == TMP_InputField.ContentType.Password)
        {
            password.contentType = TMP_InputField.ContentType.Standard;
            show.text = "Hide";
        }
        else
        {
            password.contentType = TMP_InputField.ContentType.Password;
            show.text = "Show";
        }
        password.Select();
    }

    public void Submit()
    {
        if (emailCorrect && passwordCorrect)
        {
            emailError.gameObject.SetActive(false);
            usernameError.gameObject.SetActive(false);
            passwordError.gameObject.SetActive(false);
            DataManager.username = username.text;
            StartCoroutine(ServerDataManager.Register(email.text, username.text, password.text, this));
        }
    }

    public void Back()
    {
        email.text = "";
        username.text = "";
        if (password.contentType == TMP_InputField.ContentType.Standard)
        {
            Show();
        }
        password.text = "";
        confirm.text = "";
        emailError.gameObject.SetActive(false);
        usernameError.gameObject.SetActive(false);
        passwordError.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ConfirmEdit(string confirm)
    {
        passwordError.gameObject.SetActive(confirm != password.text);
        passwordCorrect = confirm == password.text;
    }

    public void EmailFormat(string email)
    {
        bool match = Regex.Match(email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$").Success;

        if (match)
        {
            emailError.gameObject.SetActive(false);
        }
        else
        {
            emailError.text = "Incorrect email format";
            emailError.gameObject.SetActive(true);
        }
        emailCorrect = match;
    }

    public void ShowError(int error)
    {
        switch (error)
        {
            case 1:
                emailError.text = "Email already in use!";
                emailError.gameObject.SetActive(true);
                break;

            case 2:
                usernameError.text = "Username already in use!";
                usernameError.gameObject.SetActive(true);
                break;
        }
    }
}
