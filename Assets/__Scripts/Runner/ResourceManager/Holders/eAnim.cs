using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class eFrame {
	private Texture2DHolder image;
	public Texture2D Image {
		get { return image.Texture; }
		set { image.Texture = value; }
	}

	public Texture2DHolder Holder {
		get { return image; }
		set { image = value; }
	}

	private int duration = 500;
	public int Duration {
		get { return duration; }
		set { duration = value; }
	}
}

public class eAnim {
	public List<eFrame> frames;
	public XmlDocument xmld;

	public eAnim(string eaaFile){
		frames = new List<eFrame> ();
        string path = eaaFile;
		Regex pattern = new Regex("[óñ]");
		path = pattern.Replace(path, "+¦");

		string[] splitted = path.Split ('.');
		if (splitted [splitted.Length - 1] == "eaa") {
			string eaaText = "";

			switch (ResourceManager.Instance.getLoadingType ()) {
			case ResourceManager.LoadingType.SYSTEM_IO:
				path = Game.Instance.getSelectedGame() + path;
				eaaText = System.IO.File.ReadAllText (path);
				break;
			case ResourceManager.LoadingType.RESOURCES_LOAD:
				path = Game.Instance.getGameName () + path;
				TextAsset ta = Resources.Load (path) as TextAsset;
				if(ta!=null)
					eaaText = ta.text;
				break;
			}
			parseEea (eaaText);
		} else
			createOldMethod (path);


	}

	private void parseEea(string eaaText){
		xmld = new XmlDocument ();

		xmld.LoadXml (eaaText);

		eFrame tmp;
		XmlNode animation = xmld.SelectSingleNode ("/animation");
		foreach (XmlElement node in animation.ChildNodes) {
			if (node.Name == "frame") {
				tmp = new eFrame ();
				tmp.Duration = int.Parse (node.GetAttribute ("time"));
			
				string ruta = node.GetAttribute ("uri");

				tmp.Holder = new Texture2DHolder (ruta);

				frames.Add (tmp);
			} else if (node.Name == "transition") {
				if(frames.Count>0)
					frames [frames.Count - 1].Duration += int.Parse(node.GetAttribute ("time"));
			}
		}
	}


	private static string[] extensions = {".png",".jpg",".jpeg"};
	private void createOldMethod(string name){
		xmld = new XmlDocument ();
		Texture2DHolder auxHolder;
		eFrame tmp;
		int num = 1;
		string ruta = "";

		switch (Game.Instance.getLoadingType ()) {
		case ResourceManager.LoadingType.RESOURCES_LOAD:
			ruta = name + "_" + intToStr (num);
			auxHolder = new Texture2DHolder (ruta);

			while(auxHolder.Loaded()){
				tmp = new eFrame ();
				tmp.Duration = 500;
				tmp.Holder = auxHolder;
				frames.Add(tmp);

				num++;
				ruta = name + "_" + intToStr (num);
				auxHolder = new Texture2DHolder(ruta);
			}
			break;
		case ResourceManager.LoadingType.SYSTEM_IO:
			ruta = Game.Instance.getSelectedGame() + name + "_" + intToStr (num);
			string working_extension = "";
			
			foreach (string extension in extensions) {
				auxHolder = new Texture2DHolder (ruta);
				if (System.IO.File.Exists (ruta + extension)) {
					working_extension = extension;
					break;
				}
			}

			ruta = ruta + working_extension;
			while (System.IO.File.Exists (ruta)) {
				tmp = new eFrame ();
				tmp.Duration = 500;
				tmp.Holder = new Texture2DHolder (ruta);
				frames.Add (tmp);
				num++;
				ruta = name + "_" + intToStr (num) + working_extension;
			}
			break;
		}

	}

	private static string intToStr(int number){
		if (number < 10)
			return "0" + number;
		else
			return number.ToString ();
	}

	public bool Loaded(){
		return this.frames.Count > 0;
	}
}
