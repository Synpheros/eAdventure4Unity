using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager{
    static ResourceManager instance;
    public static ResourceManager Instance {
        get { 
            if (instance == null)
                instance = new ResourceManager ();
            return instance;
        }
    }

    private Dictionary<string,Texture2DHolder> images;

    private ResourceManager (){
        this.images = new Dictionary<string, Texture2DHolder> ();
       
        //TODO:
        //support for sounds and videos
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
}

