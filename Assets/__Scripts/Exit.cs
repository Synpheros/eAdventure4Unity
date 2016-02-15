using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {

	private ExitData ed;
	public ExitData exitData{
		get { return ed; }
		set { ed = value; }
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void exit(){
		Game.Instance.hideMenu ();
		Game.Instance.Execute (ed.effects);
		Game.Instance.renderScene(ed.idTarget);

		if (ed.post_effect != null)
			Game.Instance.Execute(ed.post_effect);
	}

	void OnMouseEnter(){
		GetComponent<Renderer> ().material.color = new Color (1f, 0f, 0f, 0.2f);
	}
	
	void OnMouseExit() {
		GetComponent<Renderer> ().material.color = new Color (1f, 0f, 0f, 0f);
	}
}
