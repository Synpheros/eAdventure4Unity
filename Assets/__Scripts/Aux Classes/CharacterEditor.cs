using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor {

	void OnEnable(){
	}

	SecuenceWindow editor = null;
	public override void OnInspectorGUI(){
		Character c = (target as Character);

		foreach (Action a in c.charData.Actions) {
			if(GUILayout.Button("Edit: " + a.Type)){
				if(editor == null){
					editor = EditorWindow.GetWindow<SecuenceWindow>();
					editor.Effect = a.effect;
				}
			}
		}

	}


}
