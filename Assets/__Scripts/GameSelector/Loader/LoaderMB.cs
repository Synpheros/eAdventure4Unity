using UnityEngine;
using System.Collections;

public class LoaderMB : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log (LoaderController.Instance.gamePath);
		//ResourceManager.Instance.extractFile(LoaderController.Instance.gamePath);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
