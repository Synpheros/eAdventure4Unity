using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Threading;

public enum guiState {
	GAME_SELECTION, LOADING_GAME,NOTHING,TALK_PLAYER,TALK_CHARACTER,OPTIONS_MENU,ANSWERS_MENU
}

public enum loaderState {
	NOT_LOADING, LOADING, FINISHED_LOADING
}

public class Game : MonoBehaviour {
	Descriptor descriptor;
	int currentChapter = -1;

	private guiState guistate;
	private string guitext;
	private string guitalker;
	private GameObject guitalkerObject;
	private Dictionary<string,Action> guiactions;
	private OptionNode guioptions;
	private Secuence secuencepending;
	private GUISkin style;
	private string playerName = "Jugador";

	private static Game instance;

	public static Game Instance {
		get { return instance; }
		set { instance = value; }
	}


	public XmlDocument xmld;
	public GameObject Character_Prefab;
	public GameObject Scene_Prefab;


	private Dictionary<string,int> totals;
	private Thread gameloader;
	private loaderState loader_state;

	private Dictionary<string,Condition> global_states;
	private Dictionary<string,GraphConversation> graph_conversations;
	private Dictionary<string,Effect> macros;
	private List<Effect> adaptations;
	private List<CharacterData> characters;
	private List<ObjectData> objects;
	private List<AtrezzoData> atrezzos;
	private List<SceneData> scenes;

	private string start_scene;
	public string end_scene;
	private GameObject current_scene;


	private Dictionary<string,bool> flags;
	private Dictionary<string,int> variables;

	public bool checkFlag(string flag){
		bool ret = false;
		if(flags.ContainsKey(flag)){
			ret = flags [flag];
		}
		return ret;
	}

	public void setFlag(string name, bool state){
		if(flags.ContainsKey(name)){
			flags [name] = state;
		}else{
			flags.Add (name, state);
		}

		//Debug.Log ("Flag '" + name + " puesta a " + state);
		this.reRenderScene ();
	}

	public int getVariable(string var){
		int ret = -1;
		if(variables.ContainsKey(var)){
			ret = variables [var];
		}
		return ret;
	}
	
	public void setVariable(string name, int value){
		if(variables.ContainsKey(name)){
			variables [name] = value;
		}else{
			variables.Add (name, value);
		}
		
		Debug.Log ("Valor '" + name + " puesto a " + value);
		this.reRenderScene ();
	}

	public bool checkGlobalState(string global_state){
		bool ret = false;
		if(global_states.ContainsKey(global_state)){
			ret = global_states [global_state].check();
		}
		return ret;
	}

	public CharacterData getCharacter(string name){
		CharacterData ret = null;
		foreach (CharacterData cr in characters) {
			if(cr.ID == name){
				ret = cr;
				break;
			}
		}

		return ret;
	}

	public ObjectData getObject(string name){
		ObjectData ret = null;
		foreach (ObjectData od in objects) {
			if(od.ID == name){
				ret = od;
				break;
			}
		}
		return ret;
	}

	public AtrezzoData getAtrezzo(string name){
		AtrezzoData ret = null;
		foreach (AtrezzoData ad in atrezzos) {
			if(ad.ID == name){
				ret = ad;
				break;
			}
		}
		return ret;
	}

	public GraphConversation getConversation(string id){
		GraphConversation ret = null;
		if(graph_conversations.ContainsKey(id)){
			ret = graph_conversations [id];
		}
		return ret;
	}

	public Effect getMacro(string id){
		Effect ret = null;
		if(macros.ContainsKey(id)){
			ret = macros [id];
		}

		Debug.Log (id);
		if (id == "Random-Marcado-Pierna-ConTemplates")
			Debug.Log ("SE EJECUTO");
		return ret;
	}

	public string getCurrentScene(){
		return current_scene.GetComponent<Scene>().sceneData.getName();
	}


	// Use this for initialization
	void Awake(){
		flags = new Dictionary<string, bool> ();
		variables = new Dictionary<string, int> ();
		guistate = guiState.GAME_SELECTION;

		style = Resources.Load("basic") as GUISkin;

		gameloader = new Thread (loadGame);
		loader_state = loaderState.NOT_LOADING;
	}

	void Start () {
		loader_state = loaderState.NOT_LOADING;
	}

	public string selected_game;
	void startLoad(){
		guistate = guiState.NOTHING;
		Game.Instance = this;
		XmlDocument xmldesc = new XmlDocument ();
		xmldesc.LoadXml (File.ReadAllText(selected_game + "descriptor.xml"));

		this.descriptor = new Descriptor (xmldesc.SelectSingleNode ("game-descriptor"));
		nextChapter ();
	}

