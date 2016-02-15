using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class SlidesceneResource {
	public string name;
	public Dictionary<string,eAnim> assets;
	public Condition conditions;
	
	public SlidesceneResource(XmlElement resource){
		this.assets = new Dictionary<string,eAnim> ();
		
		XmlNode conditions = resource.SelectSingleNode("condition");
		if(conditions != null)
			this.conditions = ConditionFactory.Create(conditions);
		
		foreach(XmlElement asset in resource.SelectNodes("asset")){
			string ruta = asset.GetAttribute("uri");
			string key = asset.GetAttribute("type");
			this.assets.Add(key,new eAnim(ruta));
		}
		this.name = resource.GetAttribute("name");

	}

	public bool check(){
		bool ret = true;
		if (this.conditions != null)
			ret = this.conditions.check ();
		
		return ret;
	}
}

public class SlidesceneData : SceneData{
	public string name;
	public string next;
	public string target;
	public Vector2 destiny;
	public int transition_time;
	public List<SlidesceneResource> resources;

	public string getType(){
		return "slidescene";
	}

	public string getName(){
		return name;
	}
	
	public SlidesceneData(XmlElement scene){
		this.name = scene.GetAttribute ("id");
		this.next = scene.GetAttribute ("next");
		this.target = scene.GetAttribute ("idTarget");
		if(scene.HasAttribute("transitionTime"))
			this.transition_time = int.Parse(scene.GetAttribute ("transitionTime"));
		if(scene.HasAttribute("destinyX"))
			this.destiny = new Vector2 (float.Parse (scene.GetAttribute ("destinyX")), float.Parse (scene.GetAttribute ("destinyY")));;
	
		this.resources = new List<SlidesceneResource> ();

		foreach (XmlElement resource in scene.SelectNodes("resources")) 
			resources.Add(new SlidesceneResource(resource));
	}
}
