using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class Action : Secuence{
	int keepDistance;
	string name;
	string type;
	public Dictionary<string,Texture2DHolder> resources;
	private Condition condition;
	public Effect effect;

	public string Name {
		get { return name;}
	}
	public string Type {
		get { return type;}
	}

	public Action(XmlElement action){
		this.resources = new Dictionary<string, Texture2DHolder> ();
		this.name = action.GetAttribute("name");
		this.type = action.Name;

		XmlNode conditions = action.SelectSingleNode("condition");
		if (conditions != null)
			this.condition = ConditionFactory.Create (conditions);

		XmlNode resources = action.SelectSingleNode("resources");
		if(resources != null)
		foreach(XmlElement resource in resources.ChildNodes){
			string ruta = resource.GetAttribute("uri");
			string key = resource.GetAttribute("type");
			Texture2DHolder img = new Texture2DHolder(ruta);
			
			this.resources.Add(key,img);
		}

		XmlNode effects = action.SelectSingleNode("effect");
		this.effect = new Effect (effects);
	}

	public bool execute(){
		return effect.execute ();
	}

	public bool check(){
		bool ret = true;
		if (this.condition != null)
			ret = this.condition.check ();
		
		return ret;
	}
}
