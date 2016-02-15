using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class eObject : MonoBehaviour {
	
	private ObjectData od;
	public ObjectData objData{
		get { return od; }
		set { od = value; }
	}
	public ItemReference context;

	public int DifferentActions{
		get { 
			int tmp = 0;
			foreach(Action a in od.Actions){
				if(a.check()){
					tmp++;
				}
			}
			return tmp;
		}
	}
	
	// Use this for initialization
	void Start () {
		Texture2D tmp = od.getResource("image");
		this.GetComponent<Renderer> ().material.mainTexture = tmp;
		this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.scale;
		
		Vector2 tmppos = context.position / 10 + (new Vector2(0,-transform.localScale.y))/2;
		transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.layer);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseEnter(){
		if(od.getResource("imageover")!=null)
			GetComponent<Renderer> ().material.mainTexture = od.getResource("imageover");
	}
	
	void OnMouseExit() {
		GetComponent<Renderer> ().material.mainTexture = od.getResource("image");
	}
}
