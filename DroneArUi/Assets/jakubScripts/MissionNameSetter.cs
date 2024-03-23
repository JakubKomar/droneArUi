// autor jakubk kom�rek

using TMPro;
using UnityEngine;

public class MissionNameSetter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshPro textMeshPro;

    MapData mapData;
    void Start()
    {
        mapData= FindObjectOfType<MapData>();

    }

    // Update is called once per frame
    void Update()
    {
        if (mapData != null && textMeshPro != null) {
            textMeshPro.text = "Opened:" + mapData.misionName;
        }
    }
}