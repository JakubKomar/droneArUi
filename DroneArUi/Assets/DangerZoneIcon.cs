// jakub komárek
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneIcon : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject warningZoneIcon;
    [SerializeField]
    private GameObject dangerZoneIcon;

    [SerializeField]
    MapData mapData;
    void Start()
    {
        
    }

    // Update is called once per frame
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
