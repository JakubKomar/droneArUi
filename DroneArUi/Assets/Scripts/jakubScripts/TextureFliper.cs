/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Pøevrací zadní texturu kostky, tak aby text byl èitelný
/// </summary>

using UnityEngine;

public class TextureFliper : MonoBehaviour
{

    void Start()
    {
        Vector2[] uvs = GetComponent<MeshFilter>().sharedMesh.uv;


        Debug.Log(uvs);

        uvs[6] = new Vector2(1, 0);
        uvs[7] = new Vector2(0, 0);
        uvs[10] = new Vector2(1, 1);
        uvs[11] = new Vector2(0, 1);

        GetComponent<MeshFilter>().sharedMesh.uv = uvs;
    }
}
