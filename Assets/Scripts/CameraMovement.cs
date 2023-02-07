using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private Vector2 lastSpeed;

    [SerializeField] private GameObject player;
    private PlayerController pC;

    private void Start()
    {
        pC = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        lastSpeed = pC.lastSpeed / 2;

        if (pC.strafingLeft && pC.rotatingLeft)
        {
            lastSpeed = RotateCamera(lastSpeed, 40);
        } else if (pC.strafingRight && pC.rotatingLeft)
        {
            lastSpeed = RotateCamera(lastSpeed, 10);
        } else if (pC.rotatingLeft)
        {
            lastSpeed = RotateCamera(lastSpeed, 20);
        }

        if (pC.strafingRight && pC.rotatingRight)
        {
            lastSpeed = RotateCamera(lastSpeed, -40);
        } else if ( pC.strafingLeft && pC.rotatingRight)
        {
            lastSpeed = RotateCamera(lastSpeed, -10);
        } else if (pC.rotatingRight)
        {
            lastSpeed = RotateCamera(lastSpeed, -20);
        }

        Vector3 targetPosition = Vector3.zero;
        if (pC.rolling || pC.bounceTime > Time.time)
        {
            targetPosition = player.transform.position + offset;
        } else
        {
            targetPosition = player.transform.position + new Vector3(lastSpeed.x, lastSpeed.y, 0) + offset;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private Vector2 RotateCamera(Vector2 position, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * position;
    }
}
