using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager{

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
}

