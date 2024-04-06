/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Indik�tor aktu�ln� otev�en� mise
/// </summary>

using TMPro;
using UnityEngine;

public class MissionNameSetter : MonoBehaviour
{
    [SerializeField]
    TextMeshPro textMeshPro;

    MapData mapData;
    void Start()
    {
        mapData = FindObjectOfType<MapData>();

    }

    void Update()
    {
        if (mapData != null && textMeshPro != null)
        {
            textMeshPro.text = "Opened:" + mapData.misionName;
        }
    }
}
