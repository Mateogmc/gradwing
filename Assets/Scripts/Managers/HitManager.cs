using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitManager : MonoBehaviour
{
    public static HitManager instance;

    [SerializeField] GameObject hitScreen;
    [SerializeField] GameObject hitPrefab;
    [SerializeField] Transform spawnPos;

    public List<HitComponent> hitComponents = new List<HitComponent>();

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ShowHit(string playerHitting, string playerHit, Items item)
    {
        foreach (HitComponent hitComponent in hitComponents)
        {
            if (hitComponent.playerHitting.text == playerHitting && hitComponent.item == item)
            {
                foreach(TextMeshProUGUI player in hitComponent.players)
                {
                    if (player.text == playerHit)
                    {
                        hitComponent.ResetComponent();
                        return;
                    }
                }
                hitComponent.AddHit(playerHit);
                return;
            }
            hitComponent.Accelerate();
        }
        GameObject ins = Instantiate(hitPrefab, spawnPos);
        ins.GetComponent<HitComponent>().Initialize(playerHitting, playerHit, item);
        hitComponents.Add(ins.GetComponent<HitComponent>());
    }
}
