// jakub komárek

using Mapbox.Directions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class AltIndicator : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject largeStupnice=null;
    [SerializeField]
    private GameObject smallStupnice=null;

    [SerializeField]
    private Transform tape;

    [SerializeField]
    private int min = 0;

    [SerializeField]
    private int max=150;
    [SerializeField]
    private int visibleCount=50;

    private float canvasWidth;
    private float canvasHeight;

    private float alt=23;
    [SerializeField]
    private float testAlt = 23;

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
        for (int i = min; i < max; i+=10)
        {
            GameObject stupnice=Instantiate(largeStupnice);
            largeStupniceList.Add(stupnice);
            LargeStupnice script= stupnice.GetComponent<LargeStupnice>();
            script.red = i > 100;
            script.text = i.ToString();
            stupnice.transform.parent = tape;

            stupnice.transform.localScale = Vector3.one;
            stupnice.transform.localPosition = new Vector3(18.2f, (canvasHeight/ visibleCount) *i, -1);

            
        }


        for (int i = min; i < max; i += 1)
        {
            GameObject stupnice = Instantiate(smallStupnice);
            smallStupniceList.Add(stupnice);
            stupnice.transform.parent = tape;

            SmallStupnice script = stupnice.GetComponent<SmallStupnice>();
            script.red = i > 100;
            stupnice.transform.localScale = Vector3.one;
            stupnice.transform.localPosition = new Vector3(23.8f, (canvasHeight / visibleCount) * i, -1);


        }
    }

    // Update is called once per frame
    void Update()
    {
        Drone myDrone = droneManager.ControlledDrone;

        if (myDrone == null) { 
            alt = testAlt; 
        }
        else { 
            alt=(float)myDrone.FlightData.Altitude; 
        }

        if (alt > 100 && Mathf.Abs(Time.time -lastAltWarning)>15f)
        {
            lastAltWarning = Time.time;
            TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("Altitude limit execeded.");
        }

        tape.transform.localPosition = new Vector3(0, -(canvasHeight / visibleCount) * alt, 0);

        int index = min;
        foreach(var obj in smallStupniceList)
        {

            if (index%10==0|| Mathf.Abs(index - alt) > visibleCount / 2 )
            {
                obj.SetActive(false);
            }
            else { 
                obj.SetActive(true);
            }
            
            index++;
  
        }

        index = min;
        foreach (var obj in largeStupniceList)
        {
            if (Mathf.Abs(index - alt) > visibleCount / 2 || Mathf.Abs(index - alt) < 2)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }

            index+=10;
        }
    }
}
