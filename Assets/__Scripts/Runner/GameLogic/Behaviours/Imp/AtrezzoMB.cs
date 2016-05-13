using UnityEngine;
using System.Collections;

public class AtrezzoMB : MonoBehaviour {

	private Atrezzo ad;
    public Atrezzo atrData{
		get { return ad; }
		set { ad = value; }
	}
	
    public ElementReference context;
	
	// Use this for initialization
	void Start () {
        foreach (ResourcesUni ar in ad.getResources()) {
            if (ConditionChecker.check(ar.getConditions ())) {
                Texture2DHolder th = new Texture2DHolder(ar.getAssetPath(Atrezzo.RESOURCE_TYPE_IMAGE));
                Texture2D tmp = th.Texture;
				this.GetComponent<Renderer> ().material.mainTexture = tmp;
                this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.getScale();
				
                Vector2 tmppos = new Vector2(context.getX(),context.getY()) / 10 + (new Vector2(0,-transform.localScale.y))/2;
                transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.getLayer());
				break;
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
