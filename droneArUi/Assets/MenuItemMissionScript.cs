// author jakub komárek
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using static MissionSelector;


public class MenuItemMissionScript : MonoBehaviour
{
    // Start is called before the first frame update
    public MissionSelector.menuItemTDO menuItemTdo;

    [SerializeField]
    TextMeshPro label;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(menuItemTdo != null)
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
            // Delete the file
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
        Debug.Log("File load: " + menuItemTdo.path);
    }
}
