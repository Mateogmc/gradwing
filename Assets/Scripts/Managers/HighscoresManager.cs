using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class HighscoresManager : MonoBehaviour
{
    public static HighscoresManager instance;

    private void Start()
    {
        instance = this;
        GetScores();
    }

    private void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (InputManager.instance.controls.General.PageLeft.WasPressedThisFrame())
        {
            PreviousLevel();
        }
        else  if (InputManager.instance.controls.General.PageRight.WasPressedThisFrame())
        {
            NextLevel();
        }
    }

    struct Score
    {
        public string username;
        public string time;

        public Score(string username, string time)
        {
            this.username = username;
            this.time = time;
        }
    }
    List<Score> scoreList = new List<Score>();
    List<GameObject> gameObjects = new List<GameObject>();

    [SerializeField] GameObject playerListItem;
    int currentLevel = 0;
    [SerializeField] TextMeshProUGUI levelName;
    [SerializeField] GameObject content;

    private void GetScores()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.GetComponent<RecordDisplay>().Destroy();
        }

        gameObjects.Clear();
        scoreList.Clear();

        levelName.text = DataManager.levelList.ElementAt(currentLevel).Key;

        StartCoroutine(ServerDataManager.GetRecordList(DataManager.levelList.ElementAt(currentLevel).Key));
    }

    private void GetScores(int level)
    {
        gameObjects.Clear();
        scoreList.Clear();

        levelName.text = DataManager.levelList.ElementAt(currentLevel).Key;

        StartCoroutine(ServerDataManager.GetRecordList(DataManager.levelList.ElementAt(level).Key));
    }

    public void LoadScores(string scores)
    {
        string[] players = scores.Split('|');

        for (int i = 0; i < players.Length - 1; i++)
        {
            string[] player = players[i].Split('_');
            Score score = new Score(player[1], player[2]);
            scoreList.Add(score);
        }

        for (int i = 0; i < scoreList.Count; i++)
        {
            Score score = scoreList[i];
            GameObject ins = Instantiate(playerListItem);
            ins.transform.parent = content.transform;
            ins.GetComponent<RecordDisplay>().Display(i + 1, score.username, score.time);
            gameObjects.Add(ins);
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel >= DataManager.levelList.Count)
        {
            currentLevel = 0;
        }

        GetScores();
    }

    public void PreviousLevel()
    {
        currentLevel--;
        if (currentLevel < 0)
        {
            currentLevel = DataManager.levelList.Count - 1;
        }

        GetScores();
    }
}
