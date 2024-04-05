using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVcontroler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject miniPlayer;
    [SerializeField]
    private GameObject fpvPlayer;

    [SerializeField]
    bool fpvMod=false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onFpvToggle()
    {
        fpvMod = !fpvMod;
        miniPlayer.SetActive(!fpvMod);
        fpvPlayer.SetActive(fpvMod);
    }
}
