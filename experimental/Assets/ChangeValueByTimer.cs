using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChangeValueByTimer : MonoBehaviour
{
    public GameObject textField;
    List<GameObject>[] textsGameObjects;

    TextMeshProUGUI textmeshpro_ugui;
    private float timer = 0.0f;
    private float timerLimit = 1f;
    // Start is called before the first frame update
    void Start()
    {
        textmeshpro_ugui = textField.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timerLimit)
        {
            textmeshpro_ugui.text = Random.Range(0, 100).ToString();
            timer = 0;
        }
    }
}
