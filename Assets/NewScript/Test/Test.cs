using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Data
{
    public string name;
}

public class Test : MonoBehaviour
{
    Data player = new Data() { name = "King" };

    string jsonData;

    private Text Text;

    private void Awake()
    {
        this.Text = GetComponent<Text>();

        jsonData = JsonUtility.ToJson(player);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) 
        {
            Data player = JsonUtility.FromJson<Data>(jsonData);

            Text.text = player.name;
        }
    }
}
