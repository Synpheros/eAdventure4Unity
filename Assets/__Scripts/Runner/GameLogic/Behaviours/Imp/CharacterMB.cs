using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMB : MonoBehaviour, Interactuable {

	private NPC cd;
    public NPC charData{
		get { return cd; }
		set { cd = value; }
	}

	public float update_ratio = 0.5f;
	private float current_time = 0;
    //private Animation current_anim;
    private eAnim current_anim;
	private int current_frame = 0;

    public ElementReference context;

	// Use this for initialization
	void Start () {
        foreach(ResourcesUni cr in cd.getResources()){
            if (ConditionChecker.check(cr.getConditions())) {
                string path = cr.getAssetPath (NPC.RESOURCE_TYPE_STAND_UP);

                //current_anim = Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator (path),path,new EditorImageLoader());
				current_anim = ResourceManager.Instance.getAnimation(path);

				break;
			}
		}

        this.gameObject.name = cd.getId();

		current_frame = 0;

        // ## ANIMATION METHOD ##
        /*Texture2D tmp = current_anim.getFrame(0).getImage(false,false,0).texture;
        update_ratio = current_anim.getFrame(0).getTime();//Duration/1000f;*/

        // ## EANIM METHOD ##
        Texture2D tmp = current_anim.frames[0].Image;
        update_ratio = current_anim.frames[0].Duration/1000f;
		

        this.GetComponent<Renderer> ().material.mainTexture = tmp;
        this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.getScale();

        Vector2 tmppos = new Vector2(context.getX(),context.getY()) / 10 + (new Vector2(0,-transform.localScale.y))/2;
        transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.getLayer());
	}

	public float getHeight(){
        //return (current_anim.getFrame(current_frame).getImage(false,false,0).texture.height) * context.getScale();
        return (current_anim.frames[current_frame].Image.height) * context.getScale();
	}


	public void changeFrame(){
        /*current_frame = (current_frame + 1) % current_anim.getFrames().Count;
        Texture2D tmp = current_anim.getFrame(current_frame).getImage(false,false,0).texture;
        update_ratio = current_anim.getFrame(current_frame).getTime();*/

        current_frame = (current_frame + 1) % current_anim.frames.Count;
        Texture2D tmp = current_anim.frames[current_frame].Image;
        update_ratio = current_anim.frames[current_frame].Duration/1000f;

		this.GetComponent<Renderer> ().material.mainTexture = tmp;
        this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.getScale();
	}
	
	
    bool checkingTransparency = false;
	void Update () {
		current_time += Time.deltaTime;
		
		if (current_time >= update_ratio){
			this.changeFrame();
			this.current_time = 0;
		}

        if (checkingTransparency) {
            RaycastHit hit;
            Physics.Raycast ( Camera.main.ScreenPointToRay (Input.mousePosition), out hit);

			if (((Texture2D) GetComponent<Renderer>().material.mainTexture).GetPixelBilinear (hit.textureCoord.x, hit.textureCoord.y).a > 0f)
                showHand (true);
            else
                showHand (false);
        }
	}

    bool interactable = false;
    void showHand(bool show){
        if (show && !interactable) {
            Game.Instance.setCursor ("over");
            interactable = true;
        } else if (!show && interactable) {
            Game.Instance.setCursor ("default");
            interactable = false;
        }
    }

    void OnMouseEnter(){
        checkingTransparency = true;
    }

    void OnMouseExit() {
        checkingTransparency = false;
    }

    public InteractuableResult Interacted (RaycastHit hit = default(RaycastHit)){
        InteractuableResult res = InteractuableResult.IGNORES;

        if (interactable) {
            List<Action> available = new List<Action> ();

            foreach (Action a in charData.getActions()) {
                if (ConditionChecker.check (a.getConditions ())) {
                    bool addaction = true;
                    foreach (Action a2 in available) {
                        if ((a.getType () == Action.CUSTOM || a.getType () == Action.CUSTOM_INTERACT) && (a2.getType () == Action.CUSTOM || a2.getType () == Action.CUSTOM_INTERACT)) {
                            if (((CustomAction)a).getName () == ((CustomAction)a2).getName ()) {
                                addaction = false;
                                break;
                            }
                        } else if (a.getType () == a2.getType ()){
                            addaction = false;
                            break;
                        }
                    }

                    if (addaction)
                        available.Add (a);
                }
            }

            if (available.Count > 0) {
                Game.Instance.showActions (available, Input.mousePosition);
                res = InteractuableResult.DOES_SOMETHING;
            }
        }

        return res;
    }


}
