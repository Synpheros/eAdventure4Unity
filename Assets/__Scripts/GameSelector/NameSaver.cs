using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameSaver : MonoBehaviour {
    public Text t;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SaveName()
    {
        PlayerPrefs.SetString("username", t.text);
    }
}
