/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Logika páskového indikátoru výšky, stará se o vytvoøení a následou èinost widgetu
/// </summary>

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AltIndicator : MonoBehaviour
{

    [SerializeField]
    private GameObject largeStupnice = null;
    [SerializeField]
    private GameObject smallStupnice = null;

    [SerializeField]
    private Transform tape;

    [SerializeField]
    private int min = 0;

    [SerializeField]
    private int max = 150;
    [SerializeField]
    private int visibleCount = 50;

    private float canvasWidth;
    private float canvasHeight;

    private float alt = 23;
    [SerializeField]
    private float testAlt = 0;

    private List<GameObject> largeStupniceList = new List<GameObject>();
    private List<GameObject> smallStupniceList = new List<GameObject>();
    private DroneManager droneManager;

    float lastAltWarning = 0;
    [SerializeField]
    private GameObject altWarnIcon;

    [SerializeField]
    private TextMeshProUGUI altText;
    void Start()
    {
        droneManager = FindObjectOfType<DroneManager>();

        canvasWidth = this.GetComponent<RectTransform>().rect.width;
        canvasHeight = this.GetComponent<RectTransform>().rect.height;
        tape = this.transform.Find("Tape");
        for (int i = min; i < max; i += 10)
        {
            GameObject stupnice = Instantiate(largeStupnice);
            largeStupniceList.Add(stupnice);
            LargeStupnice script = stupnice.GetComponent<LargeStupnice>();
            script.red = i > 100;
            script.text = i.ToString();
            stupnice.transform.parent = tape;

            stupnice.transform.localScale = Vector3.one;
            stupnice.transform.localRotation = Quaternion.identity;
            stupnice.transform.localPosition = new Vector3(23f, (canvasHeight / visibleCount) * i, -1);
        }


        for (int i = min; i < max; i += 1)
        {
            GameObject stupnice = Instantiate(smallStupnice);
            smallStupniceList.Add(stupnice);
            stupnice.transform.parent = tape;

            SmallStupnice script = stupnice.GetComponent<SmallStupnice>();
            script.red = i > 100;
            stupnice.transform.localScale = Vector3.one;
            stupnice.transform.localRotation = Quaternion.identity;
            stupnice.transform.localPosition = new Vector3(28f, (canvasHeight / visibleCount) * i, -1);


        }
    }

    void Update()
    {
        Drone myDrone = droneManager.ControlledDrone;
        float newAlt;
        if (myDrone == null)
        {
            newAlt = testAlt;
        }
        else
        {
            newAlt = (float)myDrone.FlightData.Altitude;
        }
        newAlt = Mathf.Round(newAlt * 10) / 10;

        if (alt == newAlt)
        {
            return;
        }
        alt = newAlt;
        if (altWarnIcon != null)
        {
            altWarnIcon.SetActive(alt > 100);

        }
        if (altText != null)
        {
            if (alt > 100)
                altText.color = new Color32(255, 0, 0, 255);
            else
                altText.color = new Color32(0, 255, 0, 255);
        }


        if (alt > 100 && Mathf.Abs(Time.time - lastAltWarning) > 15f)
        {
            lastAltWarning = Time.time;
            TextToSpeechSyntetizer textToSpeechSyntetizer = FindObjectOfType<TextToSpeechSyntetizer>();
            textToSpeechSyntetizer.say("Altitude limit execeded.");
        }

        tape.transform.localPosition = new Vector3(0, -(canvasHeight / visibleCount) * alt, 1);
    }
}
