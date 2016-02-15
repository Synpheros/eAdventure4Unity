using UnityEngine;
using System.Collections;
using System.Xml;

public class ConditionCompare : Condition {
	private enum ComparisionType { GREATER_THAN, GREATER_EQUALS_THAN, EQUALS, LESS_EQUALS_THAN, LESS_THAN, NOT_EQUALS};
	private static string[] types = {
		"greater-than",
		"greater-equals-than",
		"equals",
		"less-equals-than",
		"less-than",
		"not-equals"
	};

	private string variable;
	private ComparisionType type;
	private int value;
	
	public bool check(){
		bool ret = false;

		if (Game.Instance.getVariable (variable) < 0)
			return false;

		switch (type) {
		case ComparisionType.GREATER_THAN: ret = (value > Game.Instance.getVariable(variable)); break;
		case ComparisionType.GREATER_EQUALS_THAN: ret = (value >= Game.Instance.getVariable(variable)); break;
		case ComparisionType.EQUALS: ret = (value == Game.Instance.getVariable(variable)); break;
		case ComparisionType.LESS_EQUALS_THAN: ret = (value <= Game.Instance.getVariable(variable)); break;
		case ComparisionType.LESS_THAN: ret = (value < Game.Instance.getVariable(variable)); break;
		case ComparisionType.NOT_EQUALS: ret = (value != Game.Instance.getVariable(variable)); break;
		}

		return ret;
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
		case "greater-than": this.type = ComparisionType.GREATER_THAN; break;
		case "greater-equals-than": this.type = ComparisionType.GREATER_EQUALS_THAN; break;
		case "equals": this.type = ComparisionType.EQUALS; break;
		case "less-equals-than": this.type = ComparisionType.LESS_EQUALS_THAN; break;
		case "less-than": this.type = ComparisionType.LESS_THAN; break;
		case "not-equals": this.type = ComparisionType.NOT_EQUALS; break;
		}

		this.variable = node.Attributes ["var"].Value;
		this.value = int.Parse(node.Attributes ["value"].Value);
	}

}