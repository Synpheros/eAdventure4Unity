using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum guiState {
    GAME_SELECTION, LOADING_GAME,NOTHING,TALK_PLAYER,TALK_CHARACTER,OPTIONS_MENU,ANSWERS_MENU
}

public class Game : MonoBehaviour {

    static Game instance;

    public static Game Instance {
        get{ return instance; }
    }

	static string gameToLoad = "";
	public static string GameToLoad {
		get{ return gameToLoad; }
		set{ gameToLoad = value; }
	}

    //###########################################################################
    //########################### GAME STATE HANDLING ###########################
    //###########################################################################

    private Dictionary<string,int> flags = new Dictionary<string, int>();
    private Dictionary<string,int> variables = new Dictionary<string, int>();

    public int checkFlag(string flag){
        int ret = FlagCondition.FLAG_INACTIVE;
        if(flags.ContainsKey(flag)){
            ret = flags [flag];
        }
        return ret;
    }

    public void setFlag(string name, int state){
        if(flags.ContainsKey(name)){
            flags [name] = state;
        }else{
            flags.Add (name, state);
        }

        //Debug.Log ("Flag '" + name + " puesta a " + state);
        TimerController.Instance.checkTimers();
        this.reRenderScene ();
    }

    public int getVariable(string var){
        int ret = 0;
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

        this.reRenderScene ();
    }

    public int checkGlobalState(string global_state){
        int ret = GlobalStateCondition.GS_SATISFIED;
        GlobalState gs = data.getChapters () [current_chapter].getGlobalState (global_state);
        if(gs != null){
            ret = ConditionChecker.check (gs);
        }
        return ret;
    }

    public string getCurrentScene(){
        return current_scene.GetComponent<SceneMB>().sceneData.getId();
    }

    public Macro getMacro(string id){
        return data.getChapters () [current_chapter].getMacro (id);
    }

    public Conversation getConversation(string id){
        return data.getChapters () [current_chapter].getConversation (id);
    }

    public List<Timer> getTimers(){
        return data.getChapters () [current_chapter].getTimers ();
    }

    public ResourcesUni getButton(Action action){
        /*CustomButton ret = null;
        foreach (CustomButton b in data.getButtons ()) {
            if (b.getAction () == name && b.getType() == type) {
                ret = b;
                break;
            } 
        }*/

        return guiprovider.getButton (action);
    }

    public Player getPlayer(){
        return data.getChapters () [current_chapter].getPlayer ();
    }

    public bool isFirstPerson(){
        return data.getPlayerMode () == DescriptorData.MODE_PLAYER_1STPERSON;
    }

    public void Move(string id, Vector2 position, int time = 0){
        GameObject go = GameObject.Find (id);
        Movable m = go.GetComponent<Movable> ();
        if (m != null)
            m.Move (position);
    }

    //##########################################################################
    //##########################################################################
    //##########################################################################

	public bool useSystemIO = true;

	public ResourceManager.LoadingType getLoadingType(){
		return (useSystemIO ? ResourceManager.LoadingType.SYSTEM_IO : ResourceManager.LoadingType.RESOURCES_LOAD);
	}

    private GUISkin style;
    public GUISkin Style {
        get { return style; }
    }
    private string playerName = "Jugador";

    void Awake(){
        Game.instance = this;
        style = Resources.Load("basic") as GUISkin;
		optionlabel = new GUIStyle(style.label);
    }

	public string getGameName(){
		return gameName;
	}

	public string gamePath = "c:/Games/";
	public string gameName = "Fire";
	private string selected_game;
	private string selected_path;

	public string getSelectedGame(){
		return selected_game;
	}
	public string getSelectedPath(){
		return selected_path;
	}

	public bool forceScene = false;
	public string scene_name = "";
	public GameObject Scene_Prefab;
	public GameObject Blur_Prefab;
	private GUIProvider guiprovider;


	AdventureData data;
	MenuMB menu;

