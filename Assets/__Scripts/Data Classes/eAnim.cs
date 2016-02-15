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
		string path = Game.Instance.selected_game + eaaFile;
		Regex pattern = new Regex("[óñ]");
		path = pattern.Replace(path, "+¦");

		string[] splitted = path.Split ('.');
		if (splitted [splitted.Length - 1] == "eaa") {
			string eaaText = System.IO.File.ReadAllText (path);
			parseEea (eaaText);
		} else
			createOldMethod (path);
	}

	private void parseEea(string eaaText){
		xmld = new XmlDocument ();

		xmld.LoadXml (eaaText);

		eFrame tmp;
		foreach (XmlElement node in xmld.SelectNodes("/animation/frame")) {
			tmp = new eFrame ();
			tmp.Duration = int.Parse(node.GetAttribute("time"));

			//#################################################
			//############# RESOURCES.LOAD METHOD #############
			//#################################################
			/*
				string ruta = node.GetAttribute("uri").Split('/')[2].Split('.')[0];
				tmp.Image = Resources.Load("animation/" + ruta) as Texture2D;
				if(tmp.Image==null){
					Regex pattern = new Regex("[óñ]");
					ruta = pattern.Replace(ruta, "+¦");

					tmp.Image = Resources.Load("animation/" + ruta) as Texture2D;

					if(tmp.Image==null)
						Debug.Log("No se pudo cargar: " + ruta);
				}
			*/

			
			string ruta = node.GetAttribute("uri");//.Split('/')[2];

			tmp.Holder = new Texture2DHolder(ruta);

			frames.Add(tmp);
		}
	}


	private static string[] extensions = {".png",".jpg",".jpeg"};
	private void createOldMethod(string name){
		xmld = new XmlDocument ();
		
		eFrame tmp;
		int num = 1;
		string ruta = name + "_" + intToStr (num);

		string working_extension = "";
		foreach(string extension in extensions)
			if (System.IO.File.Exists (ruta + extension)){
				working_extension = extension;
				break;
			}

		ruta = ruta + working_extension;
		while(System.IO.File.Exists (ruta)){
			tmp = new eFrame ();
			tmp.Duration = 500;
			tmp.Holder = new Texture2DHolder(ruta);
			frames.Add(tmp);
			num++;
			ruta = name + "_" + intToStr (num) + working_extension;
		}
	}

	private static string intToStr(int number){
		if (number < 10)
			return "0" + number;
		else
			return number.ToString ();
	}
}
