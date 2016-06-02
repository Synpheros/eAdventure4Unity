using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class AddGameWindow : EditorWindow {
	
	public static AddGameWindow window;
	private Dictionary<string,string> trackHeaders = new Dictionary<string, string> (), 
	games = new Dictionary<string, string> ();
	ThreadedNet net;
	string gametitle = "";

	string url = "https://rage.e-ucm.es/api/proxy/gleaner/games";
	bool ispublic = true;

	public static void Init(Dictionary<string,string> headers)
	{
		window = (AddGameWindow) EditorWindow.GetWindow(typeof(AddGameWindow));
		window.net = new ThreadedNet ();
		window.trackHeaders = headers;
	}

	public void OnGUI()
	{
		GUILayout.BeginVertical();

		EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Game Title")).x;
		gametitle = EditorGUILayout.TextField ("Game Title", gametitle, GUILayout.Width (200));
		EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize (new GUIContent ("Pass")).x;
		ispublic = EditorGUILayout.Toggle ("Public Game", ispublic, GUILayout.Width (200));

		if (GUILayout.Button ("Add Game")) {
			addGame (gametitle, ispublic);
		}

		GUILayout.EndVertical ();
	}
	public void addGame(string title, bool ispublic){
		JSONClass json = new JSONClass ();
		json.Add ("title", new JSONData (title));
		json.Add ("public", new JSONData (ispublic));

		net.POST (url, System.Text.Encoding.UTF8.GetBytes (json.ToString ()), trackHeaders, new GameCreatorListener ());
	}

	public void addGameVersion(string id){
		JSONClass json = new JSONClass ();
		json.Add ("gameId", id);

		net.POST (url + "/" + id + "/versions" , System.Text.Encoding.UTF8.GetBytes (json.ToString ()), trackHeaders, new GameVersionCreatorListener ());
	}

	public class GameCreatorListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			JSONNode node = JSON.Parse (data);
			AddGameWindow.window.addGameVersion(node ["_id"].Value);
		}

		public void Error(string error){
			Debug.Log(error);
		}
	}

	public class GameVersionCreatorListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			AddGameWindow.window.Close ();
		}

		public void Error(string error){
			Debug.Log(error);
		}
	}
}
