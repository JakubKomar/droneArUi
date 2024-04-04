using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFliper : MonoBehaviour
{
    // Start is called before the first frame update
    //Flips the UV on the backside of the cube so it matches the front and sides
    void Start()
    {
        Vector2[] uvs = GetComponent<MeshFilter>().sharedMesh.uv;


        Debug.Log(uvs);

        uvs[6] = new Vector2(1, 0); 
        uvs[7] = new Vector2(0, 0);
        uvs[10] = new Vector2(1, 1);
        uvs[11] =  new Vector2(0, 1);

        GetComponent<MeshFilter>().sharedMesh.uv = uvs;
    }
}
