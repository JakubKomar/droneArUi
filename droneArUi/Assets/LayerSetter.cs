using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LayerSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //int layer = this.gameObject.layer;
       // this.transform.SetLayerRecursively(layer);
       foreach(Transform child in transform)
        {
            if (child.gameObject.layer==6) {//pokud je objekt vytvoøen skriptem mapbox - vrstva 6
                child.gameObject.layer = 7;// zaøaï ji do vrstvy 7-pro skrytí
            }
        }

    }
}
