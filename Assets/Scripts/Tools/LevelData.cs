using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;

public class LevelData : MonoBehaviour
{
    [SerializeField] Material illusionGroundMaterial;
    [SerializeField] Material[] textMaterials;

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
                worldName = "Everage";
                break;

            case WorldName.Sand:
                worldName = "DryOasis";
                break;

            case WorldName.Water:
                worldName = "DeepDive";
                break;

            case WorldName.Ice:
                worldName = "InfiniteFrost";
                break;

            case WorldName.Fire:
                worldName = "ScorchedFuture";
                break;

            case WorldName.Lightning:
                worldName = "HeavyLight";
                break;

            case WorldName.Clouds:
                worldName = "Hyperdrive";
                break;

            case WorldName.Illusion:
                worldName = "NoBarriers";
                break;
        }
        return worldName;
    }

    public string GetCourse()
    {
        string courseName = "";
        switch (world)
        {
            case WorldName.Lobby:
                courseName = "Lobby";
                break;

            case WorldName.City:
                courseName = "Metropolis";
                break;

            case WorldName.Sand:
                courseName = "Sand Flow";
                break;

            case WorldName.Water:
                courseName = "Aquatic Expanse";
                break;

            case WorldName.Ice:
                courseName = "Freeze Torrent";
                break;

            case WorldName.Fire:
                courseName = "Basalt Ridge";
                break;

            case WorldName.Lightning:
                courseName = "High Voltage";
                break;

            case WorldName.Clouds:
                courseName = "Cyclone";
                break;

            case WorldName.Illusion:
                courseName = "Cyberia";
                break;
        }

        courseName += " - " + SceneManager.GetActiveScene().name;
        return courseName;
    }

    public Material GetMaterial()
    {
        switch (world)
        {
            case WorldName.City:
                return textMaterials[0];

            case WorldName.Sand:
                return textMaterials[1];

            case WorldName.Water:
                return textMaterials[2];

            case WorldName.Ice:
                return textMaterials[3];

            case WorldName.Fire:
                return textMaterials[4];

            case WorldName.Lightning:
                return textMaterials[5];

            case WorldName.Clouds:
                return textMaterials[6];

            case WorldName.Illusion:
                return textMaterials[7];
        }
        return textMaterials[7];
    }
}
