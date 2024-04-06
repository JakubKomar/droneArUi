/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// Skript pro každou položku ve selektoru misí
/// </summary>
/// 
using TMPro;
using UnityEngine;
using System.IO;
public class MenuItemMissionScript : MonoBehaviour
{

    public MissionSelector.menuItemTDO menuItemTdo;

    [SerializeField]
    TextMeshPro label;
    void Start()
    {

    }

    void Update()
    {
        if (menuItemTdo != null)
        {
            label.text = menuItemTdo.name;
        }
    }
    public void onDeletePressed()
    {
        if (menuItemTdo == null)
            return;
        if (menuItemTdo == null)
            return;

        if (File.Exists(menuItemTdo.path))
        {
            File.Delete(menuItemTdo.path);
            Debug.Log("File deleted: " + menuItemTdo.path);
        }
        else
        {
            Debug.LogError("File does not exist: " + menuItemTdo.path);
        }
        menuItemTdo.missionSelector.refreshMissionList();
    }

    public void onLoadPressed()
    {
        if (menuItemTdo.missionSelector == null)
        {
            Debug.LogError("menuItemTdo.missionSelector == null");
            return;
        }

        menuItemTdo.missionSelector.onLoaded(menuItemTdo);

    }
}
