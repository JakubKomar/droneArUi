/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// fallow me script pro minimapu
/// </summary>

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class CustumeFollowMeMap : RadialView
{

    public Transform transformMap = null;
    public Transform mainCameraTransform = null;
    public FollowMeToggle followMeTogle = null;
    void Update()
    {
        Vector3 goalPosition = base.GoalPosition;
        Quaternion goalRotation = base.GoalRotation;

        transformMap.position = goalPosition;

        goalRotation.x = 0;
        goalRotation.z = 0;
        transformMap.rotation = goalRotation;

    }
    public void onAchorClick()
    {
        followMeTogle.ToggleFollowMeBehavior();

    }
}
