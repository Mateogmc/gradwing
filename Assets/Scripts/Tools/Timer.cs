using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] GameObject timer;
    [SerializeField] TextMeshProUGUI text;
    float initialTime;
    string currentTime;
    bool starting = false;

    void Update()
    {
        if (starting)
        {
            currentTime = string.Format("{0:D2}:{1:0#.###}", (int)Mathf.Floor((Time.time - initialTime)/60), Time.time - initialTime - (Mathf.Floor((Time.time - initialTime) / 60) * 60));
        }
        text.text = currentTime;
    }

    public void Initialize()
    {
        timer.SetActive(true);
        initialTime = Time.time;
        starting = true;
    }

    public void Stop()
    {
        starting = false;
    }

    public string GetTime()
    {
        return currentTime;
    }
}
