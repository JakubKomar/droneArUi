
using UnityEngine;

public class DynamicHudPosSetter : MonoBehaviour
{
    [SerializeField]
    private Transform mainCamera;

    void Update()
    {
        this.transform.position = mainCamera.transform.position;
    }
}
