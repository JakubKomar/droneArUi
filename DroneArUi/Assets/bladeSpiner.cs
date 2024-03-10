// author jakub komárek

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bladeSpiner : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private List<GameObject> bladeList = new List<GameObject>();

    [SerializeField]
    private GameObject gymbalPart=null;

    [SerializeField]
    private float gymbalTilt = 0;

    public bool spinRotors = true;

    [SerializeField]
    private float rotationSpeed = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spinRotors)
        {
            foreach (GameObject blade in bladeList)
            {
                blade.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }
        }

        gymbalPart.transform.localEulerAngles = new Vector3(gymbalTilt,0,0);
    }
}
