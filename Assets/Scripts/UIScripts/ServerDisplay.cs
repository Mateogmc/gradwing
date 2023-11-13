using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServerDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI ipAddress;
    [SerializeField] TextMeshProUGUI playerCount;

    RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        
    }

    public void OnClick()
    {
        DataManager.ipAddress = ipAddress.text;
        FindObjectOfType<JoinLobbyUI>().ConnectToIP();
        FindObjectOfType<ServerListManager>().Hide();
    }

    public void Display(int id, string name, string uri, int players)
    {
        this.name.text = name;
        this.ipAddress.text = uri;
        this.playerCount.text = $"{players} Players";

        rt.localPosition = new Vector2(rt.localPosition.x, -40 - (80 * id));
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
