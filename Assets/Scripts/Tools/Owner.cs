using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Owner : MonoBehaviour
{
    public string username;

    public void Initialize(string username)
    {
        this.username = username;
    }
}
