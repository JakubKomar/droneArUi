using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManipulatorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool rotationMan=false; 
    Vector3 lastKnownPos=Vector3.zero;
    void Update()
    {   
        if(rotationMan)
        {
            this.transform.position = lastKnownPos;

        }
        else
        {
            lastKnownPos = this.transform.position;
        }
    
    }

    public void onRotationManStart() {
        rotationMan=true;
    }

    public void onRotationManStop()
    {
        rotationMan = false ;
    }
}
