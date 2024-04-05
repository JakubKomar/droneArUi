/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// setter pro malou stupnici indikátoru výšky
/// </summary>

using UnityEngine;

public class SmallStupnice : MonoBehaviour
{
    public bool red=false;

    [SerializeField]
    private Material redMaterial;
    [SerializeField]
    private Material greenMaterial;
    [SerializeField]
    private Renderer square;

    void Update()
    {
        if (red)      
            square.material=redMaterial;
        else 
            square.material=greenMaterial;
        
    }
}
