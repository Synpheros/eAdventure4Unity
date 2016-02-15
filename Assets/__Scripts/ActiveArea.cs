using UnityEngine;
using System.Collections;

public class ActiveArea : MonoBehaviour {

	private ActiveAreaData aad;
	public ActiveAreaData aaData{
		get { return aad; }
		set { aad = value; }
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseEnter(){
		GetComponent<Renderer> ().material.color = new Color (0f, 1f, 0f, 0.2f);
	}
	
	void OnMouseExit() {
		GetComponent<Renderer> ().material.color = new Color (1f, 0f, 0f, 0f);
	}
}
