//autor jakub komárek

using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDisabler : MonoBehaviour
{
    // Start is called before the first frame update
    public AbstractMap abstractMap=null;
    public bool disableRender = true;
    void Start()
    {
        abstractMap = this.GetComponent<AbstractMap>();
        //abstractMap.
    }

    // Update is called once per frame
    void Update()
    {

    }
}
