using UnityEngine;
using System.Collections;
using System.Xml;

public class ConditionGlobalStateRef : Condition {

	private string global_state;
	private bool value;
	
	public bool check(){
		return Game.Instance.checkGlobalState(global_state) == value;
	}

	public bool canParseType(string type){
		return type == "global-state-ref";
	}
	
	public void parse(XmlNode node){
		this.global_state = node.Attributes["id"].Value;
		this.value = node.Attributes["value"].Value == "true";
	}

}
