// autor jakub komárek
using Microsoft.MixedReality.Toolkit.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeechSyntetizer : MonoBehaviour
{
    // Start is called before the first frame update
    private TextToSpeech textToSpeech;
    private calibrationScript calibrationScript;

    void Start()
    {
        textToSpeech=GetComponent<TextToSpeech>();
        calibrationScript=GetComponent<calibrationScript>();
        calibrationScript.calibrationEvent.AddListener(()=> { onSay("calibration finished"); });
    }
   

    public void onSay(string toSay)
    {
        Debug.Log("calibration say");
        if(toSay =="") { return; }
        textToSpeech.StopSpeaking();
        
        textToSpeech.StartSpeaking(toSay); 
    }
}
