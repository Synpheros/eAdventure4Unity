using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ExitData {
	public string idTarget;
	public Vector2 destiny;
	public bool hasInfluenceArea;
	public int width;
	public int height;
	public Vector2 position;
	
	public bool rectangular;
	public int transitionTime;
	public int transitionType;
	
	public Condition conditions;
	public Effect effects;
	public Effect post_effect;
	
	public ExitData(XmlElement exit){
		this.idTarget = exit.GetAttribute("idTarget");
		this.destiny = new Vector2 (float.Parse (exit.GetAttribute("destinyX")), float.Parse (exit.GetAttribute("destinyY")));
		this.position = new Vector2 (float.Parse (exit.GetAttribute("x")), float.Parse (exit.GetAttribute("y")));
		this.hasInfluenceArea = (exit.GetAttribute("hasInfluenceArea") == "yes");
		this.height = int.Parse(exit.GetAttribute("height"));
		this.width = int.Parse(exit.GetAttribute("width"));
		this.rectangular = (exit.GetAttribute("rectangular") == "yes");
		this.transitionTime = int.Parse(exit.GetAttribute("transitionTime"));
		this.transitionType = int.Parse(exit.GetAttribute("transitionType"));
		
		XmlNode conditions = exit.SelectSingleNode("condition");
		if (conditions != null)
			this.conditions = ConditionFactory.Create (conditions);
		
		XmlNode effects = exit.SelectSingleNode("effect");
		this.effects = new Effect (effects);

		XmlNode post = exit.SelectSingleNode("post-effect");
		if(post!=null)
			this.post_effect = new Effect (post);
	}

	public bool check(){
		bool ret = true;
		if (conditions != null)
			ret = conditions.check ();
		
		return ret;
	}
}