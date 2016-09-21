using UnityEngine;
using System.Collections;

public class ScreenCapture : MonoBehaviour {

    Texture2D renderedTexture;

    void Awake()
    {
        renderedTexture = new Texture2D(Screen.width, Screen.height);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnPostRender () {
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        renderedTexture.Apply();
	}

    public void capture(string filename)
    {

    }
}
