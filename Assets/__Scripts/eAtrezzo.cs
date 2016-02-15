using UnityEngine;
using System.Collections;

public class eAtrezzo : MonoBehaviour {

	private AtrezzoData ad;
	public AtrezzoData atrData{
		get { return ad; }
		set { ad = value; }
	}
	
	public ItemReference context;
	
	// Use this for initialization
	void Start () {
		foreach (AtrezzoResource ar in ad.atrezzoResources) {
			if (ar.check ()) {
				Texture2D tmp = ar.resources["image"].Texture;
				this.GetComponent<Renderer> ().material.mainTexture = tmp;
				this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.scale;
				
				Vector2 tmppos = context.position / 10 + (new Vector2(0,-transform.localScale.y))/2;
				transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.layer);
				break;
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
