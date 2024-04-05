/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// reteger - ji� se nepou��v�
/// </summary>
using UnityEngine;

public class LayerSetter : MonoBehaviour
{

    void Update()
    {
        //int layer = this.gameObject.layer;
        // this.transform.SetLayerRecursively(layer);
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == 6)
            {//pokud je objekt vytvo�en skriptem mapbox - vrstva 6
                child.gameObject.layer = 7;// za�a� ji do vrstvy 7-pro skryt�
            }
        }

    }
}
