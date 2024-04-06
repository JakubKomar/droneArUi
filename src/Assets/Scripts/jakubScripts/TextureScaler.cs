/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// scaluje texturu podle lok�ln�ho m���dka
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScaler : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField]
    GameObject obj;
    void Update()
    {
        Renderer objectRenderer = obj.GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            Vector3 objectScale = transform.localScale;
            Material material = objectRenderer.material;

            // Vypo��t�n� nov�ho m���tka textury na z�klad� m���tka objektu
            Vector2 newScale = new Vector2(objectScale.x*0.1f, objectScale.y*0.1f);

            // Nastaven� nov�ho m���tka textury
            material.mainTextureScale = newScale;
        }
        else
        {
            Debug.LogError("Tento objekt neobsahuje renderer.");
        }
    }
}
