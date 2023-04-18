using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoPitch : MonoBehaviour
{
    [SerializeField] MultiplayerController controller;
    AudioSource audio;
    float volume;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        volume = 1;
    }

    void FixedUpdate()
    {
        if (controller.currentState == PlayerStates.Jumping)
        {
            if (audio.pitch < 3f)
            {
                audio.pitch += 0.3f;
            }
            else if (audio.pitch > 3.5f)
            {
                audio.pitch -= 0.3f;
            }
        }
        else if (controller.currentState == PlayerStates.Grounded)
        {
            if (controller.boostTime > NetworkTime.time)
            {
                if (audio.pitch < 3.5f)
                {
                    audio.pitch += 0.3f;
                }
            }
            else
            {
                float pitch = Mathf.Lerp(0.1f, 3f, controller.lastSpeedMagnitude / 80);
                if (pitch > audio.pitch)
                {
                    audio.pitch += 0.1f;
                }
                else if (pitch < audio.pitch)
                {
                    audio.pitch -= 0.1f;
                }
            }
        }

        if (controller.currentState == PlayerStates.Dead)
        {
            volume -= 0.03f;

            if (audio.pitch > 0)
            {
                audio.pitch -= 0.03f;
            }
        }
        else if (volume < 1)
        {
            volume = 1;
        }

        audio.volume = volume * DataManager.soundVolume;
    }
}
