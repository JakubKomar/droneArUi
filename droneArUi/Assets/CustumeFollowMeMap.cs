// author jakub komárek

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustumeFollowMeMap :  RadialView
{

    public Transform transformMap=null;
    public Transform mainCameraTransform = null;
    public FollowMeToggle followMeTogle = null;
    void Update()
    {
        Vector3 goalPosition = base.GoalPosition;
        Quaternion goalRotation = base.GoalRotation;

        transformMap.position = goalPosition;

        goalRotation.x = 0;
        //goalRotation.y = (float)(Math.Round(goalRotation.y / 0.2f)) *0.2f;
        goalRotation.z = 0;
        transformMap.rotation = goalRotation;

    }
    private bool anchored = false;
    public void onAchorClick() {
        followMeTogle.ToggleFollowMeBehavior();
        
        anchored = !anchored;
    }
}
