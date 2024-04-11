/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// obsluhuje kruhový indikátor rychlosti - již se nepoužívá
/// </summary>
using UnityEngine;

public class speedIndicatorControler : MonoBehaviour
{
    Drone contDrone = null;
    DroneManager droneManager = null;

    [SerializeField]
    RectTransform point = null;

    float canvasWidth = 0;
    float canvasHeight = 0;

    [SerializeField]
    public float testVelX = 0;
    [SerializeField]
    public float testVelY = 0;
    [SerializeField]
    public float testCompass = 0;
    Canvas canvas;
    void Start()
    {
        droneManager = FindObjectOfType<DroneManager>();
        canvas = this.gameObject.GetComponent<Canvas>();
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
    }

    void Update()
    {
        contDrone = droneManager.ControlledDrone;
        float velocityX = 0;
        float velocityY = 0;
        float compass = 0;

        if (contDrone != null)
        {
            velocityX = (float)contDrone.FlightData.VelocityX;
            velocityY = (float)contDrone.FlightData.VelocityY;
            compass = (float)contDrone.FlightData.Compass;
        }
        else
        {
            velocityX = testVelX;
            velocityY = testVelY;
            compass = testCompass;
        }

        float newPosX = ((velocityX / 18) * canvasWidth / 2);
        float newPosY = ((velocityY / 18) * canvasHeight / 2);

        compass = (compass - 90) % 360;
        if (compass < 0)
            compass += 360;
        compass = -compass;



        float X1 = 0;// canvasWidth / 2;
        float Y1 = 0;// canvasHeight / 2;
        float x = newPosX;
        float y = newPosY;
        float rotationAngleRadians = compass * Mathf.Deg2Rad;
        newPosX = (x - X1) * Mathf.Cos(rotationAngleRadians) - (y - Y1) * Mathf.Sin(rotationAngleRadians) + X1;
        newPosY = (x - X1) * Mathf.Sin(rotationAngleRadians) + (y - Y1) * Mathf.Cos(rotationAngleRadians) + Y1;


        point.anchoredPosition = new Vector2(-newPosX, newPosY);
    }
}
