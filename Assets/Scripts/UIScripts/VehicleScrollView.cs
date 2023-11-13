using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class VehicleScrollView : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform contentPanel;
    [SerializeField] RectTransform sampleListItem;

    [SerializeField] HorizontalLayoutGroup hlg;

    [SerializeField] Button[] listItems;

    public static int currentVehicle = 0;

    bool selected = false;

    private void Start()
    {
        currentVehicle = DataManager.GetInstance().spriteValue;
        for (int i = 0; i < listItems.Length; i++)
        {
            int j = i;
            listItems[i].onClick.AddListener(delegate { ChangeItem(j); });
        }
    }

    private void Update()
    {
        CheckState();
        Move();
    }

    void CheckState()
    {
        try
        {
            if (listItems.Contains(EventSystem.current.currentSelectedGameObject.GetComponent<Button>()))
            {
                if (currentVehicle != System.Array.IndexOf(listItems, EventSystem.current.currentSelectedGameObject.GetComponent<Button>()))
                {
                    if (selected == false)
                    {
                        selected = true;
                        listItems[currentVehicle].Select();
                        contentPanel.localPosition = new Vector3(0 - (currentVehicle * (sampleListItem.rect.width + hlg.spacing)), contentPanel.localPosition.y, contentPanel.localPosition.z);
                    }
                    else
                    {
                        int oldItem = currentVehicle;
                        currentVehicle = System.Array.IndexOf(listItems, EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
                    }
                }
            }
            else
            {
                selected = false;
            }
        }
        catch 
        {
            selected = false;
        }
    }

    void Move()
    {
        contentPanel.localPosition = new Vector3(Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentVehicle * (sampleListItem.rect.width + hlg.spacing)), 10), contentPanel.localPosition.y, contentPanel.localPosition.z);
    }

    void ChangeItem(int item)
    {
        currentVehicle = item;
    }
}
