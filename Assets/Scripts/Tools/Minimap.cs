using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class Minimap : MonoBehaviour
{
    [SerializeField] Image floorImage;
    [SerializeField] RectTransform minimapRef1;
    [SerializeField] RectTransform minimapRef2;
    [SerializeField] RectTransform player1;
    [SerializeField] RectTransform player2;
    [SerializeField] RectTransform player3;
    [SerializeField] RectTransform player4;
    [SerializeField] RectTransform player5;
    [SerializeField] RectTransform player6;
    [SerializeField] RectTransform player7;
    [SerializeField] RectTransform player8;
    Transform worldRef1;
    Transform worldRef2;

    private float minimapRatio;
    private GameObject[] players;
    private RectTransform[] playerIcons = new RectTransform[8];

    private void Start()
    {
        playerIcons[0] = player1;
        playerIcons[1] = player2;
        playerIcons[2] = player3;
        playerIcons[3] = player4;
        playerIcons[4] = player5;
        playerIcons[5] = player6;
        playerIcons[6] = player7;
        playerIcons[7] = player8;

        SetMinimap(SceneManager.GetActiveScene().name);
        CalculateMapRatio();
        StartCoroutine(CheckPlayerList());
    }

    private void Update()
    {
        MovePlayers();
    }

    private void MovePlayers()
    {
        int i = 0;
        if (players != null)
        {
            foreach (GameObject player in players)
            {
                if (!playerIcons[i].gameObject.activeSelf)
                {
                    playerIcons[i].gameObject.SetActive(true);
                }
                if (player == NetworkClient.localPlayer.gameObject)
                {
                    playerIcons[i].GetComponent<Image>().color = Color.green;
                }
                else
                {
                    playerIcons[i].GetComponent<Image>().color = Color.red;
                }
                playerIcons[i].position = new Vector2(minimapRef1.position.x, minimapRef1.position.y) + new Vector2((player.transform.position.x - worldRef1.position.x) * minimapRatio, (player.transform.position.y - worldRef1.position.y) * minimapRatio);
                i++;
            }
            for (; i < 8; i++)
            {
                if (playerIcons[i].gameObject.activeSelf)
                {
                    playerIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetMinimap(string sceneName)
    {
        floorImage.sprite = Resources.Load<Sprite>("LevelThumbnails/mini_" + sceneName);
        if (floorImage.sprite == null)
        {
            floorImage.color = Vector4.zero;
        }
    }

    public void CalculateMapRatio()
    {
        GameObject[] refPoints = GameObject.FindGameObjectsWithTag("Reference");
        if (refPoints[0].name == "WorldRef1")
        {
            worldRef1 = refPoints[0].transform;
            worldRef2 = refPoints[1].transform;
        }
        else
        {
            worldRef1 = refPoints[1].transform;
            worldRef2 = refPoints[0].transform;
        }

        float distanceWorld = (worldRef1.position - worldRef2.position).magnitude;
        float distanceMinimap = (minimapRef1.position - minimapRef2.position).magnitude;

        minimapRatio = distanceMinimap / distanceWorld;
    }

    private IEnumerator CheckPlayerList()
    {
        while (true)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            yield return new WaitForSeconds(1);
        }
    }
}
