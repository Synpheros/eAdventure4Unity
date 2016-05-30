using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public static GUIManager Instance {
		get { return instance; }
	}

	private Vector2 DEFORMATION = new Vector2 (40, 30);
	public GameObject Bubble_Prefab;

	GameObject bubble;
	void Awake(){
		instance = this;
	}

	void Start () {
	
	}

	void Update () {
		/*if (Input.GetMouseButtonDown (0)) {
			BubbleData b = new BubbleData ("Hola, ¿cómo estás Gorka?", new Vector2 (0, 0), new Vector2 (40f, 30f));
			ShowBubble (b);
		}*/
	}

	public void ShowBubble(BubbleData data){
		data.origin = sceneVector2guiVector(data.origin);
		data.destiny = sceneVector2guiVector(data.destiny);
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


	private Vector2 sceneVector2guiVector(Vector2 v){
		/* OLD METHOD
		 * float width = (Screen.height / 600) * 800;
		 * float y = ((v.y / 60f) * Screen.height) - Screen.height/2;
		 * float x = ((v.x / 80f) * width) - width/2;
		*/

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
}
