using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class SceneResource {
	public string name;
	public Dictionary<string,Texture2DHolder> assets;
	public Condition conditions;
	
	public SceneResource(){
		this.assets = new Dictionary<string,Texture2DHolder> ();
	}

	public bool check(){
		bool ret = true;
		if (this.conditions != null)
			ret = this.conditions.check ();
		
		return ret;
	}
}

public class RSceneData : SceneData {
	public string name;
	public Vector2 initial_position;
	public List<ExitData> exits;
	public List<ActiveAreaData> activeareas;
	public List<ItemReference> characters;
	public List<ItemReference> objects;
	public List<ItemReference> atrezzos;
	public List<SceneResource> resources;

	public string getType(){
		return "scene";
	}

	public string getName(){
		return name;
	}

	public RSceneData(XmlElement scene){
		this.name = scene.GetAttribute ("id");
		XmlAttributeCollection inipos = scene.SelectSingleNode ("default-initial-position").Attributes;
		this.initial_position = new Vector2 (float.Parse (inipos ["x"].Value), float.Parse (inipos ["y"].Value));
		this.resources = new List<SceneResource> ();
		this.exits = new List<ExitData> ();
		this.activeareas = new List<ActiveAreaData> ();
		this.characters = new List<ItemReference> ();
		this.objects = new List<ItemReference> ();
		this.atrezzos = new List<ItemReference> ();

		SceneResource sr;
		foreach (XmlElement resource in scene.SelectNodes("resources")) {
			sr = new SceneResource();
			
			XmlNode conditions = resource.SelectSingleNode("condition");
			if(conditions != null)
				sr.conditions = ConditionFactory.Create(conditions);

			foreach(XmlElement asset in resource.SelectNodes("asset")){
				string ruta = asset.GetAttribute("uri");
				string key = asset.GetAttribute("type");
				Texture2DHolder img = new Texture2DHolder(ruta);
				sr.assets.Add(key,img);
			}
			sr.name = resource.GetAttribute("name");
			
			resources.Add(sr);
		}

		XmlNode tmpnode = scene.SelectSingleNode ("exits");
		if(tmpnode != null){
			ExitData ed;
			foreach (XmlElement exit in tmpnode.ChildNodes) {
				ed = new ExitData(exit);
				this.exits.Add(ed);
			}
		}

		tmpnode = scene.SelectSingleNode ("active-areas");
		if(tmpnode != null){
			ActiveAreaData aad;
			foreach (XmlElement activearea in tmpnode.ChildNodes) {
				aad = new ActiveAreaData(activearea);
				this.activeareas.Add(aad);
			}
		}

		tmpnode = scene.SelectSingleNode ("characters");
		if(tmpnode != null){
			ItemReference cr;
			foreach (XmlElement character in tmpnode.ChildNodes) {
				cr = new ItemReference(character);
				this.characters.Add(cr);
			}
		}

		tmpnode = scene.SelectSingleNode ("objects");
		if(tmpnode != null){
			ItemReference ir;
			foreach (XmlElement ob in tmpnode.ChildNodes) {
				ir = new ItemReference(ob);
				this.objects.Add(ir);
			}
		}

		tmpnode = scene.SelectSingleNode ("atrezzo");
		if(tmpnode != null){
			ItemReference ir;
			foreach (XmlElement ob in tmpnode.ChildNodes) {
				ir = new ItemReference(ob);
				this.atrezzos.Add(ir);
			}
		}
	}
}
