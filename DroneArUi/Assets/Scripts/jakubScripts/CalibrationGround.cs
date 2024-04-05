/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// nastavuje podlo�ku pro v��kovou kalibraci
/// </summary>
using UnityEngine;

public class CalibrationGround : Singleton<CalibrationGround>
{
    [SerializeField]
    GameObject groundObj;
    [SerializeField]
    GameObject manipulator;
    [SerializeField]
    GameObject wordScaleMap;
    void Start()
    {
        groundObj.SetActive(false);
    }

    bool inCalibration = false;
    void Update()
    {

        if (inCalibration)
        {
            float x = manipulator.transform.position.x;
            float z = manipulator.transform.position.z;
            float y = wordScaleMap.transform.position.y;

            this.transform.position = new Vector3(x, y, z);
            this.transform.rotation = manipulator.transform.rotation;
        }
    }

    public void setCalibration(bool state)
    {
        inCalibration = state;

        if (inCalibration)
        {
            groundObj.SetActive(true);
        }
        else
        {
            groundObj.SetActive(false);
        }
    }
}
