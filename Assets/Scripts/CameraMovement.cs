using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private Vector2 lastSpeed;

    private int spectatingTarget = 0;
    private bool spectating = false;
    private bool countdownFlag = true;

    [SerializeField] public GameObject player;
    private List<GameObject> players = new List<GameObject>();
    public MultiplayerController pC;
    public MultiplayerController localPC;

    public void Initialize(GameObject player)
    {
        this.player = player;
        pC = player.GetComponent<MultiplayerController>();
        localPC = pC;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    private void Update()
    {
        if (pC.currentState == PlayerStates.Finish && countdownFlag)
        {
            countdownFlag = false;
            StartCoroutine(SpectatingCountdown());
        }

        if (spectating)
        {
            FilterPlayerList();
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            spectatingTarget--;
            if (spectatingTarget < 0)
            {
                spectatingTarget = players.Count - 1;
            }
            pC.UnfollowPlayer();
            pC = players[spectatingTarget].GetComponent<MultiplayerController>();
            player = players[spectatingTarget];
            pC.FollowPlayer();
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            spectatingTarget++;
            if (spectatingTarget >= players.Count)
            {
                spectatingTarget = 0;
            }
            pC.UnfollowPlayer();
            pC = players[spectatingTarget].GetComponent<MultiplayerController>();
            player = players[spectatingTarget];
            pC.FollowPlayer();
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            localPC.ToggleResults();
        }
    }

    private void FixedUpdate()
    {
        if (pC == null) { return; }
        if (!spectating)
        {
            lastSpeed = pC.lastSpeed / 2;

            if (pC.strafingLeft && pC.rotatingLeft)
            {
                lastSpeed = RotateCamera(lastSpeed, 40);
            }
            else if (pC.strafingRight && pC.rotatingLeft)
            {
                lastSpeed = RotateCamera(lastSpeed, 10);
            }
            else if (pC.rotatingLeft)
            {
                lastSpeed = RotateCamera(lastSpeed, 20);
            }

            if (pC.strafingRight && pC.rotatingRight)
            {
                lastSpeed = RotateCamera(lastSpeed, -40);
            }
            else if (pC.strafingLeft && pC.rotatingRight)
            {
                lastSpeed = RotateCamera(lastSpeed, -10);
            }
            else if (pC.rotatingRight)
            {
                lastSpeed = RotateCamera(lastSpeed, -20);
            }

            Vector3 targetPosition = Vector3.zero;
            if (pC.rolling || pC.bounceTime > Time.time)
            {
                targetPosition = player.transform.position + offset;
            }
            else
            {
                targetPosition = player.transform.position + new Vector3(lastSpeed.x, lastSpeed.y, 0) + offset;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            lastSpeed = pC.lastSpeed / 2;
            Vector3 targetPosition = Vector3.zero;
            if (pC.rolling || pC.bounceTime > Time.time)
            {
                targetPosition = player.transform.position + offset;
            }
            else
            {
                targetPosition = player.transform.position + new Vector3(lastSpeed.x, lastSpeed.y, 0) + offset;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    private Vector2 RotateCamera(Vector2 position, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * position;
    }

    private void FilterPlayerList()
    {
        foreach (GameObject player in players)
        {
            if (player.GetComponent<MultiplayerController>().currentState == PlayerStates.Finish)
            {
                players.Remove(player);
            }
        }
    }

    private void InitializePlayerList()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<MultiplayerController>().currentState != PlayerStates.Finish)
            {
                players.Add(player);
            }
        }
    }

    private IEnumerator SpectatingCountdown()
    {
        InitializePlayerList();
        yield return new WaitForSeconds(3);
        spectating = true;
        pC = players[spectatingTarget].GetComponent<MultiplayerController>();
        player = players[spectatingTarget];
        pC.FollowPlayer();
    }
}
