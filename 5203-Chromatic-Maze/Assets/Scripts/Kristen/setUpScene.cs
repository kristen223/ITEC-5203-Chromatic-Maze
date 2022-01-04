using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class setUpScene : MonoBehaviour
{

    TextMeshProUGUI w;
    TextMeshProUGUI h;

    Slider ws;
    Slider hs;

    public static int width;
    public static int height;

    // Start is called before the first frame update
    void Start()
    {
        width = 1;
        height = 1;
        w = GameObject.Find("wtext1").GetComponent<TextMeshProUGUI>();
        h = GameObject.Find("htext1").GetComponent<TextMeshProUGUI>();

        ws = GameObject.Find("width").GetComponent<Slider>();
        hs = GameObject.Find("height").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        w.text = ws.value.ToString();
        h.text = hs.value.ToString();

        width = (int)ws.value;
        height = (int)hs.value;

    }

    void LoadLevel()
    {
        DontDestroyOnLoad(gameObject);
    }

}
