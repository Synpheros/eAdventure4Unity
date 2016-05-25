using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Xml;

public class GameButtonMB : MonoBehaviour {

	string path, imagepath;

	public string Path{
		get { return path; }
		set { 
			path = value + System.IO.Path.DirectorySeparatorChar; 
			imagepath = path + System.IO.Path.DirectorySeparatorChar + "gui" + System.IO.Path.DirectorySeparatorChar;
		}
	}

	Image image;
	Text text;
	// Use this for initialization
	void Start () {
		Transform panel = this.transform.FindChild ("Panel");
		image = panel.FindChild ("Miniatura").GetComponent<Image>();
		text = panel.FindChild ("Titulo").GetComponent<Text>();

		Texture2D tx;
		if(System.IO.File.Exists(imagepath + "standalone_game_icon.png"))
			tx = ResourceManager.Instance.getImage (imagepath + "standalone_game_icon.png");
		else
			tx = ResourceManager.Instance.getImage (imagepath + "Icono-Motor-128x128.png");
		
		image.sprite = Sprite.Create (tx, new Rect (0, 0, tx.width, tx.height), new Vector2 (0.5f, 0.5f));

		XmlDocument doc = new XmlDocument();
		doc.Load (path + "descriptor.xml");

		text.text = doc.SelectSingleNode ("/game-descriptor/title").InnerText;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
