using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ItemReference{
	public bool hasInfluenceArea;
	public string idTarget;
	public int layer;
	public float scale;
	public Vector2 position;
	
	public Condition conditions;
	
	public ItemReference(XmlElement cr){
		this.idTarget = cr.GetAttribute("idTarget");
		this.position = new Vector2 (float.Parse (cr.GetAttribute("x")), float.Parse (cr.GetAttribute("y")));
		this.hasInfluenceArea = (cr.GetAttribute("hasInfluenceArea") == "yes");
		this.layer = int.Parse(cr.GetAttribute("layer"));
		this.scale = float.Parse(cr.GetAttribute("scale"));
		
		XmlNode conditions = cr.SelectSingleNode("condition");
		if (conditions != null)
			this.conditions = ConditionFactory.Create (conditions);
	}

	public bool check(){
		bool ret = true;
		if (conditions != null)
			ret = conditions.check ();

		return ret;
	}
}