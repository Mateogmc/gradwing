using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class LobbyStatus : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] GameObject levelSelectPanel;

    [SerializeField] Image level1Image;
    [SerializeField] Image level2Image;
    [SerializeField] Image level3Image;

    [SerializeField] TextMeshProUGUI level1Name;
    [SerializeField] TextMeshProUGUI level2Name;
    [SerializeField] TextMeshProUGUI level3Name;

    public void StartCountDown()
    {
        StartCoroutine(StartCountDownRoutine());
    }

    IEnumerator StartCountDownRoutine()
    {
        LobbyManager.GetInstance().CmdSetCountdown(30);
        for (int i = LobbyManager.GetInstance().countdownValue; i >= 0; i--)
        {
            UpdateText(i.ToString());
            LobbyManager.GetInstance().CmdSetCountdown(i);
            yield return new WaitForSeconds(1f);
            if (LobbyManager.GetInstance().gameReady)
            {
                break;
            }
        }
        yield return new WaitForSeconds(1f);
        for (int i = 3; i >= 0; i--)
        {
            UpdateText("Starting in " + i.ToString() + "...");
            yield return new WaitForSeconds(1);
        }
        LobbyManager.GetInstance().CmdStartGame();
    }

    public void LobbyReady(bool ready, List<int> list)
    {
        if (ready)
        {
            StartCoroutine(StartCountDownRoutine());
            levelSelectPanel.SetActive(true);
            SetLevels(list);
        }
        else
        {
            StopAllCoroutines();
            levelSelectPanel.SetActive(false);
            UpdateText("Waiting for players");
        }
    }

    private void SetLevels(List<int> list)
    {
        level1Name.text = DataManager.levelList[list[0]];
        level2Name.text = DataManager.levelList[list[1]];
        level3Name.text = DataManager.levelList[list[2]];

        level1Image.sprite = Resources.Load<Sprite>("LevelThumbnails/" + level1Name.text);
        level2Image.sprite = Resources.Load<Sprite>("LevelThumbnails/" + level2Name.text);
        level3Image.sprite = Resources.Load<Sprite>("LevelThumbnails/" + level3Name.text);
    }

    public void UpdateText(string text)
    {
        textMeshProUGUI.text = text;
    }
}
