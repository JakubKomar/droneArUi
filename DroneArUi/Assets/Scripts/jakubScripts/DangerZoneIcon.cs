/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// setter pro ikony varování 
/// </summary>
using UnityEngine;

public class DangerZoneIcon : MonoBehaviour
{
    [SerializeField]
    private GameObject warningZoneIcon;
    [SerializeField]
    private GameObject dangerZoneIcon;

    [SerializeField]
    MapData mapData;
    void Start()
    {
        mapData =MapData.Instance;
    }

    void Update()
    {
        if(mapData.droneInBarier)
        {
            warningZoneIcon.SetActive(false);
            dangerZoneIcon.SetActive(true);
        }
        else if (mapData.droneInWarningBarier) {
            warningZoneIcon.SetActive(true);
            dangerZoneIcon.SetActive(false);
        }
        else
        {
            warningZoneIcon.SetActive(false);
            dangerZoneIcon.SetActive(false);
        }
    }
}
