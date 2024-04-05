/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Nastavuje rotaci objektu, tak aby smìøovala k hráèi
/// </summary>
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

        // Nastaví rotaci smìrm ke kameøe
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
    }
}
