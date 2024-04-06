/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// setter pro velkou stupnuci
/// </summary>
using UnityEngine;
using TMPro;
public class LargeStupnice : MonoBehaviour
{
    public string text = "";

    [SerializeField]
    private TextMeshProUGUI tmp;
    public bool red = false;

    [SerializeField]
    private SmallStupnice smallStupnice;

    private void Update()
    {
        tmp.text = text;

        tmp.color = red ? Color.red : Color.green;
        smallStupnice.red = red;
    }
}
