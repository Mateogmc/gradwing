using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitComponent : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI playerHitting;
    [SerializeField] TextMeshProUGUI playerHit;
    [SerializeField] Image itemSprite;
    public List<TextMeshProUGUI> players = new List<TextMeshProUGUI>();
    public Items item;
    float t = 0;
    float speed = 0.02f;

    private void Start()
    {
        transform.position = transform.position + Vector3.up * 100;
    }

    public void Initialize(string playerHitting, string playerHit, Items item)
    {
        this.playerHitting.text = playerHitting;
        itemSprite.sprite = Resources.Load<Sprite>("Items/" + item.ToString());
        this.item = item;

        AddHit(playerHit);
    }

    public void Accelerate()
    {
        speed += 0.02f;
    }

    public void AddHit(string playerHit)
    {
        TextMeshProUGUI ins = Instantiate(this.playerHit, this.itemSprite.transform).GetComponent<TextMeshProUGUI>();
        ins.transform.position += Vector3.down * 100 * players.Count;
        ins.text = playerHit;
        players.Add(ins);
        ResetComponent();
    }

    public void ResetComponent()
    {
        transform.position = new Vector3(transform.position.x, 100, 0);
        t = 0;
    }

    private void FixedUpdate()
    {
        if (t > 3) 
        {
            HitManager.instance.hitComponents.Remove(this);
            Destroy(gameObject);
        }
        transform.position += new Vector3(0, t * t, 0);

        playerHitting.alpha = Mathf.Lerp(1, 0, Mathf.Pow(t / 3,2));
        foreach (TextMeshProUGUI player in players)
        {
            player.alpha = Mathf.Lerp(1, 0, Mathf.Pow(t / 3, 2));
        }
        itemSprite.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, Mathf.Pow(t / 3, 2)));

        t += speed;
    }
}
