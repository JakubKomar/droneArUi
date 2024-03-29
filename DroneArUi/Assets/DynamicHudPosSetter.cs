using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicHudPosSetter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform mainCamera;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = mainCamera.transform.position;
    }
}
