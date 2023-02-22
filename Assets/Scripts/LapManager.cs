using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public int lapCount;
    [SerializeField] float checkpointTolerance;
    int checkpointCount;
    public int minimumCheckpoints;
    GameObject[] checkpoints;

    private void Start()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpointCount = checkpoints.Length;
        minimumCheckpoints = (int) (checkpointCount * checkpointTolerance);
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
}
