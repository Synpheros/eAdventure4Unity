using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class OptionNodeOption {
	public EffectNode effect;
	public int child;

	public bool checkOption(){
		return effect.check ();
	}
}

public class OptionNode : ConversationNode {
	private List<OptionNodeOption> options;
	public List<OptionNodeOption> Options{
		get { return options;}
	}
	private int child;
	private Effect additional_effect;

	public bool canParseType(string type){
		return type == "option-node";
	}
	
	public void parse(XmlElement node){
		this.child = -2;
		this.options = new List<OptionNodeOption> ();

		if (node.LastChild.Name == "effect") {
			this.additional_effect = new Effect (node.LastChild);
			node.RemoveChild(node.LastChild);
		}

		EffectNode e = null;
		OptionNodeOption ono = null;
		foreach (XmlElement child in node.ChildNodes) {
			if (child.Name == "condition") {
				e.conditions = ConditionFactory.Create(child);
			}else if (child.Name == "child") {
				ono = new OptionNodeOption();
				ono.effect = e;
				ono.child = int.Parse(child.GetAttribute("nodeindex"));
				this.options.Add(ono);
			}else{
				e = new EffectNode();
				e.action = child.Name;
				if(e.action == "macro-ref")
					e.target = child.GetAttribute("id");
				if(e.action == "speak-player"){
					e.target = "player";
					e.aditional_info.Add ("text",child.InnerText);
				}else
					e.target = child.GetAttribute("flag");
			}
		}
	}
	
	public bool execute(){
		bool ret = true;
		if (this.child == -2)
			Game.Instance.showOptions (this);
		else {
			if(additional_effect != null)
				ret = additional_effect.execute();
			else
				ret = false;
		}
		return ret;
	}
	
	public int getChild(){
		return child;
	}

	public void clicked(int option){
		this.child = option;
	}

	public void resetChild(){
		this.child = -2;
	}
}
