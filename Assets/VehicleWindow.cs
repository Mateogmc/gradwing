using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleWindow : MonoBehaviour
{
    [SerializeField] Image[] buttons;
    [SerializeField] LobbyUIManager manager;

    void Start()
    {
        manager.SetVehicleListeners();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].sprite = Resources.Load<Sprite>("Vehicles/Vehicle" + i);
            manager.vehicles[i] = buttons[i].GetComponent<Button>();
        }
    }
}
