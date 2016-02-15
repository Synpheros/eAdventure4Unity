using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class AtrezzoResource{
	public string name;
	public Condition condition;
	public Dictionary<string,Texture2DHolder> resources;

	public AtrezzoResource(XmlElement node){
		this.resources = new Dictionary<string,Texture2DHolder> ();

		XmlNode condition = node.SelectSingleNode("condition");
		if(condition != null){
			this.condition = ConditionFactory.Create (condition);
			node.RemoveChild (condition);
		}

		foreach(XmlElement resource in node.ChildNodes){
			string ruta = resource.GetAttribute("uri");
			string key = resource.GetAttribute("type");
			Texture2DHolder img = new Texture2DHolder(ruta);
			
			this.resources.Add(key,img);
		}
	}

	public bool check(){
		bool ret = true;
		if (this.condition != null)
			ret = this.condition.check ();
		
		return ret;
	}
}

public class AtrezzoData {
	
	string id;
	List<AtrezzoResource> resources;
	
	public string ID{
		get { return id; }
	}
	
	public List<AtrezzoResource> atrezzoResources{
		get { return resources; }
	}

	public AtrezzoData (XmlElement ob){
		this.id = ob.GetAttribute ("id");
		this.resources = new List<AtrezzoResource> ();
		
		XmlNodeList resources = ob.SelectNodes("resources");
		if(resources != null)
		foreach(XmlElement resource in resources){
			this.resources.Add(new AtrezzoResource(resource));
		}
	}
}
