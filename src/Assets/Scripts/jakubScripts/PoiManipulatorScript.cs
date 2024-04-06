/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// testovací poi manipulator skript
/// </summary>
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class PoiManipulatorScript : MonoBehaviour
{

    private ObjectManipulator objectManipulator = null;
    void Start()
    {
        objectManipulator = GetComponent<ObjectManipulator>();
        if (objectManipulator == null)
        {
            Debug.LogError("manipulator null err");
        }

        objectManipulator.OnManipulationStarted.AddListener(onManipultaionStart);
        objectManipulator.OnManipulationEnded.AddListener(onManipulationEnd);
        objectManipulator.OnHoverEntered.AddListener(onHoverStart);
        objectManipulator.OnHoverExited.AddListener(onHoverEnd);
    }

    public void onHoverStart(ManipulationEventData eventData)
    {
        Debug.Log("hover start");
    }

    public void onHoverEnd(ManipulationEventData eventData)
    {
        Debug.Log("hover end");
    }
    public void onManipultaionStart(ManipulationEventData eventData)
    {
        Debug.Log("manipulataion start");
    }
    public void onManipulationEnd(ManipulationEventData eventData)
    {
        Debug.Log("manipulataion ends");
    }
}
