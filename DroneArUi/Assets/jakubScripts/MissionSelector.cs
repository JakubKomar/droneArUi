// author: jakub komárek

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Microsoft.MixedReality.Toolkit.Utilities;
using static MissionSelector;

public class MissionSelector : MonoBehaviour
{
    // Start is called before the first frame update
    string pathToDir = "";

    List<menuItemTDO> saveFiles=new List<menuItemTDO>();

    List<GameObject> menuItems = new List<GameObject>();

    MapData mapdata = null;

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

        if (contentConteiner == null)
        {
            Debug.LogError("Mission selector: ContentGridObjectCollection not found err");
        }
        mapdata = FindObjectOfType<MapData>();
        if (mapdata == null)
        {
            Debug.LogError("Mission selector: MapData mapdata  not found err");
        }
        mapdata.sevedFile.AddListener(refreshMissionList);
        onClose();
    }
 

    public void refreshMissionList()
    {
        saveFiles.Clear();

        foreach (var obj in menuItems)
        {
            Destroy(obj);
        }
        menuItems.Clear();

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
                saveFiles.Add( new menuItemTDO(Path.GetFileName(file) ,file, ".json",this));
            }
        }
        foreach (string file in files)
        {
            // Zkontrolujte, zda soubor konèí pøíponou ".json"
            if (Path.GetExtension(file).Equals(".csv"))
            {
                // Pøidejte cestu k JSON souboru do pole
                saveFiles.Add(new menuItemTDO(Path.GetFileName(file), file, ".csv",this));

            }
        }
        maxPageNum = saveFiles.Count / listLen;
        if(saveFiles.Count% listLen != 0)
        {
            maxPageNum++;
        }

        pageShow(curentPageNum);
    }

    void pageShow(int page) { 

        foreach (var obj in menuItems)
        {
            Destroy(obj);
        }
        menuItems.Clear();

        if (page >= maxPageNum) { 
            page = maxPageNum-1;
        }
        if (page < 0)
        {
            page = 0;
        }
        curentPageNum = page;


        int beginIndex = page * listLen;

        int counter = 0;
        for(int i = beginIndex; i < saveFiles.Count; i++) {
            if (counter >= listLen)
                break;

            GameObject gameObject = Instantiate(_menuItemPrefab);
            MenuItemMissionScript scriptComponent = gameObject.GetComponent<MenuItemMissionScript>();
            if(scriptComponent != null)
            {
                scriptComponent.menuItemTdo = saveFiles[i];
            }

            gameObject.transform.parent = contentConteiner.transform;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localRotation = new Quaternion(0,0,0,1);
            gameObject.transform.localPosition = new Vector3(0, -0.01f, 0);
            menuItems.Add(gameObject);
            counter++;
        }
    }

    public void onLoaded(menuItemTDO menuItemTDO)
    {
        if (menuItemTDO == null)
        {
            Debug.LogError("menuItemTDO == null");
            return;
        }
        if (mapdata == null)
        {
            Debug.LogError("mapdata == null");
            return;
        }


        if (menuItemTDO.format == ".json")
        {
            mapdata.loadMision(menuItemTDO.path);
        }
        else if (menuItemTDO.format == ".csv") {
            mapdata.loadCsvMision(menuItemTDO.path,menuItemTDO.name);
        }
        else {
            Debug.LogError(menuItemTDO.format);
        }
        onClose();

    }

    public void onOpen() {
        refreshMissionList();
        this.gameObject.SetActive(true);
    }

    public void onClose() {
        this.gameObject.SetActive(false);
    }
    public  void onUp()
    {
        pageShow(curentPageNum - 1);
    }
    public void onDown()
    {
        pageShow(curentPageNum +1);
    }

    public void onNew()
    {
        mapdata.newMission();
        onClose();
    }

    // Update is called once per frame
    void Update()
    {
        contentConteiner.UpdateCollection();
    }

    public class menuItemTDO
    {
        public string name;
        public string path;
        public string format;
        public MissionSelector missionSelector=null;
        public menuItemTDO() { }
        public menuItemTDO(string name, string path, string format, MissionSelector missionSelector=null)
        {
            this.name = name;
            this.path = path;
            this.format = format;
            this.missionSelector = missionSelector;
        }
    }

}
