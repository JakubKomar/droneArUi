using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Microsoft.MixedReality.Toolkit.Utilities;

public class MissionSelector : MonoBehaviour
{
    // Start is called before the first frame update
    string pathToDir = "";

    List<string> jsonFiles=new List<string>();
    List<string> csvFiles = new List<string>();
    List<GameObject> menuItems = new List<GameObject>();

    [SerializeField]
    GridObjectCollection contentConteiner;

    [SerializeField]
    GameObject _menuItemPrefab;

    [SerializeField]
    int listLen = 6;

    private int curentPageNum=0;
    private int maxPageNum = 0;
    void Start()
    {
        pathToDir = Path.Combine(Application.persistentDataPath, "misions/");
        Debug.Log(pathToDir);
        refreshMissionList();

        if (contentConteiner != null) {
            Debug.LogError("Mission selector: ContentGridObjectCollection not found err");
        }
    }

    public void refreshMissionList()
    {
        jsonFiles.Clear();
        csvFiles.Clear();
        foreach (var obj in menuItems)
        {
            Destroy(obj);
        }

        if (!Directory.Exists(pathToDir))
        {
            Directory.CreateDirectory(pathToDir);
            Debug.Log("Created directory: " + pathToDir);
        }
       
        string[] files = Directory.GetFiles(pathToDir);

        foreach (string file in files)
        {
            // Zkontrolujte, zda soubor konèí pøíponou ".json"
            if (Path.GetExtension(file).Equals(".json"))
            {
                // Pøidejte cestu k JSON souboru do pole
                jsonFiles.Add(file);
            }
        }
        foreach (string file in files)
        {
            // Zkontrolujte, zda soubor konèí pøíponou ".json"
            if (Path.GetExtension(file).Equals(".csv"))
            {
                // Pøidejte cestu k JSON souboru do pole
                csvFiles.Add(file);

            }
        }
        maxPageNum = jsonFiles.Count / listLen;

        pageShow(0);
    }

    void pageShow(int page) { 

        foreach (var obj in menuItems)
        {
            Destroy(obj);
        }

        if(page < 0)
        {
            page = 0;
        }
        if (page > maxPageNum) { 
            page = maxPageNum;
        }
        curentPageNum = page;


        int beginIndex = page * listLen;
        for(int i = beginIndex; i < jsonFiles.Count; i++) {
            GameObject gameObject = Instantiate(_menuItemPrefab);
            gameObject.transform.parent = contentConteiner.transform;
            gameObject.transform.localScale = Vector3.one;
            menuItems.Add(gameObject);
        }

        contentConteiner.UpdateCollection();
    }

    public  void onUp()
    {
        pageShow(curentPageNum + 1);
    }
    public void onDown()
    {
        pageShow(curentPageNum -1);
    }

    public void onNew()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
