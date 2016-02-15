using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class CharacterResource{
	public string name;
	public Condition condition;
	public Dictionary<string,eAnim> animations;

	public CharacterResource(XmlElement node){
		this.name = node.GetAttribute("name");
		XmlNode conditions = node.SelectSingleNode("condition");
		if(conditions != null)
			this.condition = ConditionFactory.Create(conditions);

		this.animations = new Dictionary<string,eAnim> ();

		foreach(XmlElement animation in node.SelectNodes("asset")){
			string ruta = animation.GetAttribute("uri");
			string key = animation.GetAttribute("type");
			if(ruta != "assets/special/EmptyAnimation")
				animations.Add(key,new eAnim(ruta));
		}
	}

	public bool check(){
		bool ret = true;
		if (this.condition != null)
			ret = this.condition.check ();

		return ret;
	}
}

public class CharacterData {

	string id;
	List<CharacterResource> resources;
	List<Action> actions;

	public string ID{
		get { return id; }
	}

	public List<CharacterResource> Resources{
		get { return resources; }
	}

	public List<Action> Actions{
		get { return actions; }
	}

	public CharacterResource getResource(string res){
		CharacterResource ret = null;
		foreach (CharacterResource cr in resources) {
			if(cr.name == res){
				ret = cr;
				break;
			}
		}
		return ret;
	}

	public CharacterData (XmlElement character){
		this.id = character.GetAttribute ("id");
		this.resources = new List<CharacterResource> ();

		foreach (XmlElement node in character.SelectNodes("resources"))
			resources.Add(new CharacterResource(node));

		this.actions = new List<Action> ();
		XmlNode actions = character.SelectSingleNode("actions");
		if(actions != null)
		foreach(XmlElement action in actions.ChildNodes){
			this.actions.Add(new Action(action));
		}
	}
}
