using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectMB : MonoBehaviour, Interactuable, Movable {
	
    private Item od;
    public Item objData{
		get { return od; }
		set { od = value; }
	}
    public ElementReference context;

	public int DifferentActions{
		get { 
			int tmp = 0;
            foreach(Action a in od.getActions()){
                if(ConditionChecker.check(a.getConditions())){
					tmp++;
				}
			}
			return tmp;
		}
	}
	
    private ResourcesUni current_resource;
	
	void Start () {
        foreach(ResourcesUni cr in od.getResources()){
            if (ConditionChecker.check(cr.getConditions())) {
                current_resource = cr;
                string path = cr.getAssetPath (Item.RESOURCE_TYPE_IMAGE);
                Texture2D th = ResourceManager.Instance.getImage (path);

                this.GetComponent<Renderer> ().material.mainTexture = th;
                this.transform.localScale = new Vector3(th.width/10,th.height/10,1) * context.getScale();
                break;
            }
        }

        this.gameObject.name = od.getId();
		
        Vector2 tmppos = new Vector2(context.getX(),context.getY()) / 10 + (new Vector2(0,-transform.localScale.y))/2;
        transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.getLayer());

		hasOverSprite = current_resource.getAssetPath (Item.RESOURCE_TYPE_IMAGEOVER) != null;
	}
	
    bool dragging = false;
	bool checkingTransparency = false;
    Action drag;
	void Update () {
        if (dragging) {
            Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            this.transform.localPosition = pos;
            if (Input.GetMouseButtonUp (0)) {
                dragging = false;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll (ray);
                bool no_interaction = true;
                ActiveAreaMB aa;

                foreach (RaycastHit hit in hits) {
                    aa = hit.transform.GetComponent<ActiveAreaMB> ();
                    if (aa != null) {
                        if (aa.aaData.getId () == drag.getTargetId()) {
                            Game.Instance.Execute (new EffectHolder (drag.getEffects ()));
                            break;
                        }
                    }
                }
            }
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
	bool hasOverSprite = false;
	void showHand(bool show){
		if (show && !interactable) {
			Game.Instance.setCursor ("over");
			if(hasOverSprite)
				GetComponent<Renderer> ().material.mainTexture = ResourceManager.Instance.getImage (current_resource.getAssetPath(Item.RESOURCE_TYPE_IMAGEOVER));
			interactable = true;
		} else if (!show && interactable) {
			Game.Instance.setCursor ("default");
			GetComponent<Renderer> ().material.mainTexture = ResourceManager.Instance.getImage (current_resource.getAssetPath(Item.RESOURCE_TYPE_IMAGE));
			interactable = false;
		}
	}

	void OnMouseEnter(){
		checkingTransparency = true;
	}

	void OnMouseExit() {
		checkingTransparency = false;
	}

    public InteractuableResult Interacted (RaycastHit hit = new RaycastHit()){
        InteractuableResult ret = InteractuableResult.IGNORES;

		if (interactable) {
			if (od.isReturnsWhenDragged ()) {
				switch (od.getBehaviour ()) {
				case Item.BehaviourType.FIRST_ACTION:
					foreach (Action a in od.getActions()) {
						if (ConditionChecker.check (a.getConditions ())) {
							Game.Instance.Execute (new EffectHolder (a.getEffects ()));
							break;
						}
					}
					ret = InteractuableResult.DOES_SOMETHING;
					break;
				case Item.BehaviourType.NORMAL:
					List<Action> available = new List<Action> ();
					foreach (Action a in od.getActions()) {
						if (ConditionChecker.check (a.getConditions ())) {
							bool addaction = true;
							foreach (Action a2 in available) {
								if ((a.getType () == Action.CUSTOM || a.getType () == Action.CUSTOM_INTERACT) && (a2.getType () == Action.CUSTOM || a2.getType () == Action.CUSTOM_INTERACT)) {
									if (((CustomAction)a).getName () == ((CustomAction)a2).getName ()) {
										addaction = false;
										break;
									}
								} else if (a.getType () == a2.getType ()) {
									addaction = false;
									break;
								}
							}
                    
							if (addaction)
								available.Add (a);
						}
					}


            //We check if it's an examine action, otherwise we create one and add it
					bool addexamine = true;
					foreach (Action a in available) {
						if (a.getType () == Action.EXAMINE) {
							addexamine = false;
							break;
						}
					}

					if (addexamine) {
						Action ex = new Action (Action.EXAMINE);
						Effects exeff = new Effects ();
						exeff.add (new SpeakPlayerEffect (od.getDescription (0).getDetailedDescription ()));
						ex.setEffects (exeff);
						available.Add (ex);
					}

            //if there is an action, we show them
					if (available.Count > 0) {
						Game.Instance.showActions (available, Input.mousePosition);
						ret = InteractuableResult.DOES_SOMETHING;
					}
					break;
				case Item.BehaviourType.ATREZZO:
				default:
					ret = InteractuableResult.IGNORES;
					break;
				}
			} else {
				if (drag == null) {
					foreach (Action action in objData.getActions()) {
						if (action.getType () == Action.DRAG_TO) {
							drag = action;
							break;
						}
					}
				}
				if (ConditionChecker.check (drag.getConditions ())) {
					dragging = true;
					ret = InteractuableResult.DOES_SOMETHING;
				}
			}
		}

        return ret;
    }

    public void Move(Vector2 position){
        this.transform.localPosition = new Vector3(position.x,position.y,this.transform.localPosition.z);
        this.context.setPosition ((int) position.x * 10, (int) (60 - position.y) * 10);
    }
}
