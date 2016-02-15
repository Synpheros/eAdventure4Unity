using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class ConversationNodeFactory {
	private static List<System.Type> types;
	
	public static ConversationNode Create(XmlElement node){
		if (types == null) {
			types = System.AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => typeof(ConversationNode).IsAssignableFrom (p)).ToList();
			types.Remove(typeof(ConversationNode));
		}

		foreach (System.Type t in types) {
			ConversationNode tmp = (ConversationNode) System.Activator.CreateInstance(t);
			if(tmp.canParseType(node.Name)){
				tmp.parse(node);
				return tmp;
			}
		}

		return null;
	}
}