	public void nextChapter(){
		stopRendering ();
		gameloader = new Thread (loadGame);
		loader_state = loaderState.NOT_LOADING;

		this.currentChapter++;

		guistate = guiState.NOTHING;
		Game.Instance = this;
		xmld = new XmlDocument ();
		xmld.LoadXml (File.ReadAllText(selected_game + descriptor.chapters[currentChapter].path));

		loader_state = loaderState.LOADING;
		gameloader.Start ();
	}

	private void loadGame(){
		this.characters = new List<CharacterData> ();
		this.objects = new List<ObjectData> ();
		this.atrezzos = new List<AtrezzoData> ();
		this.graph_conversations = new Dictionary<string,GraphConversation> ();
		this.global_states = new Dictionary<string,Condition> ();
		this.macros = new Dictionary<string,Effect> ();
		this.adaptations = new List<Effect> ();
		this.scenes = new List<SceneData> ();
		
		
		XmlNodeList characters				= 	xmld.SelectNodes("/eAdventure/character")
					,objects				=	xmld.SelectNodes("/eAdventure/object")
					,atrezzos				=	xmld.SelectNodes("/eAdventure/atrezzoobject")
					,graph_conversations 	= 	xmld.SelectNodes("/eAdventure/graph-conversation")
					,global_states 			= 	xmld.SelectNodes("/eAdventure/global-state")
					,macros		 			= 	xmld.SelectNodes("/eAdventure/macro")
					,scenes 				= 	xmld.SelectNodes("/eAdventure/scene")
					,slidescenes 			= 	xmld.SelectNodes("/eAdventure/slidescene")
					,adaptations 			= 	xmld.SelectNodes("/eAdventure/adaptation");

		this.totals = new Dictionary<string, int> ();
		totals.Add ("Characters", characters.Count);
		totals.Add ("Objects", objects.Count);
		totals.Add ("Atrezzos", atrezzos.Count);
		totals.Add ("Graph-conversations", graph_conversations.Count);
		totals.Add ("Global-States", global_states.Count);
		totals.Add ("Macros", macros.Count);
		totals.Add ("Scene", scenes.Count+slidescenes.Count);
		totals.Add ("Adaptations", adaptations.Count);
		
		foreach(XmlElement character in characters)
			this.characters.Add (new CharacterData(character));
		
		foreach(XmlElement ob in objects)
			this.objects.Add (new ObjectData(ob));

		foreach(XmlElement at in atrezzos)
			this.atrezzos.Add (new AtrezzoData(at));
		
		foreach (XmlElement gs in global_states) {
			gs.RemoveChild(gs.FirstChild);
			Condition c = ConditionFactory.Create (gs);
			this.global_states.Add (gs.GetAttribute ("id"), ConditionFactory.Create (gs));
		}

		foreach (XmlElement gc in graph_conversations) {
			this.graph_conversations.Add (gc.GetAttribute ("id"), new GraphConversation(gc));
		}

		foreach (XmlElement gc in macros) {
			this.macros.Add (gc.GetAttribute ("id"), new Effect(gc));
		}

		RSceneData rsd;
		foreach (XmlElement scene in scenes) {
			rsd = new RSceneData (scene);
			this.scenes.Add (rsd);
			if (scene.GetAttribute ("start") == "yes")
				start_scene = rsd.getName();
		}

		SlidesceneData ssd;
		foreach (XmlElement scene in slidescenes) {
			ssd = new SlidesceneData (scene);
			this.scenes.Add (ssd);
			if (scene.GetAttribute ("start") == "yes")
				start_scene = ssd.getName();
			if (ssd.next == "end-game" || ssd.next == "end-chapter")
				end_scene = ssd.getName ();
		}

		foreach (XmlElement gc in adaptations) {
			this.adaptations.Add (new Effect(gc.FirstChild));
		}

		loader_state = loaderState.FINISHED_LOADING;
	}

	public void Execute(Secuence secuence){
		if (secuence.execute ()) {
			this.secuencepending = secuence;
		}
	}

	private void Interacted(){
		guistate = guiState.NOTHING;
		if (this.secuencepending != null) {
			Secuence tmp = secuencepending;
			secuencepending = null;
			Execute(tmp);
		}
	}
	
