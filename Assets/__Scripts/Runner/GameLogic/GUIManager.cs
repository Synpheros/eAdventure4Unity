﻿using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	private static GUIManager instance;
	private Vector2 DEFORMATION = new Vector2 (40, 30);
	public GameObject Bubble_Prefab;
	GameObject bubble;
	private bool get_talker = false;
	private string talker_to_find, last_text;
	private GUIProvider guiprovider;
	private AdventureData data;
	private string current_cursor = "";

	public static GUIManager Instance {
		get { return instance; }
	}

	public string Last {
		get { return last_text; }
	}

	public GUIProvider Provider{
		get { return guiprovider; }
	}

	void Awake(){
		instance = this;
	}

	void Start () {
		guiprovider = new GUIProvider (Game.Instance.GameState.Data);
	}

	void Update () {
		if (get_talker) {
			if (GameObject.Find(talker_to_find) != null) {
				get_talker = false;
				Talk (last_text, talker_to_find);
			}
		}
	}

	public void setCursor(string cursor){
		if (cursor != current_cursor) {
			Cursor.SetCursor (guiprovider.getCursor (cursor), new Vector2 (0f, 0f), CursorMode.Auto);
			current_cursor = cursor;
		}
	}
		
	public void showHand(bool show){
		if (show) 
			setCursor ("over");
		else
			setCursor ("default");
	}

	public void Talk(string text, string talker_name = null){
		last_text = text;
		if (talker_name == null || talker_name == Player.IDENTIFIER) {
			text = text.Replace ("[]", "[" + Player.IDENTIFIER + "]");
			Vector2 position;
			NPC player = Game.Instance.GameState.getPlayer ();
			BubbleData bubble;

			if (Game.Instance.GameState.isFirstPerson ()) {
				bubble = generateBubble (player, text);
			} else {
				GameObject talker_object = getTalker (talker_name);
				if (talker_object == null)
					return;
				bubble = generateBubble (player, text, talker_object);
			}

			GUIManager.Instance.ShowBubble (bubble);
		}else {
			Vector2 position;
			GameObject talker_object = getTalker (talker_name);
			if (talker_object == null)
				return;
			
			NPC cha = Game.Instance.GameState.getCharacter(talker_name);
			BubbleData bubble = generateBubble (cha, text, talker_object);
			GUIManager.Instance.ShowBubble (bubble);
		}
	}

	public void ShowBubble(BubbleData data){
		data.origin = sceneVector2guiVector(data.origin);
		data.destiny = sceneVector2guiVector(data.destiny);

		//correctBoundaries (data);

		if (bubble != null) {
			bubble.GetComponent<Bubble> ().destroy ();
		}
		bubble = GameObject.Instantiate (Bubble_Prefab);
		bubble.GetComponent<Bubble> ().Data = data;
		bubble.transform.parent = this.transform;
	}

	public void destroyBubbles(){
		if (bubble != null)
			this.bubble.GetComponent<Bubble> ().destroy ();
	}

	public BubbleData generateBubble(NPC cha, string text, GameObject talker = null){
		BubbleData bubble = new BubbleData (text, new Vector2 (40, 60), new Vector2 (40, 45));

		Color textColor, textOutline, background, border;
		ColorUtility.TryParseHtmlString (cha.getTextFrontColor (), out textColor);
		ColorUtility.TryParseHtmlString (cha.getTextBorderColor (), out textOutline);
		ColorUtility.TryParseHtmlString (cha.getBubbleBkgColor (), out background);
		ColorUtility.TryParseHtmlString (cha.getBubbleBorderColor (), out border);

		bubble.TextColor = textColor;
		bubble.TextOutlineColor = textOutline;
		bubble.BaseColor = background;
		bubble.OutlineColor = border;

		if (talker != null) {
			Vector2 position = talker.transform.localPosition;
			position.y += talker.transform.localScale.y / 2;

			bubble.Destiny = position;
			bubble.Origin = new Vector2 (position.x, position.y - 10f);
		} else {
			bubble.Origin = new Vector2 (40, 60);
			bubble.Destiny = new Vector2 (40, 45);
		}

		return bubble;
	}

	private Vector2 sceneVector2guiVector(Vector2 v){
		float w = Screen.width, h = Screen.height,
			relation = w / h,
			height = 800 / relation, 
			width = (height / 600) * 800,
			scale = width / 800f,
			leftmargin = (800 - width)/2;

		float x = (v.x * 10 * scale) + leftmargin;
		float y = (v.y * 10 * scale);

		return new Vector2 (x, y);
	}

	private void correctBoundaries(BubbleData bubble){
		if(bubble.destiny.x <= 125f) bubble.destiny.x = 125f;
		else if(bubble.destiny.x >= (800f - 125f)) bubble.destiny.x = (800f - 125f);
	}

	private GameObject getTalker(string talker){
		GameObject ret = GameObject.Find (talker);

		if (ret == null) 
			get_talker = true;
		
		return ret;
	}
}
