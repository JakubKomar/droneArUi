using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedIndicatorControler : MonoBehaviour
{
    // Start is called before the first frame update
    Drone contDrone=null;
    DroneManager droneManager=null;

    [SerializeField]
    RectTransform point =null;

    double canvasWidth=0;
    double canvasHeight=0;

    [SerializeField]
    public float testVelX = 0;
    [SerializeField]
    public float testVelY = 0;
    void Start()
    {
        droneManager = FindObjectOfType<DroneManager>();

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            // Získat rozmìry plátna
            canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
            canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        }
    }

    // Update is called once per frame
    void Update()
    {
        contDrone = droneManager.ControlledDrone;
        double velocityX = 0;
        double velocityY = 0;
        if (contDrone != null)
        {
            velocityX = contDrone.FlightData.VelocityX;
            velocityY = contDrone.FlightData.VelocityY;
        }
        else
        {
            velocityX = testVelX; velocityY =testVelY;
        }

        // rozsah do 18 m/s
        float newPosY = (float)((velocityX/18)* canvasWidth / 2);
        float newPosX = (float)((velocityY / 18) * canvasHeight / 2);

        point.anchoredPosition = new Vector2(newPosX, newPosY);
    }
}
