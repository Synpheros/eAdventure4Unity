using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

public class GraphConversation : Secuence {
	private List<ConversationNode> nodes;

	public GraphConversation(XmlElement gc){
		this.nodes = new List<ConversationNode> ();
		foreach (XmlElement node in gc.ChildNodes) {
			nodes.Add (ConversationNodeFactory.Create(node));
		}
	}

	private ConversationNode current;
	public bool execute(){
		bool forcewait = false;

		if(current==null)
			current = nodes [0];

		while(!forcewait){
			if(current.execute()){
				forcewait = true;
				break;
			}else{
				if(current.getChild()==-1)
					break;
				else{
					int child = current.getChild();
					current.resetChild();
					current = nodes[child];
				}
			}
		}

		if(!forcewait)
			this.current = null;
		
		return forcewait;
	}
}