using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Texture2DHolder : Resource {
	byte[] fileData;
	string path;
	private Texture2D tex;
	bool loaded = false;

	public Texture2D Texture {
		get { 
			if (this.tex == null) {
				tex = LoadTexture ();
			}
			
			return tex;
		}
		set { tex = value; }
	}

	// ##################################################
	// ################## CONSTRUCTORS ##################
	// ##################################################

	public Texture2DHolder(byte[] data){
		this.fileData = data;
	}

	public Texture2DHolder(string path){
        loaded = true;
		switch (ResourceManager.Instance.getLoadingType ()) {
		case ResourceManager.LoadingType.RESOURCES_LOAD:
			this.path = Game.Instance.getGameName () + "/" + path;
			tex = LoadTexture ();
			break;
		case ResourceManager.LoadingType.SYSTEM_IO:
			this.path = path;
			if (!path.Contains (ResourceManager.Instance.getSelectedGame()))
				this.path = ResourceManager.Instance.getSelectedGame() + path;

			this.fileData = LoadBytes (this.path);

			if (this.fileData == null) {
				Regex pattern = new Regex ("[óñ]");
				this.path = pattern.Replace (this.path, "+¦");
			
				this.fileData = LoadBytes (this.path);
			
				if (this.fileData == null) {
					loaded = false;
					Debug.Log ("No se pudo cargar: " + this.path);
				}
			}
			break;
		}
	}

	public bool Loaded(){
		return loaded;
	}

	// #####################################################
	// ################## LOADING METHODS ##################
	// #####################################################

	private Texture2D LoadTexture() {
		Texture2D tex = new Texture2D (1, 1);
		switch(ResourceManager.Instance.getLoadingType()){
		case ResourceManager.LoadingType.RESOURCES_LOAD:
			tex = Resources.Load (this.path.Split('.')[0]) as Texture2D;

			if (tex == null) {
				loaded = false;
				Debug.Log ("No se pudo cargar: " + this.path);
			}else
				loaded = true;

			break;
		case ResourceManager.LoadingType.SYSTEM_IO:
			tex = new Texture2D(2, 2,TextureFormat.BGRA32,false);
			tex.LoadImage(fileData);
			this.fileData = null;
			break;
		}

		return tex;
	}

	private static byte[] LoadBytes(string filePath) {
		byte[] fileData = null;

		if (System.IO.File.Exists(filePath))
			fileData = System.IO.File.ReadAllBytes(filePath);

		return fileData;
	}
}
