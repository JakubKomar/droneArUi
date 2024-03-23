// autor jakub komárek
using Microsoft.MixedReality.Toolkit.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeechSyntetizer : Singleton<TextToSpeechSyntetizer>
{
    // Start is called before the first frame update
    private TextToSpeech textToSpeech;

    void Start()
    {
        textToSpeech=GetComponent<TextToSpeech>();
    }
   

    public void say(string toSay)
    {
        if(textToSpeech == null) { return; }
        if(toSay =="") { return; }
        textToSpeech.StopSpeaking();
        textToSpeech.StartSpeaking(toSay); 
    }
}
