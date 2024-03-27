// jakub komárek

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassIndicator : MonoBehaviour
{

    // Start is called before the first frame update

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
    private float canvasHeight;

    private float heading = 23;


    private List<GameObject> largeStupniceList = new List<GameObject>();
    private List<GameObject> smallStupniceList = new List<GameObject>();
    private DroneManager droneManager;

    float lastAltWarning = 0;
    void Start()
    {
        droneManager = FindObjectOfType<DroneManager>();

        canvasWidth = this.GetComponent<RectTransform>().rect.width;
        canvasHeight = this.GetComponent<RectTransform>().rect.height;

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

    // Update is called once per frame
    void Update()
    {
        heading = staticHudUp.playerHading;

        tape.transform.localPosition= new Vector3((canvasWidth / visibleCount) * (-heading),0,-1);

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

            index+=10;

        }

        index = min;
        foreach (var obj in largeStupniceList)
        {
            if (Mathf.Abs(index - heading) > visibleCount / 2 )
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }

            index += 45;
        }
    }
}



