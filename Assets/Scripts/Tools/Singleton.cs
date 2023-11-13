using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public Singleton instance;
    public int id;

    private void Awake()
    {
        if (instance.id == id)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
}
