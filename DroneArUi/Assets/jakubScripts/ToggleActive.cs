// jakub komárek

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    // Start is called before the first frame update
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
