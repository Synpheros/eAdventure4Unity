using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Texture2DHolder {

	/*public static Texture2D LoadTexture(string filePath) {
		
		Texture2D tex = null;
		byte[] fileData;
		
		if (System.IO.File.Exists(filePath))     {
			fileData = System.IO.File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		return tex;
	}
	public static byte[] LoadBytes(string filePath) {
		byte[] fileData = null;
		
		if (System.IO.File.Exists(filePath))
			fileData = System.IO.File.ReadAllBytes(filePath);
		
		return fileData;
	}*/


	private string path;
	private Texture2D tex;
	public Texture2D Texture {
		get { 
			if (this.tex == null) {
				tex = new Texture2D (100, 100, TextureFormat.BGRA32,false);
				Texture2D tmp = (Resources.Load (path) as Texture2D);

				if (tmp == null) {
					Regex pattern = new Regex("[óñ]");
					path = pattern.Replace(path, "+¦");

					tmp = (Resources.Load (path) as Texture2D);
				}

				tex = tmp;
			}
			
			return tex;
		}
		set { tex = value; }
	}

	public Texture2DHolder(string path){
		if(!path.Contains(Game.Instance.selected_game))
			path = Game.Instance.selected_game + path;

		this.path = path.Split('.')[0];

		/*this.fileData = LoadBytes(path);

		if(this.fileData==null){
			Regex pattern = new Regex("[óñ]");
			path = pattern.Replace(path, "+¦");
			
			this.fileData = LoadBytes(path);
			
			if(this.fileData==null)
				Debug.Log("No se pudo cargar: " + path);
		}*/
	}

	public Texture2DHolder(Texture2D texture){
		this.tex = texture;
	}
}
