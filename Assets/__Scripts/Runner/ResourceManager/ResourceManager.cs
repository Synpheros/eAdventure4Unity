using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;

public sealed class ResourceManager{

	//#############################################
	//################# SINGLETON #################
	//#############################################

	public enum LoadingType
	{
		SYSTEM_IO,
		RESOURCES_LOAD
	}

    static ResourceManager instance;
    public static ResourceManager Instance {
        get { 
            if (instance == null)
                instance = new ResourceManager ();
            return instance;
        }
    }

	//##################################################
	//################# IMPLEMENTATION #################
	//##################################################

	private class MovieHolder{
		public MovieTexture movie;
	}

	LoadingType type = LoadingType.SYSTEM_IO;

    private Dictionary<string,Texture2DHolder> images;
	private Dictionary<string,eAnim> animations;

    private ResourceManager (){
        this.images = new Dictionary<string, Texture2DHolder> ();
		this.animations = new Dictionary<string, eAnim> ();

		if (Game.Instance != null) {
			type = Game.Instance.getLoadingType ();
		} else
			type = LoadingType.SYSTEM_IO;
       
        //TODO:
        //support for sounds and videos
    }

	public LoadingType getLoadingType(){
		return type;
	}

	public string getSelectedGame(){
		if (Game.Instance != null)
			return Game.Instance.getSelectedGame ();
		else
			return "";
	}

    public Texture2D getImage(string uri){
        if (images.ContainsKey (uri))
            return images [uri].Texture;
        else {
            Texture2DHolder holder = new Texture2DHolder (uri);
            if (holder.Loaded ()) {
                images.Add (uri, holder);
                return holder.Texture;
            } else
                return null;
        }
    }

	public eAnim getAnimation(string uri){
		if (animations.ContainsKey (uri))
			return animations [uri];
		else {
			eAnim animation = new eAnim (uri);
			if (animation.Loaded ()) {
				animations.Add (uri, animation);
				return animation;
			} else
				return null;
		}
	}

	public MovieTexture getVideo(string uri){
		MovieHolder holder = new MovieHolder ();

		Game.Instance.StartCoroutine (loadMovie (uri,holder));

		while (!movieLoaded (holder)) {
		}

		return holder.movie;
	}

	IEnumerator loadMovie(string uri, MovieHolder holder){
		string url_prefix = "file:///";
		string videoname = uri;
		string dir = "";
		if (System.IO.File.Exists (Game.Instance.getSelectedGame() + videoname.Split ('.') [0] + ".ogv"))
			dir = url_prefix + Game.Instance.getSelectedGame() + videoname.Split ('.') [0] + ".ogv";
		else
			dir = url_prefix + Game.Instance.getSelectedGame() + videoname;

		WWW www = new WWW (dir);
		yield return www;
		if (www.error != null) {
			Debug.Log ("Error: Can't laod movie! - " + www.error);
			yield break;

		} else {
			MovieTexture video = www.movie as MovieTexture;
			Debug.Log("Movie loaded");
			Debug.Log(www.movie);
			holder.movie = video;
		}
	}

	bool movieLoaded(MovieHolder holder){
		return holder.movie == null;
	}

	public bool extracted = false;
	public void extractFile(string file){
		extracted = false;
		string[] dir = file.Split (System.IO.Path.DirectorySeparatorChar);
		string filename = dir [dir.Length - 1].Split ('.') [0];
			
		string exportLocation = getCurrentDirectory () + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar + filename;

		ZipUtil.Unzip (file, exportLocation);

		foreach(string f in System.IO.Directory.GetFiles(exportLocation)){
			if (!f.Contains (".xml"))
				System.IO.File.Delete (f);
		}

		string[] tmp;
		foreach(string f in System.IO.Directory.GetDirectories(exportLocation)){
			tmp = f.Split (System.IO.Path.DirectorySeparatorChar);
			if (tmp[tmp.Length-1] != "assets" && tmp[tmp.Length-1] != "gui")
				System.IO.Directory.Delete (f,true);
		}

		extracted = true;
	}

	public string getCurrentDirectory(){
		string ret = "";
#if UNITY_ANDROID
		ret = "/mnt/sdcard/uAdventure";//Application.persistentDataPath;
#elif UNITY_IPHONE
		ret = "";
#else
		ret = System.IO.Directory.GetCurrentDirectory ();
#endif
		return ret;
	}

	public string getStoragePath(){
		string ret = "";
#if UNITY_ANDROID
		ret = "/mnt/sdcard";
#elif UNITY_IPHONE
		ret = "";
#else
		ret = System.IO.Directory.GetCurrentDirectory ();
#endif
	return ret;
	}


}