	// Update is called once per frame
	Vector2 clicked_on;
	void Update () {
		if (gameloader != null) {
			if (loader_state == loaderState.FINISHED_LOADING && current_scene == null){
				if(adaptations.Count>0){
					int template = Random.Range(0,adaptations.Count-1);
					adaptations[template].execute();
					Debug.Log ("Template: "+template);
				}

				renderScene (start_scene);
				gameloader = null;
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			if (secuencepending != null) {
				Interacted ();
			} else if (guistate == guiState.ANSWERS_MENU) {
			} else {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
			
				if (Physics.Raycast (ray, out hit)) {
					if (hit.transform.GetComponent<Exit> () != null) {
						hit.transform.GetComponent<Exit> ().exit ();
					} else if (hit.transform.GetComponent<Character> () != null) {
						if (guistate == guiState.NOTHING) {
							Character c = hit.transform.GetComponent<Character> ();
							foreach (Action a in c.charData.Actions)
								if (a.check ()) {
									clicked_on = Input.mousePosition;
									setActions(c.charData.Actions);
									this.guistate = guiState.OPTIONS_MENU;
									break;
								}
						}
					} else if (hit.transform.GetComponent<eObject> () != null) {
						if (guistate == guiState.NOTHING) {
							eObject ob = hit.transform.GetComponent<eObject> ();
							if (ob.DifferentActions > 1) {
								clicked_on = Input.mousePosition;
								setActions(ob.objData.Actions);
								this.guistate = guiState.OPTIONS_MENU;
							} else {
								foreach (Action a in ob.objData.Actions) {
									if (a.check ()) {
										Execute (a);
										break;
									}
								}
							}
						}
					} else if (hit.transform.GetComponent<ActiveArea> () != null) {
						if (guistate == guiState.NOTHING) {
							ActiveArea aa = hit.transform.GetComponent<ActiveArea> ();
							foreach (Action a in aa.aaData.actions)
								if (a.check ()) {
									clicked_on = Input.mousePosition;
									setActions(aa.aaData.actions);
									this.guistate = guiState.OPTIONS_MENU;
									break;
								}
						}
					} else {
						current_scene.GetComponent<Scene> ().Interacted ();
					}
				}
			}
		} else if (Input.GetMouseButtonDown (1)) {
			if(guistate == guiState.OPTIONS_MENU){
				guistate = guiState.NOTHING;
			}
		}
	}

	private void setActions(List<Action> actions){
		this.guiactions = new Dictionary<string,Action> ();

		foreach (Action a in actions) {
			if(a.check()){
				if(!guiactions.ContainsKey(a.Name))
					guiactions.Add(a.Name,a);
			}
		}

	}

	public GameObject renderScene(string scene_id){
		if (current_scene != null) {
			GameObject.Destroy(current_scene);
		}

		GameObject ret = null;
		foreach (SceneData sd in scenes) {
			if(sd.getName() == scene_id){
				ret = GameObject.Instantiate (Scene_Prefab);
				ret.GetComponent<Transform> ().localPosition = new Vector2(0f,0f);
				ret.GetComponent<Scene> ().sceneData = sd;
				break;
			}
		}

		current_scene = ret;
		return ret;
	}

	public void stopRendering(){
		if (current_scene != null) {
			GameObject.Destroy(current_scene);
		}
	}

	public void reRenderScene(){
		if(current_scene!=null)
			current_scene.GetComponent<Scene> ().renderScene ();
	}

	private GameObject putCharacter(string name, Vector3 position){
		GameObject ret = null;
		foreach (CharacterData cd in characters) {
			if(cd.ID == name){
				ret = GameObject.Instantiate (Character_Prefab);
				ret.GetComponent<Transform> ().localPosition = position;
				ret.GetComponent<Character> ().charData = cd;
				break;
			}
		}
		return ret;
	}
	
	public void talk(string text, string character){
		if (character == null){
			this.guitext = text.Replace("[]","["+playerName+"]");
			this.guistate = guiState.TALK_PLAYER;
		}else {
			this.guitext = text;
			this.guitalkerObject = null;
			this.guitalker = character;
			this.guistate = guiState.TALK_CHARACTER;
		}
	}

	public void showOptions(OptionNode options){
		this.guioptions = options;
		this.guistate = guiState.ANSWERS_MENU;
	}

	public void hideMenu(){
		this.guistate = guiState.NOTHING;
		this.guiactions = null;
	}

	public void secuenceNotFinished(Secuence secuence){
		this.secuencepending = secuence;
	}

	void OnGUI () {
		float guiscale = Screen.width/800f;

		style.box.fontSize = Mathf.RoundToInt(guiscale * 20);
		style.button.fontSize = Mathf.RoundToInt(guiscale * 20);
		style.label.fontSize = Mathf.RoundToInt(guiscale * 36);
		style.GetStyle("talk_player").fontSize = Mathf.RoundToInt(guiscale * 20);

		float rectwith = guiscale * 330;

		switch (guistate) {
		case guiState.TALK_PLAYER:
			GUILayout.BeginArea (new Rect ((Screen.width/2)-rectwith/2, 50, rectwith, 400));
			GUILayout.BeginHorizontal ();
			GUILayout.Box (guitext,style.GetStyle("talk_player"));
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();
			break;
		case guiState.TALK_CHARACTER:
			if(Camera.current!=null){
				Vector2 position;
				if(this.guitalkerObject == null){
					this.guitalkerObject = GameObject.Find(guitalker);
				}else{
					if(this.guitalkerObject != null)
						position = Camera.current.WorldToScreenPoint(this.guitalkerObject.transform.position);
					else
						position = new Vector2(Screen.width/2,100);

					if(position.x <= rectwith/2)
						position.x = rectwith/2;
					else if(position.x >= (Screen.width - rectwith/2) )
						position.x = (Screen.width - rectwith/2);

					GUILayout.BeginArea (
						new Rect (position.x-rectwith/2,
							Screen.height 
					          - position.y 
					          - (this.guitalkerObject.GetComponent<Character>().getHeight())*guiscale/2 
					          - style.box.CalcHeight(new GUIContent(guitext),rectwith), rectwith, 400)
						);
					GUILayout.BeginHorizontal ();
					GUILayout.Box (guitext,style.box);
					GUILayout.EndHorizontal ();
					GUILayout.EndArea ();
				}
			}
			break;
		case guiState.OPTIONS_MENU:
			int box_width = 0, box_height = 0;
			Action a;
			foreach(KeyValuePair<string,Action> sa in guiactions){
				a = sa.Value;
				if(a.Type == "custom"){
					box_width += a.resources["buttonNormal"].Texture.width;
					if(a.resources["buttonNormal"].Texture.height> box_height)
						box_height = a.resources["buttonNormal"].Texture.height;
				}else{
					box_width += 100;
					if(box_height<50)
						box_height = 50;
				}
			}

			GUILayout.BeginArea (new Rect (clicked_on.x-(box_width/2),(Screen.height - clicked_on.y) -(box_height/2),box_width, box_height));
			GUILayout.BeginHorizontal ();
			foreach(KeyValuePair<string,Action> sa in guiactions){
				a = sa.Value;
				switch(a.Type){
					case "custom":
						if(GUILayout.Button (a.resources["buttonNormal"].Texture,style.GetStyle("action_with_image"))){
							this.guistate = guiState.NOTHING;
							Execute(a);
						}
						break;
					default:
						if(GUILayout.Button (a.Type,style.button)){
							this.guistate = guiState.NOTHING;
							Execute(a);
						}
					break;
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();
			break;
		case guiState.ANSWERS_MENU:
			GUILayout.BeginArea (new Rect (Screen.width*0.1f, Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f));
			GUILayout.BeginVertical ();
			foreach(OptionNodeOption ono in guioptions.Options){
				if(ono.checkOption())
					if(GUILayout.Button ((string) ono.effect.aditional_info["text"],style.button)){
						guioptions.clicked(ono.child);
						Interacted();
					};
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		case guiState.LOADING_GAME:
			break;
		case guiState.NOTHING:
			if(loader_state == loaderState.LOADING){
				GUILayout.BeginArea (new Rect (Screen.width*0.1f, Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f));
				GUILayout.BeginVertical ();
				GUILayout.Label ("Cargando",style.label);

				if(totals != null){
					GUILayout.Box ("Personajes: " + this.characters.Count + " de " + totals["Characters"],style.box);
					GUILayout.Box ("Objetos: " + this.objects.Count + " de " + totals["Objects"],style.box);
					GUILayout.Box ("Objetos de Atrezzo: " + this.atrezzos.Count + " de " + totals["Atrezzos"],style.box);
					GUILayout.Box ("Estados Globales: " + this.global_states.Count + " de " + totals["Global-States"],style.box);
					GUILayout.Box ("Grafos de Conversacion: " + this.graph_conversations.Count + " de " + totals["Graph-conversations"],style.box);
					GUILayout.Box ("Macros: " + this.macros.Count + " de " + totals["Macros"],style.box);

					GUILayout.Box ("Escenas: " + this.scenes.Count + " de " + totals["Scene"],style.box);
				}


				GUILayout.EndVertical ();
				GUILayout.EndArea ();
			}
			break;
		case guiState.GAME_SELECTION:
			GUILayout.BeginArea (new Rect (Screen.width*0.1f, Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f));
			GUILayout.BeginVertical ();
			string[] games = Directory.GetDirectories("Games/");

			GUILayout.Label("eAdventure Loader v0.1",style.label);
			foreach(string game in games){
				if(GUILayout.Button (game.Split('/')[1],style.button)){
					this.selected_game = game + "/";
					this.startLoad();
				};
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		default: break;

		}
	}
}