	int current_chapter = 0;
	void Start () {
		if (Game.GameToLoad != "") {
			gameName = Game.GameToLoad;
			gamePath = ResourceManager.Instance.getCurrentDirectory () + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar;
			useSystemIO = true;
		}

		selected_path = gamePath + gameName;
		selected_game = selected_path + "/";

        //Controller.getInstance ().init ("Games/Fire.eap");
        List<Incidence> incidences = new List<Incidence>();

        data = new AdventureData ();
        AdventureHandler_ adventure = new AdventureHandler_ (data);
		switch (getLoadingType ()) {
		case ResourceManager.LoadingType.RESOURCES_LOAD:
			adventure.Parse (gameName +  "/descriptor");
			break;
		case ResourceManager.LoadingType.SYSTEM_IO:
			adventure.Parse (selected_game + "descriptor.xml");
			break;
		}
        

		/*Texture2DHolder holder = new Texture2DHolder (data.getChapters () [0].getScenes () [0].getResources () [0].getAssetPath (Scene.RESOURCE_TYPE_BACKGROUND));

		if (!holder.Loaded ())
			Debug.Log ("no se ha cargado");*/

        if(data.getCursors().Count == 0) loadDefaultCursors ();

        guiprovider = new GUIProvider (data);

        if(!forceScene)
            renderScene (data.getChapters()[current_chapter].getInitialGeneralScene().getId());
        else
            renderScene(scene_name);

        TimerController.Instance.Timers = getTimers ();
        TimerController.Instance.Run ();


	}
	
