using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapManager : MonoBehaviour
{
    public int lapCount;
    [SerializeField] float checkpointTolerance;
    int checkpointCount;
    public int minimumCheckpoints;
    GameObject[] checkpoints;
    public Button exitToLobby;

    private void Start()
    {
        GameStateManager.GetInstance().gameState = GameStateManager.GameState.Running;
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpointCount = checkpoints.Length;
        minimumCheckpoints = (int) (checkpointCount * checkpointTolerance);
        exitToLobby.onClick.AddListener(() => ExitToLobby());
    }

    public bool CheckNextLap(int checkpoints)
    {
        EnableCheckpoints();
        if (checkpoints >= minimumCheckpoints)
        {
            return true;
        }
        return false;
    }

    private void EnableCheckpoints()
    {
        foreach(GameObject checkpoint in checkpoints)
        {
            checkpoint.SetActive(true);
        }
    }

    public void DisableCheckpoints(int currentCheckpoint)
    {
        foreach(GameObject checkpoint in checkpoints)
        {
            if (checkpoint.GetComponent<CheckpointValue>().checkpointNumber <= currentCheckpoint)
            {
                checkpoint.SetActive(false);
            }
        }
    }

    private void ExitToLobby()
    {

    }
}
