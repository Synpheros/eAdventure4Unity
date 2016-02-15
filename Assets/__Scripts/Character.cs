using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	private CharacterData cd;
	public CharacterData charData{
		get { return cd; }
		set { cd = value; }
	}

	public float update_ratio = 0.5f;
	private float current_time = 0;
	private eAnim current_anim;
	private int current_frame = 0;

	public ItemReference context;

	// Use this for initialization
	void Start () {
		foreach(CharacterResource cr in cd.Resources){
			if (cr.check()) {
				this.current_anim = cr.animations["standup"];
				break;
			}
		}

		this.gameObject.name = cd.ID;

		current_frame = 0;
		Texture2D tmp = current_anim.frames[0].Image;
		update_ratio = current_anim.frames [0].Duration/1000f;
		this.GetComponent<Renderer> ().material.mainTexture = tmp;
		this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.scale;

		Vector2 tmppos = context.position / 10 + (new Vector2(0,-transform.localScale.y))/2;
		transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.layer);
	}

	public float getHeight(){
		return (current_anim.frames [current_frame].Image.height) * context.scale;
	}


	public void changeFrame(){
		current_frame = (current_frame + 1) % current_anim.frames.Count;
		Texture2D tmp = current_anim.frames[current_frame].Image;
		update_ratio = current_anim.frames [current_frame].Duration/1000f;
		this.GetComponent<Renderer> ().material.mainTexture = tmp;
		this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.scale;
	}
	
	// Update is called once per frame
	void Update () {
		current_time += Time.deltaTime;
		
		if (current_time >= update_ratio){
			this.changeFrame();
			this.current_time = 0;
		}
	}
}
