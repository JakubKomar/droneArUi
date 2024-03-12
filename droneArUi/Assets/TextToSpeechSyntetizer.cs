using Microsoft.MixedReality.Toolkit.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeechSyntetizer : MonoBehaviour
{
    // Start is called before the first frame update
    private TextToSpeech textToSpeech;
    private calibrationScript calibrationScript;

    public string toSay="";
    void Start()
    {
        textToSpeech=GetComponent<TextToSpeech>();
        calibrationScript=GetComponent<calibrationScript>();
        calibrationScript.calibrationEvent.AddListener(onSay);
    }
   

    public void onSay()
    {
        Debug.Log("calibration say");
        if(toSay =="") { return; }
        textToSpeech.StopSpeaking();
        
        textToSpeech.StartSpeaking(toSay); 
    }
}
