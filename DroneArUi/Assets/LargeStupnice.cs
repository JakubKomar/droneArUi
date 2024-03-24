using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using TMPro;
public class LargeStupnice : MonoBehaviour
{
    public string text="";

    [SerializeField]
    private TextMeshProUGUI tmp;
    public bool red = false;

    [SerializeField]
    private SmallStupnice smallStupnice;

    private void Update()
    {
        tmp.text = text;

        tmp.color = red?Color.red:Color.green;
        smallStupnice.red = red;
    }
}
