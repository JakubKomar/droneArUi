using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationGround : Singleton<CalibrationGround>
{
    // Start is called before the first frame update
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

    // Update is called once per frame
    bool inCalibration=false;
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
        inCalibration=state;

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
