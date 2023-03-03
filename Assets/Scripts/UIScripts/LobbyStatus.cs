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
        for (int i = 30; i >= 0; i--)
        {
            UpdateText(i.ToString());
            yield return new WaitForSeconds(1f);
        }
    }

    public void LobbyReady(bool ready, List<int> list)
    {
        if (ready)
        {
            UpdateText("Lobby Ready!");
            levelSelectPanel.SetActive(true);
            SetLevels(list);
        }
        else
        {
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
