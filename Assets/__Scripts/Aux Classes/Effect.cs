using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class EffectNode{
	public static string[] NodeTypes = {
		"activate",
		"deactivate",
		"speak-player",
		"speak-char",
		"macro-ref",
		"trigger-scene",
		"trigger-cutscene",
		"trigger-last-scene",
		"trigger-conversation",
		"random-effect",
		"set-value",
		"increment",
		"decrement"
	};

	public Rect Position = new Rect (0, 0, 25, 25);

	public string action;
	public string target;
	public Dictionary<string,object> aditional_info;
	public Condition conditions;

	public bool Collapsed = false;

	private bool runs_once = true;
	private int times_runed = 0;

	public EffectNode(){
		aditional_info = new Dictionary<string, object> ();
	}

	public EffectNode(XmlElement effect){
		aditional_info = new Dictionary<string, object> ();
		this.action = effect.Name;

		switch(this.action){
		case "activate":
			this.target = effect.GetAttribute("flag");
			break;
		case "deactivate":
			this.target = effect.GetAttribute("flag");
			break;
		case "speak-player": 
			this.target = "player";
			this.aditional_info.Add("text",effect.InnerText);
			break;
		case "speak-char":
			this.target = effect.GetAttribute("idTarget");
			this.aditional_info.Add("text",effect.InnerText);
			break;
		case "macro-ref": 
			this.target = effect.GetAttribute("id"); 
			break;
		case "trigger-scene": 
			this.target = effect.GetAttribute("idTarget"); 
			break;
		case "trigger-cutscene": 
			this.target = effect.GetAttribute("idTarget"); 
			break;
		case "trigger-last-scene":
			break;
		case "trigger-conversation": 
			this.target = effect.GetAttribute("idTarget"); 
			break;
		case "random-effect": 
			this.aditional_info.Add("probability",int.Parse(effect.GetAttribute("probability")));
			this.aditional_info.Add("first",new EffectNode((XmlElement) effect.ChildNodes[0]));
			this.aditional_info.Add("second",new EffectNode((XmlElement) effect.ChildNodes[1]));
			break;
		case "set-value": 
		case "increment": 
		case "decrement": 
			this.target = effect.GetAttribute("var");
			this.aditional_info.Add("value",int.Parse (effect.GetAttribute("value")));
			break;
		default:
			Debug.LogWarning("EFECTO NO SOPORTADO: " + this.action);
			break;
		}
	}
	
	public bool execute(){
		bool forcewait = false;
		if(!(runs_once && times_runed >0)){
			if (conditions == null || conditions.check ()) {
				switch(action){
				case "activate": Game.Instance.setFlag(target,true); break;
				case "deactivate": Game.Instance.setFlag(target,false); break;
				case "speak-player": Game.Instance.talk((string) aditional_info["text"],null); forcewait = true; break;
				case "speak-char": Game.Instance.talk((string) aditional_info["text"],target); forcewait = true; break;
				case "trigger-scene": runs_once = false; Game.Instance.renderScene(target); break;
				case "trigger-cutscene": 
					runs_once = false;
					if(times_runed > 0){
						forcewait = ((Scene) aditional_info["scene"]).Interacted() ;
					}else{
						aditional_info.Add("lastscene",Game.Instance.getCurrentScene());
						aditional_info.Add("scene",Game.Instance.renderScene(target).GetComponent<Scene>());
						forcewait = true; 
					}

					if(!forcewait && ((SlidesceneData) ((Scene) aditional_info["scene"]).sceneData).next =="go-back"){
						string last = (string) aditional_info["lastscene"];
						Game.Instance.renderScene(last);
					}

					break;
				case "trigger-last-scene": runs_once = false; Game.Instance.renderScene(Game.Instance.end_scene); break;
				case "trigger-conversation": runs_once = false; forcewait = Game.Instance.getConversation(target).execute(); break;
				case "random-effect": 
					runs_once = false;
					if(times_runed==0){
						int pro = (int)aditional_info["probability"], now = Random.Range(0,100);
						if(aditional_info.ContainsKey("current"))
							aditional_info.Remove("current");

						if(pro <= now)
							aditional_info.Add("current", "first");
						else
							aditional_info.Add("current", "second");

						forcewait = ((EffectNode) aditional_info[((string) aditional_info["current"])]).execute();
					}else
						forcewait = ((EffectNode) aditional_info[((string) aditional_info["current"])]).execute();

					break;
				case "set-value": Game.Instance.setVariable(target,(int) aditional_info["value"]); break;
				case "increment": Game.Instance.setVariable(target, Game.Instance.getVariable(target) + ((int) aditional_info["value"])); break;
				case "decrement": Game.Instance.setVariable(target, Game.Instance.getVariable(target) - ((int) aditional_info["value"])); break;
				case "macro-ref": runs_once = false;
					Effect macro = Game.Instance.getMacro(target);
					forcewait = macro.execute();
					break;
				}
			}
		}

		if (!forcewait)
			times_runed = 0;
		else
			times_runed++;
		
		return forcewait;
	}

	public bool check(){
		return conditions.check ();
	}
}

public class Effect : Secuence{
	public List<EffectNode> effects;

	private string documentation;

	public Effect(XmlNode effects){
		this.effects = new List<EffectNode> ();

		if (effects != null && effects.HasChildNodes) {

			if (effects.FirstChild.Name == "documentation") {
				this.documentation = effects.FirstChild.InnerText;
				effects.RemoveChild(effects.FirstChild);
			}

			EffectNode e = null;
			List<Condition> conditions = new List<Condition>();
			foreach (XmlElement effect in effects.ChildNodes) {
				if (effect.Name == "condition") {
					conditions.Add(ConditionFactory.Create(effect));
				}else{
					this.effects.Add(new EffectNode(effect));
					this.effects[this.effects.Count-1].Position = new Rect (0, this.effects.Count, 25, 25);
				}
			}

			int i = 0;
			foreach(Condition c in conditions){
				this.effects[i].conditions = c;
				i++;
			}

		}
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
}