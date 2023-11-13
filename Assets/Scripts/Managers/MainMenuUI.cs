using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void DirectConnection()
    {
        SceneManager.LoadScene("Direct Connection");
    }

    public void JoinLobby()
    {
        SceneManager.LoadScene("Join Lobby");
    }
}
