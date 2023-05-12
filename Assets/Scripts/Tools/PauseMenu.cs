using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;
    [SerializeField] Image musicImage;
    [SerializeField] Image soundImage;
    [SerializeField] Button exitButton;

    int selectedIndex = 0;

    bool pressedDown = false;
    bool pressedUp = false;
    bool pressedLeft = false;
    bool pressedRight = false;

    private void OnEnable()
    {
        selectedIndex = 0;
        musicSlider.value = DataManager.musicVolume;
        soundSlider.value = DataManager.soundVolume;
        AudioManager.instance.Play("ItemGet");
    }

    private void OnDisable()
    {
        DataManager.GetInstance().WriteStats();
    }

    private void Awake()
    {
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    void Update()
    {
        CheckInput();
        ColorManager();
    }

    private void ColorManager()
    {
        switch (selectedIndex)
        {
            case 0:
                musicImage.color = Color.green;
                soundImage.color = Color.white;
                exitButton.GetComponent<Image>().color = Color.white;
                break;

            case 1:
                musicImage.color = Color.white;
                soundImage.color = Color.green;
                exitButton.GetComponent<Image>().color = Color.white;
                break;

            case 2:
                musicImage.color = Color.white;
                soundImage.color = Color.white;
                exitButton.GetComponent<Image>().color = Color.green;
                break;
        }
    }

    private void CheckInput()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Vertical") < -0.3f) && !pressedDown)
        {
            pressedDown = true;
            if (selectedIndex == 2)
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
                selectedIndex = 2;
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

        if (selectedIndex == 0)
        {
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < -0.3f) && !pressedLeft)
            {
                pressedLeft = true;
                musicSlider.value -= 0.1f;
                if (musicSlider.value < 0.09)
                {
                    musicSlider.value = 0;
                }
                DataManager.musicVolume = musicSlider.value;
                MusicManager.instance.SetVolume(DataManager.musicVolume);
                AudioManager.instance.Play("ItemTing");
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") > -0.3f)
            {
                pressedLeft = false;
            }

            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0.3f) && !pressedRight)
            {
                pressedRight = true;
                musicSlider.value += 0.1f;
                if (musicSlider.value > 0.91)
                {
                    musicSlider.value = 1;
                }
                DataManager.musicVolume = musicSlider.value;
                MusicManager.instance.SetVolume(DataManager.musicVolume);
                AudioManager.instance.Play("ItemTing");
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetAxis("Horizontal") < 0.3f)
            {
                pressedRight = false;
            }
        }
        else if (selectedIndex == 1)
        {
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < -0.3f) && !pressedLeft)
            {
                pressedLeft = true;
                soundSlider.value -= 0.1f;
                if (soundSlider.value < 0.09)
                {
                    soundSlider.value = 0;
                }
                DataManager.soundVolume = soundSlider.value;
                AudioManager.instance.SetVolume(DataManager.soundVolume);
                AudioManager.instance.Play("ItemTing");
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") > -0.3f)
            {
                pressedLeft = false;
            }

            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0.3f) && !pressedRight)
            {
                pressedRight = true;
                soundSlider.value += 0.1f;
                if (soundSlider.value > 0.91)
                {
                    soundSlider.value = 1;
                }
                DataManager.soundVolume = soundSlider.value;
                AudioManager.instance.SetVolume(DataManager.soundVolume);
                AudioManager.instance.Play("ItemTing");
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetAxis("Horizontal") < 0.3f)
            {
                pressedRight = false;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) || (DataManager.GetInstance().xboxController ? Input.GetKeyDown(KeyCode.Joystick1Button0) : Input.GetKeyDown(KeyCode.Joystick1Button1))) && selectedIndex == 2)
        {
            Application.Quit();
        }
    }
}
