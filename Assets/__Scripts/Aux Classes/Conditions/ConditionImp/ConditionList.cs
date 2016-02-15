using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ConditionList : Condition {

	public enum ConditionListType {ALL, EITHER};
	private static string[] types = {
		"condition",
		"either",
		"global-state"
	};

	private ConditionListType type;
	private List<Condition> condition_list;

	public ConditionList(){
	}

	private void set(XmlNodeList conditionlist){
		this.condition_list = new List<Condition> ();
		foreach(XmlElement condition in conditionlist){
			condition_list.Add(ConditionFactory.Create(condition));
        }
	}

	public bool check(){
		bool result = true;
		switch (type) {
		case ConditionListType.ALL:
			foreach (Condition c in condition_list){
				result &= c.check();
				if(result == false)
					break;
			}
			break;
		case ConditionListType.EITHER:
			result = false;
			foreach (Condition c in condition_list){
				result = c.check();
				if(result == true)
                    break;
            }
			break;
		}
		return result;
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
		case "condition": this.type = ConditionListType.ALL; break;
		case "global-state": this.type = ConditionListType.ALL; break;
		case "either": this.type = ConditionListType.EITHER; break;
		}
		
		this.set (node.ChildNodes);
	}
}
