using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallStupnice : MonoBehaviour
{
    // Start is called before the first frame update
    public bool red=false;

    [SerializeField]
    private Material redMaterial;
    [SerializeField]
    private Material greenMaterial;
    [SerializeField]
    private Renderer square;
    // Update is called once per frame
    void Update()
    {
        if (red)      
            square.material=redMaterial;
        else 
            square.material=greenMaterial;
        
    }
}
