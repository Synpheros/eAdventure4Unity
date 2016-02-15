using UnityEngine;
using System.Collections;
using System.Xml;

public class ConditionFlag : Condition {
	private static string[] types = {
		"active",
		"inactive"
	};

	private string flag;
	private bool value;

	public bool check(){
		return Game.Instance.checkFlag(flag) == value;
	}

	public bool canParseType(string type){
		bool can = false;
		for (int i = 0; i<types.Length; i++) {
			if(type == types[i]){
				can = true;
				break;
			}
		}
		
		return can;
	}


	public void parse(XmlNode node){
		switch (node.Name) {
		case "active": this.value = true; break;
		case "inactive": this.value = false; break;
		}
		
		this.flag = node.Attributes ["flag"].Value;
	}
}
