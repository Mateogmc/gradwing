using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flash : MonoBehaviour
{
    SpriteRenderer sr;
    CircleCollider2D cc;

    [SerializeField] Material material;
    [SerializeField] float jumpDistance;
    [SerializeField] float lerpAmount;
    [SerializeField] Vector4 color1;
    [SerializeField] Vector4 color2;
    [SerializeField] Vector4 color1Inactive;
    [SerializeField] Vector4 color2Inactive;

    [SerializeField] LineRenderer lr;
    [SerializeField] Material lrMaterial;

    static GameObject explosion;
    [SerializeField] GameObject explosionSerialized;

    MultiplayerController pC;
    public bool canUse;
    int ground;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CircleCollider2D>();
        sr.material = new Material(material);
        lr.material = new Material(lrMaterial);
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.0001f);
        if (collider != null && collider.CompareTag("Ground"))
        {
            Enable(true);
            ground = 1;
        }
        else
        {
            Enable(false);
            ground = 0;
        }

        explosion = explosionSerialized;
    }

    private void Update()
    {
        lr.SetPosition(0, pC.gameObject.transform.position);
        lr.SetPosition(1, transform.position);
        lr.material.SetFloat("_Tiling", (lr.GetPosition(0) - lr.GetPosition(1)).magnitude / 10);

        Enable(ground > 0);
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Instantiate(MultiplayerController mc)
    {
        pC = mc;
        transform.position = mc.gameObject.transform.position;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Move()
    {
        Vector3 targetPosition = pC.transform.position + (Quaternion.Euler(pC.transform.rotation.eulerAngles) * Vector2.right * jumpDistance);
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount);
    }

    private void Enable(bool enable)
    {
        if (enable)
        {
            canUse = true;
            sr.material.SetColor("_ArrowsColor1", color1);
            sr.material.SetColor("_ArrowsColor2", color2);
        }
        else
        {
            canUse = false;
            sr.material.SetColor("_ArrowsColor1", color1Inactive);
            sr.material.SetColor("_ArrowsColor2", color2Inactive);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Enable(true);
            ground++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Enable(false);
            ground--;
        }
    }
}
