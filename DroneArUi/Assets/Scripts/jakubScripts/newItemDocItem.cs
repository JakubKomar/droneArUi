/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// logika pro spawner prefab�, kter� se n�sledn� zan�i do mapy
/// </summary>
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class newItemDocItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public SpawnOnMap spawnOnMap;
    [SerializeField]
    public GameObject itemPrefab;
    [SerializeField]
    public TextMeshProUGUI descriptionTextMash;
    [SerializeField]
    public MapObject.ObjType type;

    private GameObject movebleGameObject = null;

    [SerializeField]
    public string description = "";
    void Start()
    {
        // nastaven� popisku
        descriptionTextMash.text = description;

        // vytvo� um�stiteln� prefab
        if (itemPrefab != null)
        {
            movebleGameObject = Instantiate(itemPrefab);
            movebleGameObject.transform.parent = this.transform;
            movebleGameObject.transform.localScale = Vector3.one;
            movebleGameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError("spawner: " + description + " dont have prefab");
            return;
        }

        ObjectManipulator manipulator = movebleGameObject.GetComponent<ObjectManipulator>();

        if (manipulator != null)
        {
            manipulator.OnManipulationStarted.AddListener(onManipulationStart);
            manipulator.OnManipulationEnded.AddListener(onManipulationEnd);
        }
        else
        {
            Debug.LogError("prefab in spawner: " + description + " dont have MRTK manipulator script");
            return;
        }
    }

    private void onManipulationStart(ManipulationEventData eventData)
    {
        // nepot�ebuju
    }


    private void onManipulationEnd(ManipulationEventData eventData)
    {

        if (spawnOnMap == null)
        {
            Debug.LogError("dock:" + description + " dont have abstract map");
            return;
        }
        // vytvo� objekt
        spawnOnMap.createNewObject(type, movebleGameObject);

        // p�esun objekt sp�tky do doku
        movebleGameObject.transform.localScale = Vector3.one;
        movebleGameObject.transform.localPosition = Vector3.zero;
    }
}