    Interactuable next_interaction = null;
    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            if (next_interaction != null && guistate != guiState.ANSWERS_MENU) {
                Interacted ();
            } else if(guistate == guiState.NOTHING){
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				List<RaycastHit> hits = new List<RaycastHit>( Physics.RaycastAll (ray));
				//hits.Reverse ();

                bool no_interaction = true;
                foreach (RaycastHit hit in hits) {
                    Interactuable interacted = hit.transform.GetComponent<Interactuable> ();
                    if (interacted != null && InteractWith (interacted)) {
                        no_interaction = false;
                        break;
                    }
                }

                if(no_interaction)
                    current_scene.GetComponent<SceneMB> ().Interacted ();
            }
        } else if (Input.GetMouseButtonDown (1)) {
            MenuMB.Instance.hide ();
        }

		if (getTalker) {
			this.guitalkerObject = GameObject.Find(guitalker);
			if (this.guitalkerObject != null) {
				getTalker = false;
				talk (guitext, guitalker);
			}
		}
    }

    private bool InteractWith(Interactuable interacted){
        bool exit = false;
        next_interaction = null;
        switch (interacted.Interacted ()) {
            case InteractuableResult.DOES_SOMETHING:
                exit = true;
                break;
            case InteractuableResult.REQUIRES_MORE_INTERACTION:
                exit = true;
                next_interaction = interacted;
                break;
            case InteractuableResult.IGNORES:
            default:
                break;
        }
        return exit;
    }

    public void Execute(Interactuable interactuable){
        MenuMB.Instance.hide (true);
        if (interactuable.Interacted() == InteractuableResult.REQUIRES_MORE_INTERACTION) {
            this.next_interaction = interactuable;
        }
    }

    private void Interacted(){
        guistate = guiState.NOTHING;
		GUIManager.Instance.destroyBubbles ();
        if (this.next_interaction != null) {
            Interactuable tmp = next_interaction;
            next_interaction = null;
			Execute(tmp);
        }
    }

    public bool isSomethingRunning(){
        return next_interaction != null;
    }

    public NPC getCharacter(string name){
        return data.getChapters () [current_chapter].getCharacter (name);
    }

    public Item getObject(string name){
        return data.getChapters () [current_chapter].getItem (name);
    }

    public Atrezzo getAtrezzo(string name){
        Chapter c =  data.getChapters () [current_chapter];
        return c.getAtrezzo (name);
    }

    //#################################################################
    //########################### RENDERING ###########################
    //#################################################################

    GameObject current_scene;
    public GameObject renderScene(string scene_id, int transition_time = 0, int transition_type = 0){
        MenuMB.Instance.hide (true);
        if (current_scene != null) {
            current_scene.GetComponent<SceneMB> ().destroy (transition_time / 1000f);
        }

        GameObject ret = null;
        ret = GameObject.Instantiate (Scene_Prefab);
        ret.GetComponent<Transform> ().localPosition = new Vector2(0f,0f);
        ret.GetComponent<SceneMB> ().sceneData = data.getChapters () [current_chapter].getGeneralScene (scene_id);

        current_scene = ret;
        return ret;
    }

    public void reRenderScene(){
        if(current_scene!=null)
            current_scene.GetComponent<SceneMB> ().renderScene ();
    }

    public void renderLastScene(){
        foreach (GeneralScene scene in data.getChapters()[current_chapter].getGeneralScenes()) {
            if (scene.getType () == GeneralScene.GeneralSceneSceneType.SLIDESCENE && ((Slidescene)scene).getNext () == Slidescene.ENDCHAPTER) {
                renderScene (scene.getId ());
                break;
            }
        }
    }


    Dictionary<string,Texture2D> cursores = new Dictionary<string, Texture2D>();
    public void setCursor(string cursor){
        if(!cursores.ContainsKey(cursor)){
            string path = data.getCursorPath(cursor);
            if (path != null) {
                Texture2DHolder th = new Texture2DHolder (path);
                cursores.Add (cursor, th.Texture);
            } else {
                if(!cursores.ContainsKey("default"))
                    cursores.Add ("default", new Texture2DHolder(data.getCursorPath("default")).Texture);
                cursores.Add (cursor, cursores ["default"]);
            }
        }

        Cursor.SetCursor (cursores[cursor], new Vector2 (0f, 0f), CursorMode.Auto);
    }

    private void loadDefaultCursors(){
        cursores.Add("default",ResourceManager.Instance.getImage ("gui/cursors/default.png"));
        cursores.Add("over",ResourceManager.Instance.getImage ("gui/cursors/over.png"));
        cursores.Add("exit",ResourceManager.Instance.getImage ("gui/cursors/exit.png"));
        cursores.Add("action",ResourceManager.Instance.getImage ("gui/cursors/action.png"));
    }

    //#################################################################
    //#################################################################
    //#################################################################

    //private SceneElement actionshower;
    public void showActions (List<Action> actions, Vector2 position/*, SceneElement shower = null*/){
        //this.actionshower = shower;
        Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        MenuMB.Instance.transform.position = new Vector3 (pos.x, pos.y, -30);;
        MenuMB.Instance.setActions(actions);
        MenuMB.Instance.show ();
        this.clicked_on = position;
    }

	GameObject blur;
    public void showOptions(ConversationNodeHolder options){
        if (options.getNode ().getType () == ConversationNodeViewEnum.OPTION) {
			blur = GameObject.Instantiate (Blur_Prefab);
			blur.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 1);
            this.guioptions = options;
            this.guistate = guiState.ANSWERS_MENU;
        }
    }

	bool getTalker = false;
    public void talk(string text, string character){
        if (character == null || character == Player.IDENTIFIER){
			this.guitalker = "Player";
            this.guitext = text.Replace("[]","["+playerName+"]");
            this.guistate = guiState.TALK_PLAYER;
			Vector2 position;

			if (isFirstPerson ()) {
				GUIManager.Instance.ShowBubble (
					new BubbleData (
						guitext
					, new Vector2 (40, 60)
					, new Vector2 (40, 45)
					, Color.white
					, Color.blue
					, Color.white
					, Color.blue
					)
				);
			} else {
				this.guitalkerObject = null;
				this.guitalkerObject = GameObject.Find (guitalker);
				if (this.guitalkerObject == null) {
					getTalker = true;
					return;
				}

				NPC cha = data.getChapters () [current_chapter].getPlayer ();

				BubbleData bubble = generateBubble (cha);

				position = this.guitalkerObject.transform.localPosition;

				position.y += this.guitalkerObject.transform.localScale.y/2;

				bubble.Destiny = position;
				bubble.Origin = new Vector2 (position.x, position.y - 10f);
				GUIManager.Instance.ShowBubble (bubble);
			}
        }else {
            this.guitext = text;
            this.guitalker = character;
			this.guitalkerObject = null;
            this.guistate = guiState.TALK_CHARACTER;
			Vector2 position;

			if (this.guitalkerObject == null) {
				this.guitalkerObject = GameObject.Find (guitalker);
				if (this.guitalkerObject == null) {
					getTalker = true;
					return;
				}
			}

			NPC cha = data.getChapters () [current_chapter].getCharacter (guitalker);

			BubbleData bubble = generateBubble (cha);

			position = this.guitalkerObject.transform.localPosition;

			/*if(position.x <= rectwith/2)
				position.x = rectwith/2;
			else if(position.x >= (Screen.width - rectwith/2) )
				position.x = (Screen.width - rectwith/2);*/

			position.y += this.guitalkerObject.GetComponent<CharacterMB> ().transform.localScale.y/2;

			bubble.Destiny = position;
			bubble.Origin = new Vector2 (position.x, position.y - 10f);
			GUIManager.Instance.ShowBubble (bubble);
        }
    }

	private BubbleData generateBubble(NPC cha){
		BubbleData bubble = new BubbleData (guitext, new Vector2 (40, 60), new Vector2 (40, 45));

		Color textColor, textOutline, background, border;
		ColorUtility.TryParseHtmlString (cha.getTextFrontColor (), out textColor);
		ColorUtility.TryParseHtmlString (cha.getTextBorderColor (), out textOutline);
		ColorUtility.TryParseHtmlString (cha.getBubbleBkgColor (), out background);
		ColorUtility.TryParseHtmlString (cha.getBubbleBorderColor (), out border);

		bubble.TextColor = textColor;
		bubble.TextOutlineColor = textOutline;
		bubble.BaseColor = background;
		bubble.OutlineColor = border;

		return bubble;
	}

    private Vector2 clicked_on;
    private guiState guistate = guiState.NOTHING;
    private string guitext;
    private string guitalker;
    private GameObject guitalkerObject;
    private List<Action> guiactions;
    private ConversationNodeHolder guioptions;

	GUIStyle optionlabel;

    void OnGUI () {
        float guiscale = Screen.width/800f;

        style.box.fontSize = Mathf.RoundToInt(guiscale * 20);
        style.button.fontSize = Mathf.RoundToInt(guiscale * 20);
        style.label.fontSize = Mathf.RoundToInt(guiscale * 20);
		optionlabel.fontSize = Mathf.RoundToInt (guiscale * 36);
        //style.label.fontSize = Mathf.RoundToInt(guiscale * 20);
        style.GetStyle("talk_player").fontSize = Mathf.RoundToInt(guiscale * 20);

        float rectwith = guiscale * 330;

        switch (guistate) {
		case guiState.TALK_PLAYER:
            /*GUILayout.BeginArea (new Rect ((Screen.width/2)-rectwith/2, 50, rectwith, 400));
            GUILayout.BeginHorizontal ();
            GUILayout.Box (guitext,style.GetStyle("talk_player"));
            GUILayout.EndHorizontal ();
            GUILayout.EndArea ();*/
            break;
        case guiState.TALK_CHARACTER:
			/*GUIStyle current = new GUIStyle(style.box);
                    if (this.guitalkerObject != null) {
                        position = Camera.current.WorldToScreenPoint (this.guitalkerObject.transform.position);
                        Color tmp = Color.black;
                        NPC cha = data.getChapters () [current_chapter].getCharacter (guitalker);


                        ColorUtility.TryParseHtmlString(
                            cha.getTextFrontColor()
                            ,out tmp);
                        
                        current.normal.textColor = tmp;
                        current.normal.background = BorderGenerator.generateFor (cha);
                    }else
                        position = new Vector2(Screen.width/2,100);

                    if(position.x <= rectwith/2)
                        position.x = rectwith/2;
                    else if(position.x >= (Screen.width - rectwith/2) )
                        position.x = (Screen.width - rectwith/2);

                    GUILayout.BeginArea (
                        new Rect (position.x-rectwith/2,
                            Screen.height 
                            - position.y 
                            - (this.guitalkerObject.GetComponent<CharacterMB>().getHeight())*guiscale/2 
                            - current.CalcHeight(new GUIContent(guitext),rectwith), rectwith, 400)
                    );
                    GUILayout.BeginHorizontal ();
                    GUILayout.Box (guitext,current);
                    GUILayout.EndHorizontal ();
                    GUILayout.EndArea ();*/
            break;
		case guiState.ANSWERS_MENU:
			GUILayout.BeginArea (new Rect (Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f));
			GUILayout.BeginVertical ();
			OptionConversationNode options = (OptionConversationNode)guioptions.getNode ();

			GUILayout.Label (guitext, optionlabel);
            for(int i = 0; i < options.getLineCount(); i++){
                ConversationLine ono = options.getLine (i);
                if(ConditionChecker.check(options.getLineConditions(i)))
                    if(GUILayout.Button ((string) ono.getText(),style.button)){
						GameObject.Destroy (blur);
                        guioptions.clicked(i);
                        Interacted();
                    };
            }
            GUILayout.EndVertical ();
            GUILayout.EndArea ();
            break;
        case guiState.LOADING_GAME:
            break;
        case guiState.NOTHING:
            /*if(loader_state == loaderState.LOADING){
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
            }*/
            break;
        case guiState.GAME_SELECTION:
            /*GUILayout.BeginArea (new Rect (Screen.width*0.1f, Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f));
            GUILayout.BeginVertical ();
            string[] games = Directory.GetDirectories("Games/");

            GUILayout.Label("eAdventure Loader v0.1",style.label);
            foreach(string game in games){
                if(GUILayout.Button (game.Split('/')[1],style.button)){
                    this.selected_game = game + "/";
                    //this.startLoad();
                };
            }
            GUILayout.EndVertical ();
            GUILayout.EndArea ();
            break;*/
        default: break;

        }
    }
}
