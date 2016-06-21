using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChangerMB : MonoBehaviour {
	
	void Start ()
    {
        VideoConverter converter = new VideoConverter();
        converter.Convert("C:/Users/Synpheros/Desktop/ffmpeg/MAQ29909.mpg");
    }

	void Update () {
		
	}

	public void ChangeScene(string name){
		SceneManager.LoadScene (name);
	}
}
