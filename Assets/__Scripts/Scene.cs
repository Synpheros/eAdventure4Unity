using UnityEngine;
using System.Collections;

public class Scene : MonoBehaviour{
	
	public GameObject Exit_Prefab;
	public GameObject ActiveArea_Prefab;
	public GameObject Character_Prefab;
	public GameObject Object_Prefab;
	public GameObject Atrezzo_Prefab;
	
	private SceneData sd;
	public SceneData sceneData{
		get { return sd; }
		set { sd = value; }
	}
	
	// Use this for initialization
	void Start () {
		this.gameObject.name = sd.getName ();;
		renderScene ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private int current_slide;
	private SlidesceneResource current_resource;

	public void renderScene(){
		switch (sceneData.getType ()) {
		case "scene": 
			RSceneData rsd = (RSceneData) sd;
			foreach(SceneResource sr in rsd.resources){
				if(sr.check()){
					Texture2D tmp = sr.assets["background"].Texture;
					this.transform.position = new Vector3 (40, 30, 20);
					this.transform.FindChild("Background").GetComponent<Renderer> ().material.mainTexture = tmp;
					break;
				}
			}
			
			Transform characters = this.transform.FindChild ("Characters");
			foreach(Transform child in characters) {GameObject.Destroy(child.gameObject);}
			
			foreach (ItemReference cr in rsd.characters) {
				if(cr.check()){
					GameObject ret = GameObject.Instantiate (Character_Prefab);
					Transform trans = ret.GetComponent<Transform> ();
					ret.GetComponent<Character>().context = cr;
					ret.GetComponent<Character>().charData = Game.Instance.getCharacter(cr.idTarget);
					trans.SetParent(characters);
				}
			}
			
			Transform objects = this.transform.FindChild ("Objects");
			foreach(Transform child in objects) {GameObject.Destroy(child.gameObject);}
			
			foreach (ItemReference ir in rsd.objects) {
				if(ir.check()){
					GameObject ret = GameObject.Instantiate (Object_Prefab);
					Transform trans = ret.GetComponent<Transform> ();
					ret.GetComponent<eObject>().context = ir;
					ret.GetComponent<eObject>().objData = Game.Instance.getObject(ir.idTarget);
					trans.SetParent(objects);
				}
			}
			
			Transform atrezzos = this.transform.FindChild ("Atrezzos");
			foreach(Transform child in atrezzos) {GameObject.Destroy(child.gameObject);}
			
			foreach (ItemReference ir in rsd.atrezzos) {
				if(ir.check()){
					GameObject ret = GameObject.Instantiate (Atrezzo_Prefab);
					Transform trans = ret.GetComponent<Transform> ();
					ret.GetComponent<eAtrezzo>().context = ir;
					ret.GetComponent<eAtrezzo>().atrData = Game.Instance.getAtrezzo(ir.idTarget);
					trans.SetParent(objects);
				}
			}
			
			Transform activeareas = this.transform.FindChild ("ActiveAreas");
			foreach(Transform child in activeareas) {GameObject.Destroy(child.gameObject);}

			foreach (ActiveAreaData ad in rsd.activeareas) {
				if(ad.check()){
					GameObject ret = GameObject.Instantiate (ActiveArea_Prefab);
					Transform trans = ret.GetComponent<Transform> ();
					ret.GetComponent<ActiveArea>().aaData = ad;
					trans.localScale = new Vector3(ad.width/10,ad.height/10,1);
					Vector2 tmppos = ad.position/10 + (new Vector2(trans.localScale.x,trans.localScale.y))/2;
					
					trans.localPosition = new Vector2(tmppos.x,60-tmppos.y);
					trans.SetParent(activeareas);
				}
			}

			Transform exits = this.transform.FindChild ("Exits");
			foreach(Transform child in exits) {GameObject.Destroy(child.gameObject);}
			
			foreach (ExitData ed in rsd.exits) {
				if(ed.check()){
					GameObject ret = GameObject.Instantiate (Exit_Prefab);
					Transform trans = ret.GetComponent<Transform> ();
					ret.GetComponent<Exit>().exitData = ed;
					trans.localScale = new Vector3(ed.width/10,ed.height/10,1);
					Vector2 tmppos = ed.position/10 + (new Vector2(trans.localScale.x,trans.localScale.y))/2;
					
					trans.localPosition = new Vector2(tmppos.x,60-tmppos.y);
					trans.SetParent(exits);
				}
			}
			break;
		case "slidescene": 
			SlidesceneData ssd = (SlidesceneData) sd;
			foreach(SlidesceneResource r in ssd.resources){
				if(r.check()){
					current_resource = r;
					this.transform.FindChild("Background").GetComponent<Renderer> ().material.mainTexture = r.assets["slides"].frames[0].Image;
					this.transform.position = new Vector3 (40, 30, 20);
					break;
				}
			}
			break;
		}
	}
	
	public bool Interacted(){
		bool forcewait = false;
		switch (sceneData.getType ()) {
		case "slidescene":
			if(current_slide+1 < current_resource.assets["slides"].frames.Count){
				current_slide++;
				this.transform.FindChild("Background").GetComponent<Renderer> ().material.mainTexture = current_resource.assets["slides"].frames[current_slide].Image;
				forcewait = true;
			}else{
				string next = ((SlidesceneData)sceneData).next;

				switch(next){
					case "new-scene": Game.Instance.renderScene (((SlidesceneData)sceneData).target); break;
					case "end-chapter": Game.Instance.nextChapter(); break;
				}
				
			}
			break;
		}

		return forcewait;
	}
}
