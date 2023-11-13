using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    [HideInInspector] public static VFXManager instance;

    [SerializeField] GameObject flashExplosion;
    [SerializeField] GameObject carCrash;
    [SerializeField] GameObject wallSparks;
    [SerializeField] GameObject rebounderSparks;
    [SerializeField] GameObject bounceRipple;
    [SerializeField] GameObject fanCrash;
    [SerializeField] GameObject itemTake;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Flash(Vector3 initPos, Vector3 finalPos)
    {
        GameObject l = Instantiate(flashExplosion, finalPos, Quaternion.identity);
        float angle = -Vector2.SignedAngle(initPos - finalPos, Vector3.right);
        l.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        l.GetComponent<FlashEffect>().Initialize((initPos - finalPos).magnitude);
    }

    public void CarCrash(Vector3 pos1, Vector3 pos2, Vector3 contactPoint, float velocity)
    {
        float angle = -Vector2.SignedAngle(pos2 - pos1, Vector3.right);
        Instantiate(carCrash, contactPoint, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<VisualEffect>().SetFloat("Velocity", Mathf.Lerp(200, 1400, velocity / 160));
    }

    public void WallSparks(Vector3 pos, Vector2 normal)
    {
        float angle = -Vector2.SignedAngle(normal, Vector3.right);
        Instantiate(wallSparks, pos, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<VisualEffect>().SetVector4("Color", FindObjectOfType<LevelData>().GetColor());
    }

    public void Rebounder(Vector3 pos1, Vector3 pos2, Vector3 contactPoint, float velocity)
    {
        float angle = -Vector2.SignedAngle(pos2 - pos1, Vector3.right);
        Instantiate(rebounderSparks, contactPoint, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<VisualEffect>().SetFloat("Velocity", Mathf.Lerp(200, 1400, velocity / 160));
    }

    public void BounceRipple(Vector3 pos, Vector2 normal)
    {
        float angle = -Vector2.SignedAngle(normal, Vector3.right) + 180;
        StartCoroutine(Ripple(pos, angle));
    }

    private IEnumerator Ripple(Vector3 pos, float angle)
    {
        GameObject g = Instantiate(bounceRipple, pos, Quaternion.AngleAxis(angle, Vector3.forward));
        Color c = FindObjectOfType<LevelData>().GetColor();
        g.GetComponent<VisualEffect>().SetVector4("Color", new Vector4(c.g, c.b, c.r, c.a));
        g.GetComponent<VisualEffect>().SetFloat("Angle", angle);
        for (int i = 0; i < 8; i++)
        {
            g.GetComponent<VisualEffect>().SendEvent("Play");
            yield return new WaitForSeconds(0.15f - (0.5f / i));
        }
    }

    public void FanCrash(Vector3 pos1, Vector3 pos2, float velocity)
    {
        float angle = -Vector2.SignedAngle(pos2 - pos1, Vector3.right);
        Instantiate(fanCrash, pos2, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<VisualEffect>().SetFloat("Velocity", Mathf.Lerp(1000, 3000, velocity / 40));
    }

    public void ItemTake(Vector3 pos)
    {
        Instantiate(itemTake, pos, Quaternion.identity);
    }
}
