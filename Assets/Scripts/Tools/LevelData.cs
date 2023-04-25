using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LevelData : MonoBehaviour
{
    [SerializeField] Material illusionGroundMaterial;

    private void Awake()
    {
        if (world == WorldName.Illusion)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Ground"))
            {
                g.GetComponent<SpriteShapeRenderer>().material = illusionGroundMaterial;
                g.GetComponent<SpriteShapeRenderer>().color = new Color(1, 1, 1, 0.2f);
            }
        }
    }

    enum WorldName
    {
        Lobby,
        City,
        Sand,
        Water,
        Ice,
        Fire,
        Lightning,
        Clouds,
        Illusion
    }

    [SerializeField]
    WorldName world;

    public Vector4 GetColor()
    {
        Vector4 color = Vector4.zero;
        switch (world)
        {
            case WorldName.Lobby:
                color = new Vector4(2, 20, 10, 1);
                break;

            case WorldName.City:
                color = new Vector4(10, 2, 100, 1);
                break;

            case WorldName.Sand:
                color = new Vector4(80, 30, 2, 1);
                break;

            case WorldName.Water:
                color = new Vector4(0, 2, 100, 1);
                break;

            case WorldName.Ice:
                color = new Vector4(0, 10, 10, 1);
                break;

            case WorldName.Fire:
                color = new Vector4(100, 5, 2, 1);
                break;

            case WorldName.Lightning:
                color = new Vector4(30, 30, 3, 1);
                break;

            case WorldName.Clouds:
                color = new Vector4(6, 10, 10, 1);
                break;

            case WorldName.Illusion:
                color = new Vector4(0, 100, 2, 1);
                break;
        }
        return color;
    }

    public string GetWorld()
    {
        string worldName = "";
        switch (world)
        {
            case WorldName.Lobby:
                worldName = "Lobby";
                break;

            case WorldName.City:
                worldName = "NoBarriers";
                break;

            case WorldName.Sand:
                worldName = "Default";
                break;

            case WorldName.Water:
                worldName = "InfiniteFrost";
                break;

            case WorldName.Ice:
                worldName = "InfiniteFrost";
                break;

            case WorldName.Fire:
                worldName = "Default";
                break;

            case WorldName.Lightning:
                worldName = "HeavyLight";
                break;

            case WorldName.Clouds:
                worldName = "HeavyLight";
                break;

            case WorldName.Illusion:
                worldName = "NoBarriers";
                break;
        }
        return worldName;
    }
}
