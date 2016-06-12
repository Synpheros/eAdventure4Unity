using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum guiState {
    GAME_SELECTION, LOADING_GAME,NOTHING,TALK_PLAYER,TALK_CHARACTER,OPTIONS_MENU,ANSWERS_MENU
}

public class Game : MonoBehaviour {

	//#################################################################
	//########################### SINGLETON ###########################
	//#################################################################

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
		Tracker.T ().Var (name, (state == FlagCondition.FLAG_ACTIVE ? 1 : 0));
		Tracker.T ().RequestFlush ();

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

		Tracker.T ().Var (name, value);
		Tracker.T ().RequestFlush ();

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

	public bool useSystemIO = true, forceScene = false;
	private GUISkin style;
	public string gamePath = "c:/Games/", gameName = "Fire", scene_name = "";
	private string playerName = "Jugador", selected_game, selected_path;
	int current_chapter = 0;
	public GameObject Scene_Prefab, Blur_Prefab;
	private GUIProvider guiprovider;
	AdventureData data;
	MenuMB menu;
	Interactuable next_interaction = null;
	GameObject current_scene;
    
    public GUISkin Style {
        get { return style; }
    }

	public ResourceManager.LoadingType getLoadingType(){
		return (useSystemIO ? ResourceManager.LoadingType.SYSTEM_IO : ResourceManager.LoadingType.RESOURCES_LOAD);
	}

	public string getGameName(){
		return gameName;
	}

	public string getSelectedGame(){
		return selected_game;
	}
	public string getSelectedPath(){
		return selected_path;
	}

	void Awake(){
		Game.instance = this;
		style = Resources.Load("basic") as GUISkin;
		optionlabel = new GUIStyle(style.label);
	}

	void Start () {
		if (Game.GameToLoad != "") {
			gameName = Game.GameToLoad;
			gamePath = ResourceManager.Instance.getCurrentDirectory () + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar;
			useSystemIO = true;
		}

		selected_path = gamePath + gameName;
		selected_game = selected_path + "/";

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

        if(data.getCursors().Count == 0) loadDefaultCursors ();

        guiprovider = new GUIProvider (data);

        if(!forceScene)
            renderScene (data.getChapters()[current_chapter].getInitialGeneralScene().getId());
        else
            renderScene(scene_name);

        TimerController.Instance.Timers = getTimers ();
        TimerController.Instance.Run ();
	}

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

    public GameObject renderScene(string scene_id, int transition_time = 0, int transition_type = 0){
        MenuMB.Instance.hide (true);
        if (current_scene != null) {
            current_scene.GetComponent<SceneMB> ().destroy (transition_time / 1000f);
        }

        GameObject ret = null;
        ret = GameObject.Instantiate (Scene_Prefab);
        ret.GetComponent<Transform> ().localPosition = new Vector2(0f,0f);
        ret.GetComponent<SceneMB> ().sceneData = data.getChapters () [current_chapter].getGeneralScene (scene_id);

		Tracker.T ().Screen (scene_id);
		Tracker.T ().RequestFlush ();

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

    public void showActions (List<Action> actions, Vector2 position/*, SceneElement shower = null*/){
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
		GUIManager.Instance.Talk (text, character);
    }

    private Vector2 clicked_on;
    private guiState guistate = guiState.NOTHING;
    private List<Action> guiactions;
    private ConversationNodeHolder guioptions;

	GUIStyle optionlabel;

    void OnGUI () {
        float guiscale = Screen.width/800f;

        style.box.fontSize = Mathf.RoundToInt(guiscale * 20);
        style.button.fontSize = Mathf.RoundToInt(guiscale * 20);
        style.label.fontSize = Mathf.RoundToInt(guiscale * 20);
		optionlabel.fontSize = Mathf.RoundToInt (guiscale * 36);
        style.GetStyle("talk_player").fontSize = Mathf.RoundToInt(guiscale * 20);

        float rectwith = guiscale * 330;

        switch (guistate) {
		case guiState.ANSWERS_MENU:
			GUILayout.BeginArea (new Rect (Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f));
			GUILayout.BeginVertical ();
			OptionConversationNode options = (OptionConversationNode)guioptions.getNode ();

			GUILayout.Label (GUIManager.Instance.Last, optionlabel);
			for (int i = 0; i < options.getLineCount (); i++) {
				ConversationLine ono = options.getLine (i);
				if (ConditionChecker.check (options.getLineConditions (i)))
				if (GUILayout.Button ((string)ono.getText (), style.button)) {
					GameObject.Destroy (blur);
					guioptions.clicked (i);
					Tracker.T ().Choice (GUIManager.Instance.Last, ono.getText ());
					Tracker.T ().RequestFlush ();
					Interacted ();
				}
				;
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
        default: break;
        }
    }
}
