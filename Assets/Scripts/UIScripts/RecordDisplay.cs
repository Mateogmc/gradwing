using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordDisplay : MonoBehaviour
{
    ScrollRect scrollRect;

    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI placement;

    [SerializeField] TMP_FontAsset lessThan100;
    [SerializeField] TMP_FontAsset moreThan100;

    RectTransform rt;

    bool isLocalPlayer = false;
    [SerializeField] int placementValue = 0;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        //transform.parent.parent
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            Vector2 newPosition = new Vector2(rt.localPosition.x, - (80 * (placementValue - 1)));

            //rt.localPosition = newPosition;
        }
    }

    public void Display(int placement, string name, string time)
    {
        this.name.text = System.Text.RegularExpressions.Regex.Unescape(name);
        this.time.text = time;

        if (placement < 20)
        {
            this.placement.font = lessThan100;

            if (placement % 10 == 0)
            {
                this.placement.text = "";
                for (int i = 0; i < (placement / 10); i++)
                {
                    this.placement.text += "0";
                }
            }
            else
            {
                string text = Mathf.FloorToInt(placement % 10).ToString();

                for (int i = 0; i < Mathf.FloorToInt(placement / 10); i++)
                {
                    text += "0";
                }
                this.placement.text = text;
            }
        }
        else
        {
            this.placement.font = moreThan100;
            this.placement.text = placement.ToString();
        }

        if (name == DataManager.username)
        {
            this.placement.color = new Color(1, 1, 0.5f);
            this.name.color = new Color(1, 1, 0.5f);
            this.time.color = new Color(1, 1, 0.5f);
            GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.5f);
            placementValue = placement;
            isLocalPlayer = true;
        }

        rt.localPosition = new Vector2(rt.localPosition.x, -40 - (80 * placement));
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
