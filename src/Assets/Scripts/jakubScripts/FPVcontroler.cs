/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// p�ep�na� fpv m�du
/// </summary>

using UnityEngine;

public class FPVcontroler : MonoBehaviour
{
    [SerializeField]
    private GameObject miniPlayer;
    [SerializeField]
    private GameObject fpvPlayer;

    [SerializeField]
    bool fpvMod=false;


    public void onFpvToggle()
    {
        fpvMod = !fpvMod;
        if(miniPlayer!=null)
            miniPlayer.SetActive(!fpvMod);
        if (fpvPlayer != null)
            fpvPlayer.SetActive(fpvMod);
    }
}
