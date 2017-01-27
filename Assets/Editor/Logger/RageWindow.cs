using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class RageWindow : EditorWindow {

	public class GameConfiguration
	{
		string alias = "", score = "", progress = "", gameId = "", versionId = "", trackingCode = "";
		int maxScore = 0;
		List<Warning> warnings = new List<Warning> ();

		public string Alias{
			set { alias = value; }
			get { return alias; }
		}
		public string Score{
			set { score = value; }
			get { return score; }
		}
		public int MaxScore{
			set { maxScore = value; }
			get { return maxScore; }
		}
		public string Progress{
			set { progress = value; }
			get { return progress; }
		}
		public string GameId{
			set { gameId = value; }
			get { return gameId; }
		}
		public string VersionId{
			set { versionId = value; }
			get { return versionId; }
		}
		public string TrackingCode{
			set { trackingCode = value; }
			get { return trackingCode; }
		}
		public List<Warning> Warnings{
			set { warnings = value; }
			get { return warnings; }
		}

		public GameConfiguration(string gameId, string versionId){
			this.gameId = gameId;
			this.versionId = versionId;
		}

		public GameConfiguration(JSONNode node){
			this.gameId = node["gameId"];
			this.versionId = node["_id"];
			this.alias = node["alias"];
			this.score = node["score"];
			this.maxScore = node["maxScore"].AsInt;
			this.progress = node["progress"];
			this.trackingCode = node["trackingCode"];
		}

		public void addWarning(string message = "", string condition = ""){
			warnings.Add (new Warning (message,condition));
		}

		public JSONClass toJson(){
			JSONClass json = new JSONClass ();

			if(versionId != "") 	json.Add ("_id", versionId);
			if(alias != "") 		json.Add ("alias", alias);
			if(gameId != "") 		json.Add ("gameId", gameId);
			if(progress != "") 		json.Add ("progress", progress);
			if(score != "") 		json.Add ("score", score);
			if(trackingCode != "") 	json.Add ("trackingCode", trackingCode);

			json.Add ("maxScore", new JSONData (maxScore));

			JSONArray ws = new JSONArray ();
			foreach (Warning w in warnings) {
				ws.Add (w.toJson());
			}

			json.Add ("warnings", ws);

			return json;
		}
	}

	public class Warning
	{
		string message;
		string condition;

		public string Message{
			set { message = value; }
			get { return message; }
		}

		public string Condition{
			set { condition = value; }
			get { return condition; }
		}
		
		public Warning(string message = "", string condition = ""){
			this.message = message;
			this.condition = condition;
		}

		public JSONClass toJson(){
			JSONClass json = new JSONClass ();

			json.Add ("message", message);
			json.Add("cond", condition);

			return json;
		}
	}

	public static RageWindow window;
	private Dictionary<string,string> trackHeaders = new Dictionary<string, string> ();

	ThreadedNet net;
	string user = "", pass = "", log = "", gametitle = "";

	string baseurl = "https://rage.e-ucm.es", loginurl, proxy, gameurl, userkey = "", currentGameId = "";
	string[] gameVersions;
	string[] gameids, gametitles;

	GameConfiguration currentConfig;
	List<GameConfiguration> configs;

	int selectedgame = 0, selectedGameVersion = 0;

	bool ispublic = true;
	WWW www;

	public string Log {
		get { return log; }
		set { log = value; }
	}

	JSONNode json;
	public JSONNode Json{
		set { json = value; }
		get { return json; }
	}

	// Add menu item 
	[MenuItem("eAdventure4Unity/Open Login Screen")]
	static void Init()
	{
		if (window != null)
			window.Close ();
		
		window = (RageWindow) EditorWindow.GetWindow(typeof(RageWindow));
		window.net = new ThreadedNet ();
		window.trackHeaders.Add ("Content-Type", "application/json");

		window.initUrls ();
		window.getSavedAuthToken ();
		if (window.userkey != "") {
			window.getGames ();
		}
	}

	public void initUrls(){
		loginurl = "/api/login";
		proxy = "/proxy/gleaner/";

		gameurl = "/api" + window.proxy + "games";
	}

	public void getSavedAuthToken(){
		setAuthToken(System.IO.File.ReadAllText (Application.persistentDataPath + "/auth.ua"));
	}

	public void OnGUI()
	{
		GUILayout.BeginVertical();


		if (string.IsNullOrEmpty (userkey)) {
			EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("User")).x;
			user = EditorGUILayout.TextField ("User", user, GUILayout.Width (200));
			EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Pass")).x;
			pass = EditorGUILayout.TextField ("Pass", pass, GUILayout.Width (200));

			if (GUILayout.Button ("Login")) {
				login (user, pass);
			}
		} else {
			if (gameids != null) {
				GUILayout.BeginHorizontal ();

				EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Game ")).x;
				int preselected = selectedgame;

				selectedgame = EditorGUILayout.Popup ("Game ", selectedgame, gametitles);
				if (GUILayout.Button ("+")) {
					AddGameWindow.Init (trackHeaders);
				}

				if (preselected != selectedgame) {
					getGameVersions (gameids [selectedgame]);
				}
				GUILayout.EndHorizontal ();
			}
		}


		if(gameVersions!=null){
			GUILayout.BeginHorizontal ();
			EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Version ")).x;
			selectedGameVersion = EditorGUILayout.Popup ("Version ", selectedGameVersion, gameVersions);
			GUILayout.EndHorizontal ();

			currentConfig = configs [selectedGameVersion];

			EditorGUILayout.LabelField ("Configuration");
			EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Alias")).x;
			currentConfig.Alias = EditorGUILayout.TextField ("Alias", currentConfig.Alias);
			EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Score")).x;
			EditorGUILayout.PrefixLabel ("Score");
			currentConfig.Score = EditorGUILayout.TextArea (currentConfig.Score, GUILayout.Height (75));
			EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Max Score")).x;
			currentConfig.MaxScore = EditorGUILayout.IntField ("Max Score", currentConfig.MaxScore);
			EditorGUILayout.PrefixLabel ("Progress");
			currentConfig.Progress = EditorGUILayout.TextArea (currentConfig.Progress, GUILayout.Height (75));

			if (GUILayout.Button ("Save Configuration")) {
				saveGameVersion (currentConfig);
			}
		}
			
		EditorGUILayout.HelpBox ("This is the log:\n" + log, MessageType.None);
		GUILayout.EndVertical ();
	}

	public void login(string user, string pass){
		JSONClass json = new JSONClass ();
		json.Add ("username", new JSONData (this.user));
		json.Add ("password", new JSONData (this.pass));

		www = net.POST (baseurl + loginurl, System.Text.Encoding.UTF8.GetBytes (json.ToString ()), trackHeaders, new LoginListener ());
	}

	public void getGames(){
		www = net.GET (baseurl + gameurl + "/my", trackHeaders, new getGamesListener ());
	}

	public void setGames(JSONNode games){
		List<string> ids = new List<string> (),
					titles = new List<string> ();

		/*foreach (JSONNode child in games.Childs) {
			ids.Add (child["_id"]);
			titles.Add (child ["title"]);
		}*/

		this.gameids = ids.ToArray ();
		this.gametitles = titles.ToArray ();
	}

	public void getGameVersions(string gameId){
		www = net.GET (baseurl + gameurl + "/" + gameId + "/versions" , trackHeaders, new getVersionsListener ());
	}

	public void setGameVersions(JSONNode versions){
		List<string> vs = new List<string>();
		configs = new List<GameConfiguration> ();
		/*foreach (JSONNode child in versions.Childs) {
			vs.Add (child["_id"]);
			configs.Add (new GameConfiguration (child));
		}*/
		this.gameVersions = vs.ToArray ();
	}

	public void saveGameVersion(GameConfiguration config){
		net.POST (
			baseurl + gameurl + "/" + config.GameId + "/versions/" + config.VersionId,
			System.Text.Encoding.UTF8.GetBytes (config.toJson ().ToString ()),
			trackHeaders,
			new HelpBoxListener ()
		);
	}

	public void setAuthToken(string token){
		if (userkey != token) {
			System.IO.File.WriteAllText (Application.persistentDataPath + "/auth.ua", token);
			userkey = token;

			if (trackHeaders.ContainsKey ("Authorization"))
				trackHeaders ["Authorization"] = token;
			else
				trackHeaders.Add ("Authorization", "Bearer " + token);
		}
	}

	public class HelpBoxListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			RageWindow.window.log = data;
		}

		public void Error(string error){
			RageWindow.window.Log = error;
		}
	}

	public class LoginListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			RageWindow.window.Log = data;
			JSONNode node = JSON.Parse (data);
			RageWindow.window.setAuthToken(node ["user"]["token"].Value);
		}

		public void Error(string error){
			RageWindow.window.Log = error;
		}
	}

	public class getGamesListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			RageWindow.window.Log = data;
			JSONNode node = JSON.Parse (data);
			RageWindow.window.setGames(node);
		}

		public void Error(string error){
			RageWindow.window.Log = error;
		}
	}

	public class getVersionsListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			RageWindow.window.Log = data;
			JSONNode node = JSON.Parse (data);
			RageWindow.window.setGameVersions(node);
		}

		public void Error(string error){
			RageWindow.window.Log = error;
		}
	}
}
