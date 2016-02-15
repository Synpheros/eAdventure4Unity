using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class DialogNode : ConversationNode {
	private enum DNEffectState {NONE, BASIC, END_CONVERSATION, ADDITIONAL};

	private List<Effect> effects;
	public int child;

	public bool canParseType(string type){
		return type == "dialogue-node";
	}

	public void parse(XmlElement node){
		this.child = -2;
		this.effects = new List<Effect> ();

		do {
			XmlNode last = node.LastChild;
			if (last.Name == "end-conversation") {
				child = -1;
				if (last.HasChildNodes) {
					effects.Insert(0,new Effect (last.FirstChild));
				}
			} else if (last.Name == "effect") {
				effects.Insert(0,new Effect(last));
			}else{
				this.child = int.Parse (last.Attributes ["nodeindex"].Value);
			}
			node.RemoveChild (node.LastChild);
		} while(this.child==-2);

		effects.Insert(0,new Effect (node));

	}


	private int lastexecuted = 0;
	public bool execute(){

		bool forcewait = false;
		for (int i = lastexecuted; i<effects.Count; i++) {
			if(effects[i].execute()){
				lastexecuted = i;
				forcewait = true;
				break;
			}
		}
		
		if (!forcewait)
			lastexecuted = 0;

		return forcewait;
	}

	public int getChild(){
		return child;
	}

	public void resetChild(){
	}
}
