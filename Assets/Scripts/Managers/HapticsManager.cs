using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager instance;
    private Gamepad pad;
    private float currentLowFrequency;
    private float currentHighFrequency;

    [HideInInspector] public bool gravel = false;
    [HideInInspector] public bool heal = false;
    [HideInInspector] public bool ice = false;
    [HideInInspector] public bool fire = false;

    private bool reservedVibration = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        currentLowFrequency = 0;
        currentHighFrequency = 0;
    }

    private void Update()
    {
        RumbleState();
    }

    private void RumbleState()
    {
        pad = Gamepad.current;
        if (pad != null && !reservedVibration && DataManager.GetInstance().rumble)
        {
            if (fire)
            {
                pad.SetMotorSpeeds(0.5f, 0.7f);
            }
            else if (gravel)
            {
                pad.SetMotorSpeeds(0.5f, 0.2f);
            }
            else if (ice)
            {
                pad.SetMotorSpeeds(0.1f, 0.3f);
            }
            else if (heal)
            {
                pad.SetMotorSpeeds(0.1f, 0.1f);
            }
            else
            {
                pad.SetMotorSpeeds(0, 0);
            }
        }
    }

    public void Rumble(float lowFrequency, float highFrequency, float time)
    {
        StopAllCoroutines();
        StartCoroutine(RumbleRoutine(lowFrequency, highFrequency, time));
    }

    public void RumbleLinear(float lowFrequency, float highFrequency, float time)
    {
        StopAllCoroutines();
        StartCoroutine(LinearRumbleRoutine(lowFrequency, highFrequency, time));
    }

    private IEnumerator RumbleRoutine(float lowFrequency, float highFrequency, float time)
    {
        pad = Gamepad.current;
        if (pad != null && DataManager.GetInstance().rumble)
        {
            reservedVibration = true;
            pad.SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSeconds(time);
            pad.SetMotorSpeeds(0, 0);
            reservedVibration = false;
        }
    }

    private IEnumerator LinearRumbleRoutine(float lowFrequency, float highFrequency, float time)
    {
        pad = Gamepad.current;
        if (pad != null && DataManager.GetInstance().rumble)
        {
            reservedVibration = true;
            for (float i = time; i > 0; i -= time / 10)
            {
                pad.SetMotorSpeeds(Mathf.Lerp(0, lowFrequency, i / time), Mathf.Lerp(0, highFrequency, i / time));
                yield return new WaitForSeconds(time / 10);
            }
            pad.SetMotorSpeeds(0, 0);
            reservedVibration = false;
        }
    }
}