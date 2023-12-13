/*
 * UserProfile manager - class to store users data as height, threshold values.
 * 
 * Author : Martin Kyjac (xkyjac00)
 */

using TMPro;

public class UserProfileManager : Singleton<UserProfileManager>
{
    public float Height;
    public string DroneName;
    public string Username;
    public decimal DroneThreshold;
    public TextMeshPro TresholdSetting;
    public TrackingTypeEnum TrackingType;
    public TextMeshPro TrackingTypeText;
    public bool DisplayControlledDrone;

    private readonly decimal Increasement = 0.1m;
    private readonly string TrackingTypeTemplate = "Current tracking by\n{0}";
    // Start is called before the first frame update
    void Start()
    {
        TresholdSetting.text = "0";
        TrackingTypeText.text = string.Format(TrackingTypeTemplate, TrackingType.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseTreshold()
    {
        DroneThreshold += Increasement;
        SetThresholdText();
    }

    public void DecreaseTreshold()
    {
        if (DroneThreshold >= Increasement)
        {
            DroneThreshold -= Increasement;
            SetThresholdText();
        }    
    }

    public void ToggleDroneModel()
    {
        var droneModel = DroneManager.Instance.ControlledDroneGameObject;
        droneModel.SetActive(!droneModel.activeSelf);
    }

    public void ToggleTrackingType()
    {
        if (TrackingType == TrackingTypeEnum.GPS)
        {
            TrackingType = TrackingTypeEnum.IMU;
        }
        else
        {
            TrackingType = TrackingTypeEnum.GPS;
        }

        TrackingTypeText.text = string.Format(TrackingTypeTemplate, TrackingType.ToString());
    }

    public void ToggleControlledDroneDisplay()
    {
        DisplayControlledDrone = !DisplayControlledDrone;
    }

    private void SetThresholdText()
    {
        TresholdSetting.text = string.Format("{0:0.0}", DroneThreshold);
    }
}

public enum TrackingTypeEnum
{
    GPS,
    IMU
}
