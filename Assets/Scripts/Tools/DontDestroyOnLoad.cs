using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        if (gameObject.tag == "GlobalVolume")
        {
            if (GameObject.FindGameObjectsWithTag("GlobalVolume").Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
