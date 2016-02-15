using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class ObjectData {
	
	string id;
	Dictionary<string,Texture2DHolder> resources;
	List<Action> actions;
	
	public string ID{
		get { return id; }
	}

	public List<Action> Actions{
		get { return actions; }
	}
	
	public ObjectData (XmlElement ob){
		this.id = ob.GetAttribute ("id");
		this.resources = new Dictionary<string,Texture2DHolder> ();

		XmlNode resources = ob.SelectSingleNode("resources");
		if(resources != null)
		foreach(XmlElement resource in resources.ChildNodes){
			string ruta = resource.GetAttribute("uri");
			string key = resource.GetAttribute("type");
			Texture2DHolder img = new Texture2DHolder(ruta);
			
			this.resources.Add(key,img);
		}
		
		this.actions = new List<Action> ();
		XmlNode actions = ob.SelectSingleNode("actions");
		if(actions != null)
		foreach(XmlElement action in actions.ChildNodes){
			this.actions.Add(new Action(action));
		}
	}

	public Texture2D getResource(string name){
		Texture2D ret = null;
		if(resources.ContainsKey(name)){
			ret = resources [name].Texture;
		}
		return ret;
	}
}
