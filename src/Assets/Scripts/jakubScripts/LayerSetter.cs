/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// reteger - již se nepoužívá
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
            {//pokud je objekt vytvoøen skriptem mapbox - vrstva 6
                child.gameObject.layer = 7;// zaøaï ji do vrstvy 7-pro skrytí
            }
        }

    }
}
