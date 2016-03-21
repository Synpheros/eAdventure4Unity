using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoaderRequest {

	public System.Type t;
	public ResourceHandler handler;
	public string path;


	public LoaderRequest(ResourceHandler handler, string path, System.Type t){
		this.handler = handler;
		this.path = path;
		this.t = t;
	}

}

public class ResourceLoader : MonoBehaviour {

	public static ResourceLoader S;

	private Stack<LoaderRequest> requests;

	public void loadResource<T>(ResourceHandler handler, string path){
		requests.Push (new LoaderRequest(handler, path,typeof(T)));
	}

	// Use this for initialization
	void Awake () {
		S = this;
		this.requests = new Stack<LoaderRequest> ();
	}
	
	// Update is called once per frame
	void Update () {
		while(requests.Count > 0){
			LoaderRequest r = requests.Pop ();
			r.handler.loaded(Resources.Load(r.path,r.t));
		}
	}
}
