using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equalizer : MonoBehaviour
{
    GameObject target;
    int ranVal;
    [SerializeField] GameObject equalizerRadius;
    [SerializeField] GameObject equalizerBlast;
    float t = 0;
    [SerializeField] public float maxDamage;

    public string username;

    public void Initialize(GameObject target, int ranVal, string parentUsername)
    {
        this.target = target;
        this.ranVal = ranVal;
        username = parentUsername;
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        int i = 100;
        while (i > 0)
        {
            GetComponent<AudioSource>().volume = DataManager.soundVolume;
            GetComponent<AudioSource>().pitch = 1 + (1 - ((float)i / 100f));
            GetComponent<AudioSource>().Play();
            i -= ranVal;
            yield return new WaitForSeconds(0.2f);
        }
        target = null;
        t = Time.time + 0.5f;
        equalizerBlast.SetActive(true);

        equalizerBlast.GetComponent<AudioSource>().volume = DataManager.soundVolume;
        equalizerBlast.GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        equalizerRadius.transform.Rotate(new Vector3(0f, 0f, 5f));
        if (t != 0 && t - Time.time <= 0)
        {
            Destroy(gameObject);
        }
        else if (equalizerBlast.activeSelf)
        {
            float s = Mathf.Lerp(0, 1, (t - Time.time) * 2);
            equalizerBlast.transform.localScale = new Vector3(s, s, s);
        }
    }

    private void Move()
    {
        if (target != null)
        {
            transform.position = target.transform.position;
        }
    }
}
