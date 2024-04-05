/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Toggle vlastnost active u zvoleného objektu
/// </summary>
/// 

using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    [SerializeField]
    private GameObject target = null;

    public void onToggleActive()
    {
        target.SetActive(!target.activeSelf);
    }

    public void setActive(bool active = true)
    {
        target.SetActive(active);
    }
}
