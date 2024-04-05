/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// obsluhuje syntetizátor hlasu
/// </summary>
/// 
using Microsoft.MixedReality.Toolkit.Audio;


public class TextToSpeechSyntetizer : Singleton<TextToSpeechSyntetizer>
{
    private TextToSpeech textToSpeech;

    void Start()
    {
        textToSpeech = GetComponent<TextToSpeech>();
    }


    public void say(string toSay)
    {
        if (textToSpeech == null) { return; }
        if (toSay == "") { return; }
        textToSpeech.StopSpeaking();
        textToSpeech.StartSpeaking(toSay);
    }
}
