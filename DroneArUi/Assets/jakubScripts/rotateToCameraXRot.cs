//author jakub komárek
using UnityEngine;

public class rotateToCameraXRot : MonoBehaviour
{
    public Camera _camera;

    public void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        Vector3 lookDirection = _camera.transform.position - transform.position;

        // Set the rotation to face the camera only on the Y-axis
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
    }
}
