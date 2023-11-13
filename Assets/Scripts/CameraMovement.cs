using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;

    private Vector3 backOffset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.3f;
    [SerializeField] private float smoothTimeRotation = 0.00005f;
    private Vector3 velocity = Vector3.zero;
    private Vector2 lastSpeed;

    private int spectatingTarget = 0;
    private bool spectating = false;
    private bool countdownFlag = true;

    [SerializeField] public GameObject player;
    private List<GameObject> players = new List<GameObject>();
    public MultiplayerController pC;
    public MultiplayerController localPC;

    private bool cinematic = false;
    private Vector3 cinematicPosition = Vector3.zero;
    bool zoomReady = true;

    private void Start()
    {
        /*
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
        */
    }

    public void Initialize(GameObject player)
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
        if (InputManager.instance.controls.Buttons.LBumper.WasPressedThisFrame())
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
        if (InputManager.instance.controls.Buttons.RBumper.WasPressedThisFrame())
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
        if (cinematic)
        {
            transform.position = Vector3.SmoothDamp(transform.position, cinematicPosition, ref velocity, smoothTime);
        }
        else if (!spectating)
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
            if ((pC.rolling && pC.currentState == PlayerStates.Grounded) && !DataManager.GetInstance().cameraRotation && false/* || pC.bounceTime > Time.time*/)
            {
                targetPosition = player.transform.position + backOffset;
            }
            else
            {
                if (pC.rolling)
                {
                    float magn = lastSpeed.magnitude;
                    lastSpeed.x = Mathf.Cos(pC.transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
                    lastSpeed.y = Mathf.Sin(pC.transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
                    lastSpeed *= magn;
                }
                targetPosition = player.transform.position + (DataManager.GetInstance().cameraRotation ? (new Vector3(Mathf.Cos(player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0).normalized * 15) + (new Vector3(lastSpeed.x / 1.3f, lastSpeed.y / 1.3f, 0)) : new Vector3(lastSpeed.x, lastSpeed.y, 0)) + backOffset;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, (pC.rolling && pC.currentState == PlayerStates.Grounded ? smoothTime / 2 : smoothTime));
        }
        else
        {
            lastSpeed = pC.lastSpeed / 2;
            Vector3 targetPosition = Vector3.zero;
            if (pC.rolling || pC.bounceTime > Time.time)
            {
                targetPosition = player.transform.position + backOffset;
            }
            else
            {
                targetPosition = player.transform.position + new Vector3(lastSpeed.x, lastSpeed.y, 0) + backOffset;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        if (DataManager.GetInstance().cameraRotation)
        {
            if (!pC.rolling || true)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, pC.transform.rotation * Quaternion.Euler(0, 0, -90), smoothTimeRotation);
            }
        }
        else
        {
            transform.rotation = Quaternion.identity;
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

    private IEnumerator ResizeRoutine(float oldSize, float newSize, float time)
    {
        while (!zoomReady)
        {
            yield return null;
        }
        zoomReady = false;
        float elapsed = 0;
        while (elapsed <= time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(oldSize, newSize, t);
            yield return null;
        }
        zoomReady = true;
    }

    public void EnableCinematic(Vector3 position, float zoom)
    {
        cinematic = true;
        cinematicPosition = position;
        StopCoroutine(ResizeRoutine(0, 0, 0));
        StartCoroutine(ResizeRoutine(GetComponent<Camera>().orthographicSize, zoom, 0.5f));
    }

    public void DisableCinematic()
    {
        cinematic = false;
        StopCoroutine(ResizeRoutine(0, 0, 0));
        StartCoroutine(ResizeRoutine(GetComponent<Camera>().orthographicSize, 30, 0.5f));
    }
}
