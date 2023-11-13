using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LobbyStatus : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public TextMeshProUGUI playerCountText;

    [SerializeField] GameObject levelSelectPanel;

    Dictionary<string, Color> colorDict = new Dictionary<string, Color>{ { "E", new Color(0, 0.8f, 1f, 0.6f) }, { "M", new Color(0.7f, 0.9f, 0, 0.6f) }, { "H", new Color(1f, 0.2f, 0, 0.6f) }, { "C", new Color(0.7f, 0, 0.7f, 0.6f)} };

    [SerializeField] Image level1Panel;
    [SerializeField] Image level2Panel;
    [SerializeField] Image level3Panel;

    [SerializeField] Image level1Image;
    [SerializeField] Image level2Image;
    [SerializeField] Image level3Image;

    [SerializeField] TextMeshProUGUI level1Name;
    [SerializeField] TextMeshProUGUI level2Name;
    [SerializeField] TextMeshProUGUI level3Name;

    [SerializeField] TextMeshProUGUI level1Difficulty;
    [SerializeField] TextMeshProUGUI level2Difficulty;
    [SerializeField] TextMeshProUGUI level3Difficulty;

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

    public void LobbyReady(int playerCount, List<int> list, bool lobbyReady)
    {
        if (((playerCount >= 8 && false) || LobbyManager.GetInstance().lobbyReady) && lobbyReady)
        {
            StartCoroutine(StartCountDownRoutine());
            levelSelectPanel.SetActive(true);
            SetLevels(list);
        }
        else
        {
            StopAllCoroutines();
            levelSelectPanel.SetActive(false);
            UpdateText("Waiting for players...");
        }
    }

    private void SetLevels(List<int> list)
    {
        level1Name.text = DataManager.levelList.ElementAt(list[0]).Key;
        level2Name.text = DataManager.levelList.ElementAt(list[1]).Key;
        level3Name.text = DataManager.levelList.ElementAt(list[2]).Key;

        level1Image.sprite = Resources.Load<Sprite>("LevelThumbnails/" + level1Name.text);
        level2Image.sprite = Resources.Load<Sprite>("LevelThumbnails/" + level2Name.text);
        level3Image.sprite = Resources.Load<Sprite>("LevelThumbnails/" + level3Name.text);

        level1Panel.color = colorDict[DataManager.levelList.ElementAt(list[0]).Value];
        level2Panel.color = colorDict[DataManager.levelList.ElementAt(list[1]).Value];
        level3Panel.color = colorDict[DataManager.levelList.ElementAt(list[2]).Value];

        switch (DataManager.levelList.ElementAt(list[0]).Value)
        {
            case "E":
                level1Difficulty.text = "Easy";
                break;

            case "M":
                level1Difficulty.text = "Medium";
                break;

            case "H":
                level1Difficulty.text = "Hard";
                break;

            case "C":
                level1Difficulty.text = "Challenging";
                break;
        }

        switch (DataManager.levelList.ElementAt(list[1]).Value)
        {
            case "E":
                level2Difficulty.text = "Easy";
                break;

            case "M":
                level2Difficulty.text = "Medium";
                break;

            case "H":
                level2Difficulty.text = "Hard";
                break;

            case "C":
                level2Difficulty.text = "Challenging";
                break;
        }

        switch (DataManager.levelList.ElementAt(list[2]).Value)
        {
            case "E":
                level3Difficulty.text = "Easy";
                break;

            case "M":
                level3Difficulty.text = "Medium";
                break;

            case "H":
                level3Difficulty.text = "Hard";
                break;

            case "C":
                level3Difficulty.text = "Challenging";
                break;
        }
    }

    public void UpdateText(string text)
    {
        textMeshProUGUI.text = text;
        playerCountText.text = LobbyManager.GetInstance().playerCount + " players online";
    }
}
