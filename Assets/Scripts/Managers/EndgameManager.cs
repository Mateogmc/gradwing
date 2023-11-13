using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class EndgameManager : MonoBehaviour
{
    public static EndgameManager instance;

    public GameObject[] playerList = new GameObject[8];
    public Image[] placementList = new Image[8];
    public TextMeshProUGUI[] usernameList = new TextMeshProUGUI[8];
    public TextMeshProUGUI[] timerList = new TextMeshProUGUI[8];
    public Dictionary<int, GameObject> playerDictionary = new Dictionary<int, GameObject>(8);
    public Button returnToLobby;
    bool returning = true;

    [SerializeField] TextMeshProUGUI bestTime;
    [SerializeField] TextMeshProUGUI record;

    public void ReturnToLobby()
    {
        RaceManager.GetInstance().CmdReturnToLobby();
    }


    private void Update()
    {
        if (MultiplayerController.localPlayer.isServer && playerDictionary.Count >= GameObject.FindGameObjectsWithTag("Player").Length && returning)
        {
            returning = false;
            StartCoroutine(ReturnInSeconds());
        }

        for (int i = 0; i < playerDictionary.Count; i++)
        {
            if (!playerList[i].activeSelf)
            {
                playerList[i].SetActive(true);
            }

            placementList[i].sprite = Resources.Load<Sprite>("UI/placement" + (i + 1));
            usernameList[i].text = playerDictionary[i + 1].GetComponent<MultiplayerController>().usernameText;
            timerList[i].text = playerDictionary[i + 1].GetComponent<MultiplayerController>().timer;
        }
        if (GameStateManager.GetInstance().gameState == GameStateManager.GameState.Running && NetworkServer.active)
        {
            returnToLobby.gameObject.SetActive(true);
        }
        else
        {
            returnToLobby.gameObject.SetActive(false);
        }
    }

    public void SetInstance()
    {
        instance = this;
    }

    public void AddDictionaryEntry(int placement, GameObject player)
    {
        while (playerDictionary.ContainsKey(placement))
        {
            placement++;
        }
        playerDictionary[placement] = player;
    }

    public int GetPlacement(string username)
    {
        for (int i = 1; i <= playerDictionary.Count; i++)
        {
            if (playerDictionary[i].GetComponent<MultiplayerController>().usernameText == username)
            {
                return i;
            }
        }
        return 8;
    }

    public void NewRecord(string time)
    {
        bestTime.text = "NEW RECORD!";
        record.text = time;
        StartCoroutine(NewRecordRoutine());
    }

    public void OldRecord(string time)
    {
        record.text = time;
    }

    private IEnumerator NewRecordRoutine()
    {
        while (true)
        {
            bestTime.color = Color.Lerp(Color.white, Color.yellow, Mathf.Abs(Mathf.Sin(Time.time * 3)));
            record.color = Color.Lerp(Color.white, Color.yellow, Mathf.Abs(Mathf.Sin(Time.time * 3)));
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ReturnInSeconds()
    {
        if (MultiplayerController.localPlayer.isServer)
        {
            GameStateManager.GetInstance().CmdSetState(GameStateManager.GameState.GameOver);
        }
        yield return new WaitForSeconds(3);
        ReturnToLobby();
    }
}
