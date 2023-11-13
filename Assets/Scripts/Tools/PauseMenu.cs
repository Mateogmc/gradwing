using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] TMP_Text musicText;
    [SerializeField] Slider soundSlider;
    [SerializeField] TMP_Text soundText;
    [SerializeField] Image musicImage;
    [SerializeField] Image soundImage;
    [SerializeField] Button rumbleButton;
    [SerializeField] Button strafeButton;
    [SerializeField] Button gamepadButton;
    [SerializeField] Button cameraButton;
    [SerializeField] Button resumeButton;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject options;

    int selectedIndex = 0;

    bool pressedDown = false;
    bool pressedUp = false;
    bool pressedLeft = false;
    bool pressedRight = false;

    public void Hide()
    {
        menu.SetActive(false);
        options.SetActive(false);
        DataManager.GetInstance().WriteStats();
        DataManager.GetInstance().paused = false;
        DataManager.GetInstance().pauseMenuOpen = false;
    }

    public void Show()
    {
        menu.SetActive(true);
        options.SetActive(false);
        selectedIndex = 0;
        musicSlider.value = DataManager.musicVolume;
        soundSlider.value = DataManager.soundVolume;
        resumeButton.Select();
        AudioManager.instance.Play("ItemGet");
        DataManager.GetInstance().paused = true;
        DataManager.GetInstance().pauseMenuOpen = true;
    }

    public void Options()
    {
        options.SetActive(true);
        menu.SetActive(false);
        musicSlider.Select();
    }

    public bool IsActive()
    {
        return menu.activeSelf || options.activeSelf;
    }

    void Update()
    {
        if (IsActive())
        {
            CheckInput();
        }
    }

    /*
    private void ColorManager()
    {
        switch (selectedIndex)
        {
            case 0:
                musicImage.color = Color.green;
                musicText.color = Color.white;
                soundImage.color = Color.white;
                soundText.color = new Color(0.5f, 0.5f, 0.5f);
                rumbleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                strafeButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                gamepadButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                cameraButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                exitButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case 1:
                musicImage.color = Color.white;
                musicText.color = new Color(0.5f, 0.5f, 0.5f);
                soundImage.color = Color.green;
                soundText.color = Color.white;
                rumbleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                strafeButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                gamepadButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                cameraButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                exitButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case 2:
                musicImage.color = Color.white;
                musicText.color = new Color(0.5f, 0.5f, 0.5f);
                soundImage.color = Color.white;
                soundText.color = new Color(0.5f, 0.5f, 0.5f);
                rumbleButton.GetComponent<Image>().color = Color.white;
                strafeButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                gamepadButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                cameraButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                exitButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case 3:
                musicImage.color = Color.white;
                musicText.color = new Color(0.5f, 0.5f, 0.5f);
                soundImage.color = Color.white;
                soundText.color = new Color(0.5f, 0.5f, 0.5f);
                rumbleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                strafeButton.GetComponent<Image>().color = Color.white;
                gamepadButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                cameraButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                exitButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case 4:
                musicImage.color = Color.white;
                musicText.color = new Color(0.5f, 0.5f, 0.5f);
                soundImage.color = Color.white;
                soundText.color = new Color(0.5f, 0.5f, 0.5f);
                rumbleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                strafeButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                gamepadButton.GetComponent<Image>().color = Color.white;
                cameraButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                exitButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case 5:
                musicImage.color = Color.white;
                musicText.color = new Color(0.5f, 0.5f, 0.5f);
                soundImage.color = Color.white;
                soundText.color = new Color(0.5f, 0.5f, 0.5f);
                rumbleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                strafeButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                gamepadButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                cameraButton.GetComponent<Image>().color = Color.white;
                exitButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                break;

            case 6:
                musicImage.color = Color.white;
                musicText.color = new Color(0.5f, 0.5f, 0.5f);
                soundImage.color = Color.white;
                soundText.color = new Color(0.5f, 0.5f, 0.5f);
                rumbleButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                strafeButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                gamepadButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                cameraButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                exitButton.GetComponent<Image>().color = Color.white;
                break;
        }
    }
    */
    public void MusicVolume(float value)
    {
        DataManager.musicVolume = value;
        MusicManager.instance.SetVolume(value);
    }

    public void SoundVolume(float value)
    {
        DataManager.soundVolume = value;
        AudioManager.instance.SetVolume(value);
    }

    public void Exit()
    {
        if (DataManager.GetInstance().loggedIn)
        {
            StartCoroutine(ServerDataManager.RemoveServers(DataManager.userID, true));
        }
        else
        {
            Application.Quit();
        }
    }

    public void Disconnect()
    {
        Hide();
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        DataManager.GetInstance().connectionDisplay.text = "Not connected";
        DataManager.GetInstance().spectating = false;
        StartCoroutine(ServerDataManager.RemoveServers(DataManager.userID));
    }

    private void CheckInput()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Vertical") < -0.3f) && !pressedDown)
        {
            pressedDown = true;
            if (selectedIndex == 6)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex++;
            }
            AudioManager.instance.Play("ItemTing");
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetAxis("Vertical") > -0.3f)
        {
            pressedDown = false;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0.3f) && !pressedUp)
        {
            pressedUp = true;
            if (selectedIndex == 0)
            {
                selectedIndex = 6;
            }
            else
            {
                selectedIndex--;
            }
            AudioManager.instance.Play("ItemTing");
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetAxis("Vertical") < 0.3f)
        {
            pressedUp = false;
        }

        if (InputManager.instance.controls.General.Accept.WasPressedThisFrame())
        {
            AudioManager.instance.Play("ItemGet");
        }
        if (InputManager.instance.controls.General.Cancel.WasPressedThisFrame())
        {
            AudioManager.instance.Play("ItemGet");
            if (options.activeSelf)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}
