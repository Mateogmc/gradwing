using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoaderManager : MonoBehaviour
{
    [SerializeField] GameObject register;
    [SerializeField] GameObject login;

    [SerializeField] Button initialButton;

    private void Start()
    {
        initialButton.Select();
    }

    public void Register()
    {
        register.SetActive(true);
        login.SetActive(false);
    }

    public void Login()
    {
        login.SetActive(true);
        register.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
