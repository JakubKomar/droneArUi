/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// logika pro vytvoøení a update kompasu spolu s ikonami v nìm
/// </summary>

using System.Collections.Generic;
using UnityEngine;

public class CompassIndicator : Singleton<CompassIndicator>
{

    [SerializeField]
    private GameObject largeStupnice = null;
    [SerializeField]
    private GameObject smallStupnice = null;

    [SerializeField]
    private StaticHudDataUpdater staticHudUp = null;

    [SerializeField]
    private Transform tape;

    private int min = -360;

    private int max = 540;

    private int visibleCount = 180;

    private float canvasWidth;

    private float heading = 23;


    private List<GameObject> largeStupniceList = new List<GameObject>();
    private List<GameObject> smallStupniceList = new List<GameObject>();
    private DroneManager droneManager;


    public GameObject playerCamera;

    // objekty ve wordscalu
    public GameObject drone;
    public GameObject activeWaypoint;
    public GameObject landingPad;

    public GameObject droneIcon;
    public GameObject waypointIcon;
    public GameObject landingPadIcon;

    void Start()
    {
        droneManager = FindObjectOfType<DroneManager>();

        canvasWidth = this.GetComponent<RectTransform>().rect.width;
        //canvasHeight = this.GetComponent<RectTransform>().rect.height;

        tape = this.transform.Find("Tape");

        for (int i = min; i < max; i += 45)
        {
            GameObject stupnice = Instantiate(largeStupnice);
            largeStupniceList.Add(stupnice);
            LargeStupnice script = stupnice.GetComponent<LargeStupnice>();



            int azimut = i % 360;
            if (azimut < 0)
            {
                azimut += 360;
            }
            string text = "";
            switch (azimut)
            {
                case 0:
                    text = "N";
                    break;
                case 45:
                    text = "NE";
                    break;
                case 90:
                    text = "E";
                    break;
                case 135:
                    text = "SE";
                    break;
                case 180:
                    text = "S";
                    break;
                case 225:
                    text = "SW";
                    break;
                case 270:
                    text = "W";
                    break;
                case 315:
                    text = "NW";
                    break;
                default:
                    text = "ERR";
                    break;

            }

            script.text = text;
            stupnice.transform.parent = tape;

            stupnice.transform.localScale = Vector3.one;
            stupnice.transform.localPosition = new Vector3((canvasWidth / visibleCount) * i, 0, -1);
        }


        for (int i = min; i < max; i += 10)
        {
            GameObject stupnice = Instantiate(smallStupnice);
            smallStupniceList.Add(stupnice);
            stupnice.transform.parent = tape;

            SmallStupnice script = stupnice.GetComponent<SmallStupnice>();
            stupnice.transform.localScale = Vector3.one;
            stupnice.transform.localPosition = new Vector3((canvasWidth / visibleCount) * i, 0, -1);


        }
    }


    float calcAzimut(Transform player, Transform obj)
    {
        Vector3 direction = (obj.position - player.position).normalized;
        //direction -= staticHudUp.playerHadingOffset;
        // Spoèítejte azimut pomocí funkce Atan2
        float azimuth = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + staticHudUp.playerHadingOffset; // pøiètu calibraèní offset

        azimuth = azimuth % 360;
        // Azimut mùže být v rozsahu -180 až 180 stupòù, pøevedeme ho na rozsah 0 až 360 stupòù
        if (azimuth < 0)
        {
            azimuth += 360f;
        }

        return azimuth;
    }

    // Update is called once per frame
    void Update()
    {
        heading = staticHudUp.playerHading;

        tape.transform.localPosition = new Vector3((canvasWidth / visibleCount) * (-heading), 0, -1);

        int index = min;
        foreach (var obj in smallStupniceList)
        {

            if (index % 45 == 0 || Mathf.Abs(index - heading) > visibleCount / 2)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }

            index += 10;

        }

        index = min;
        foreach (var obj in largeStupniceList)
        {
            if (Mathf.Abs(index - heading) > visibleCount / 2)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }

            index += 45;
        }

        setIconPos(drone, droneIcon);
        setIconPos(landingPad, landingPadIcon);
        setIconPos(activeWaypoint, waypointIcon);
    }

    void setIconPos(GameObject trackedObject, GameObject icon)
    {
        if (trackedObject != null)
        {
            float az = calcAzimut(playerCamera.transform, trackedObject.transform);

            // pokouším se iconu umístit na rùzná místa na pásce - páska je orotovaná zhruba 2.5x aby byla vidìt vždy celá
            if (Mathf.Abs(az - heading) > visibleCount / 2)
            {
                az = (az + 360);

                if (Mathf.Abs(az - heading) > visibleCount / 2)
                {
                    az = az % 360;
                    az -= 360;
                    az = az % 360;

                    // pokud objekt nelze umístit, vypínáme ho
                    if (Mathf.Abs(az - heading) > visibleCount / 2)
                    {
                        icon.SetActive(false);
                        return;
                    }
                }
            }



            icon.transform.localPosition = new Vector3((canvasWidth / visibleCount) * az, 15, -1);
            icon.SetActive(true);
        }
        else
        {
            icon.SetActive(false);
        }
    }
}



