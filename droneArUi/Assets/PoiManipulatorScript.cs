// autor: jakub komárek
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoiManipulatorScript : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject gmObject = null;
    private ObjectManipulator objectManipulator=null;
    void Start()
    {
        gmObject = this.gameObject;
        objectManipulator=GetComponent<ObjectManipulator>();
        if (objectManipulator == null) {
            Debug.LogError("manipulator null err");
        }

        objectManipulator.OnManipulationStarted.AddListener(onManipultaionStart);
        objectManipulator.OnManipulationEnded.AddListener(onManipulationEnd);
        objectManipulator.OnHoverEntered.AddListener(onHoverStart);
        objectManipulator.OnHoverExited.AddListener(onHoverEnd);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onHoverStart(ManipulationEventData eventData) {
        Debug.Log("hover start");
    }

    public void onHoverEnd(ManipulationEventData eventData) {
        Debug.Log("hover end");
    }
    public void onManipultaionStart(ManipulationEventData eventData) {
        Debug.Log("manipulataion start");
    }
    public void onManipulationEnd(ManipulationEventData eventData) {
        Debug.Log("manipulataion ends");
    }
}
