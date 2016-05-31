using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class LoginWindow : EditorWindow {

	public static LoginWindow window;
	private Dictionary<string,string> trackHeaders = new Dictionary<string, string> ();
	ThreadedNet net;
	string user = "", pass = "", log = "", baseurl = "https://rage.e-ucm.es/", loginurl = "api/login";
	WWW www;

	public string Log {
		get { return log; }
		set { log = value; }
	}

	// Add menu item 
	[MenuItem("eAdventure4Unity/Open Login Screen")]
	static void Init()
	{
		window = (LoginWindow) EditorWindow.GetWindow(typeof(LoginWindow));
		window.net = new ThreadedNet ();
		window.trackHeaders.Add ("Content-Type", "application/json");
	}

	public void OnGUI()
	{
		GUILayout.BeginVertical();
		EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent("User")).x;
		user = EditorGUILayout.TextField("User", user ,GUILayout.Width(200));
		EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent("Pass")).x;
		pass = EditorGUILayout.TextField("Pass", pass ,GUILayout.Width(200));

		if (GUILayout.Button ("Login")) {

			JSONClass json = new JSONClass ();
			json.Add ("username", new JSONData(this.user));
			json.Add ("password", new JSONData(this.pass));

			www = net.POST (baseurl + loginurl, System.Text.Encoding.UTF8.GetBytes (json.ToString()),trackHeaders, new HelpBoxListener());
		}

		EditorGUILayout.HelpBox ("This is the log:\n" + log, MessageType.None);
		GUILayout.EndVertical ();
	}

	public class HelpBoxListener : ThreadedNet.IRequestListener {
		public void Result(string data){
			LoginWindow.window.Log = data;
		}

		public void Error(string error){
			LoginWindow.window.Log = error;
		}
	}
}
