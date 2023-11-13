using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CommandLine : MonoBehaviour
{
    [SerializeField] GameObject commandLine;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI chatBox;

    private void Awake()
    {
        inputField.onSubmit.AddListener(Message);
    }
    public bool IsActive()
    {
        return commandLine.activeSelf;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            commandLine.SetActive(!commandLine.activeSelf);
            inputField.Select();
            if (commandLine.activeSelf)
            {
                inputField.ActivateInputField();
            }
            else
            {
                inputField.DeactivateInputField();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            commandLine.SetActive(false);
            inputField.DeactivateInputField();
        }
    }

    public void Message(string command)
    {
        if (command.StartsWith('/'))
        {
            Command(command.Substring(1));
        }
        else
        {
            chatBox.text += command + "\n";
        }
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void Command(string command)
    {
        switch (command.Split(' ')[0].ToLower())
        {
            case "help":
                chatBox.text += "<color=green>GiveItem, SetStage</color>\n";
                break;

            case "giveitem":
                NetworkClient.localPlayer.GetComponent<MultiplayerController>().CmdGetItem(command.Substring(9));
                commandLine.SetActive(false);
                inputField.DeactivateInputField();
                break;

            case "setstage":
                LobbyManager.GetInstance().CmdStartGame(command.Substring(9));
                commandLine.SetActive(false);
                inputField.DeactivateInputField();
                break;

            default:
                chatBox.text += "<color=red>Unknown command. Type /Help for a list of commands.</color>\n";
                break;
        }
    }
}
