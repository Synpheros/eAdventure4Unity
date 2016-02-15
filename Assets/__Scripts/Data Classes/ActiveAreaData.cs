using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ActiveAreaData {
	public string id;
	public bool hasInfluenceArea;
	public int width;
	public int height;
	public Vector2 position;
	
	public bool rectangular;
	public int transitionTime;
	public int transitionType;
	
	public Condition conditions;
	public List<Action> actions;
	
	public ActiveAreaData(XmlElement activearea){
		this.id = activearea.GetAttribute ("id");
		this.position = new Vector2 (float.Parse (activearea.GetAttribute("x")), float.Parse (activearea.GetAttribute("y")));
		this.hasInfluenceArea = (activearea.GetAttribute("hasInfluenceArea") == "yes");
		this.height = int.Parse(activearea.GetAttribute("height"));
		this.width = int.Parse(activearea.GetAttribute("width"));
		this.rectangular = (activearea.GetAttribute("rectangular") == "yes");
		
		XmlNode conditions = activearea.SelectSingleNode("condition");
		if (conditions != null)
			this.conditions = ConditionFactory.Create (conditions);
		
		this.actions = new List<Action> ();
		XmlNode actions = activearea.SelectSingleNode("actions");
		if(actions != null)
		foreach(XmlElement action in actions.ChildNodes){
			this.actions.Add(new Action(action));
		}
	}
	
	public bool check(){
		bool ret = true;
		if (conditions != null)
			ret = conditions.check ();
		
		return ret;
	}
}