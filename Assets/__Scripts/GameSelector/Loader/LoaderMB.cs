using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading;

public class LoaderMB : MonoBehaviour {

	Slider slider;
	Thread unzipthread;

	// Use this for initialization
	void Start () {
		//Debug.Log (LoaderController.Instance.gamePath);
		slider = this.GetComponent<Slider> ();
		unzipthread = new Thread (unZip);
		unzipthread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		slider.value = ZipUtil.Progress;

		if (ResourceManager.Instance.extracted) {
			unzipthread.Abort ();
			SceneManager.LoadScene ("_MenuScene");
		}
	}

		
	void unZip(){
		ResourceManager.Instance.extractFile(LoaderController.Instance.gamePath);
	}
}
