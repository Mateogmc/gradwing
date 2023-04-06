using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Sight : NetworkBehaviour
{
    GameObject target;
    public GameObject missile;

    private void Start()
    {
        target = FindFirstPlayer();
        StartCoroutine(Activate());
    }

    private void Update()
    {
        if (missile == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = target.transform.position;
            transform.Rotate(new Vector3(0, 0, 1));
        }
    }

    private IEnumerator Activate()
    {
        while (true)
        {
            target = FindFirstPlayer();
            yield return new WaitForSeconds(3);
        }
    }

    private GameObject FindFirstPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 1; i <= players.Length; i++)
        {
            foreach (GameObject p in players)
            {
                if (p.GetComponent<MultiplayerController>().placement == i && p.GetComponent<MultiplayerController>().currentState != PlayerStates.Finish)
                {
                    return p;
                }
            }
        }
        return null;
    }
}
