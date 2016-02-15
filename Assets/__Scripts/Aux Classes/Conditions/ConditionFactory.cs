using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class ConditionFactory {
	private static List<System.Type> types;

	public static Condition Create(XmlElement node){
		return Create (node.Clone());
	}

	public static Condition Create(XmlNode node){
		if (types == null) {
			types = System.AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => typeof(Condition).IsAssignableFrom (p)).ToList();
			types.Remove(typeof(Condition));
		}
		
		foreach (System.Type t in types) {
			Condition tmp = (Condition) System.Activator.CreateInstance(t);
			if(tmp.canParseType(node.Name)){
				tmp.parse(node);
				return tmp;
			}
		}
		
		return null;
	}
}

/*public static class ConditionFactory {

	public static Condition Create(XmlNode condition){
		return new ConditionList(condition);
	}

	public static Condition Create(XmlElement condition){
		Condition con;
		
		switch(condition.Name){
		case "global-state-ref":
			con = new ConditionGlobalStateRef(condition.GetAttribute("id"),condition.GetAttribute("value") == "true");
			break;
		case "active":
			con = new ConditionFlag(condition.GetAttribute("flag"),true);
			break;
		case "inactive":
			con = new ConditionFlag(condition.GetAttribute("flag"),false);
			break;
		case "either":
			con = new ConditionList(condition);
			break;
		case "neither":
			con = new ConditionList(condition);
			break;
		default:
			con = new ConditionList(condition);
			break;
		}
		
		return con;
	}
}*/
