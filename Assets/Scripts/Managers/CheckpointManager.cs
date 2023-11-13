using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    void Start()
    {
        foreach (Checkpoint c in FindObjectsOfType<Checkpoint>())
        {
            try
            {
                c.checkpointNumber = int.Parse(c.gameObject.name.ToString().Split('_')[1]);
            } catch { }
        }
    }
}
