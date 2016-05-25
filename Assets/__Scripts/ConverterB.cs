using UnityEngine;
using System.Collections;

public class ConverterB : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//ResourceManager.Instance.convertVideo ("C:/Games/", "Electrodos.avi");
		ResourceManager.Instance.extractFile("C:/","TravelGame.jar");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
