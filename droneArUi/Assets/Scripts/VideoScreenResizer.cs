/*
Author: Bc. Kamil Sedlmajer (kamilsedlmajer@gmail.com)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoScreenResizer : MonoBehaviour
{
    public int FOV;
    public int width;
    public int height;
    public float distance;
    public Texture noVideoTexture;

    public Slider FowSlider;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("CameraResWidth") && PlayerPrefs.HasKey("CameraResHeight") && PlayerPrefs.HasKey("CameraFOV"))
        {
            FOV = PlayerPrefs.GetInt("CameraFOV");
            width = PlayerPrefs.GetInt("CameraResWidth");
            height = PlayerPrefs.GetInt("CameraResHeight");
            distance = PlayerPrefs.GetFloat("CameraScreenDistance");

            resize();
        }

        Material material = gameObject.GetComponent<Renderer>().sharedMaterial;
        // material.SetTexture("_MainTex", noVideoTexture);
        material.SetTextureScale("_MainTex", new Vector2(1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        //resize();
    }

    void resize()
    {
        float videoSizeWhidth = Mathf.Tan((FOV / 2) * Mathf.Deg2Rad) * 2 * distance;
        float videoSizeHeight = videoSizeWhidth / width * height;

        transform.localPosition = new Vector3(-1*distance, transform.localPosition.y, transform.localPosition.z);
        transform.localScale = new Vector3(videoSizeWhidth, videoSizeHeight, 0.01f);

    }

    public void resizeSlider(float newFOV){
        PlayerPrefs.SetInt("CameraFOV", (int)newFOV);
        FOV = (int)newFOV;
        resize();
    }

    public void ChangeHeight(float newHeight){
        PlayerPrefs.SetInt("CameraResHeight", (int)newHeight);
        height = (int)newHeight;
        resize();
    }

        public void ChangeWidth(float newWidth){
        PlayerPrefs.SetInt("CameraResWidth", (int)newWidth);
        width = (int)newWidth;
        resize();
    }

    public void ChangeDistance(float newDistance){
        PlayerPrefs.SetFloat("CameraScreenDistance", newDistance);
        distance = (int)newDistance;
        resize();
    }
    
}
