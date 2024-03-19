using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    public string description="";
    void Start()
    {
        descriptionTextMash.text = description;

        // spawn prefab

        // add liseners for grap and release
    }

    // Update is called once per frame
    void Update()
    {
        // if object graped - object handover to spawnOnMap

        // start 500s timer
    }

    // after 500s create new itemPrefab
}
