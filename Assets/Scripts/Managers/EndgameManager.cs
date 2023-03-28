using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class EndgameManager : MonoBehaviour
{
    public GameObject[] playerList = new GameObject[8];
    public Image[] placementList = new Image[8];
    public TextMeshProUGUI[] usernameList = new TextMeshProUGUI[8];
    public Dictionary<int, string> playerDictionary = new Dictionary<int, string>(8);
    public Button returnToLobby;
    bool returning = true;

    public void ReturnToLobby()
    {
        Debug.Log("Click");
        RaceManager.GetInstance().CmdReturnToLobby();
    }

    private void Update()
    {
        if (playerDictionary.Count >= GameObject.FindGameObjectsWithTag("Player").Length && returning)
        {
            returning = false;
            StartCoroutine(ReturnInSeconds());
        }
        Debug.Log(GameObject.FindGameObjectsWithTag("Player").Length);

        for (int i = 0; i < playerDictionary.Count; i++)
        {
            if (!playerList[i].activeSelf)
            {
                playerList[i].SetActive(true);
            }

            placementList[i].sprite = Resources.Load<Sprite>("UI/placement" + (i + 1));
            usernameList[i].text = playerDictionary[i + 1];
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

    public void AddDictionaryEntry(int placement, string username)
    {
        while (playerDictionary.ContainsKey(placement))
        {
            placement++;
        }
        playerDictionary[placement] = username;
    }

    private IEnumerator ReturnInSeconds()
    {
        yield return new WaitForSeconds(3);
        ReturnToLobby();
    }
}
