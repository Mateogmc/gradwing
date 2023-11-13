using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    [SerializeField] GameObject timer;
    [SerializeField] TextMeshProUGUI text;
    float initialTime;
    string currentTime;
    public float recordTime;
    public string recordTimeFormated;
    bool starting = false;

    void Update()
    {
        if (starting)
        {
            currentTime = string.Format("{0:D2}:{1:0#.###}", (int)Mathf.Floor((Time.time - initialTime)/60), Time.time - initialTime - (Mathf.Floor((Time.time - initialTime) / 60) * 60));
        }
        text.text = currentTime;
    }

    public void SetInstance()
    {
        instance = this;
    }

    public void Initialize()
    {
        timer.SetActive(true);
        initialTime = Time.time;
        starting = true;
    }

    public void Stop(bool localPlayer)
    {
        starting = false;
        if (!localPlayer) { return; }
        if (Time.time < recordTime)
        {
            EndgameManager.instance.NewRecord(currentTime);
            if (DataManager.GetInstance().loggedIn)
            {
                StartCoroutine(ServerDataManager.PublishRecord(DataManager.userID, DataManager.levelName, currentTime));
            }
            else
            {
                StartCoroutine(DataManager.GetInstance().SaveRecord(currentTime));
            }
        }
        else
        {
            Debug.Log("Sent Time");
            EndgameManager.instance.OldRecord(recordTimeFormated);
        }
    }

    public string GetTime()
    {
        return currentTime;
    }

    public void SetRecordTime(string timeFormated, float time)
    {
        recordTime = initialTime + time;
        recordTimeFormated = timeFormated;

        Debug.Log(recordTime);
        Debug.Log(initialTime);
    }
}
