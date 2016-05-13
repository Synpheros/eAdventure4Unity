using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveAreaMB : MonoBehaviour, Interactuable {

    private ActiveArea aad;
    public ActiveArea aaData{
		get { return aad; }
		set { aad = value; }
	}

	// Use this for initialization
	void Start () {
        adaptate ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseEnter(){
		//GetComponent<Renderer> ().material.color = new Color (0f, 1f, 0f, 0.2f);
        showHand(true);
	}
	
	void OnMouseExit() {
		//GetComponent<Renderer> ().material.color = new Color (1f, 0f, 0f, 0f);
        showHand(false);
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

    public InteractuableResult Interacted (RaycastHit hit = new RaycastHit()){
        InteractuableResult ret = InteractuableResult.IGNORES;

        if (aad.getInfluenceArea () != null) {

        }

        switch(aad.getBehaviour()) {
        case Item.BehaviourType.FIRST_ACTION:
            foreach (Action a in aad.getActions()) {
                if (ConditionChecker.check (a.getConditions ())) {
                    Game.Instance.Execute (new EffectHolder(a.getEffects()));
                    break;
                }
            }
            ret = InteractuableResult.DOES_SOMETHING;
            break;
        case Item.BehaviourType.NORMAL:
            List<Action> available = new List<Action> ();
            foreach (Action a in aad.getActions()) {
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
            string desc = aad.getDescription (0).getDetailedDescription ();
            if (desc != "") {
                foreach (Action a in available) {
                    if (a.getType () == Action.EXAMINE) {
                        addexamine = false;
                        break;
                    }
                }

                if (addexamine) {
                    Action ex = new Action (Action.EXAMINE);
                    Effects exeff = new Effects ();
                    exeff.add (new SpeakPlayerEffect (desc));
                    ex.setEffects (exeff);
                    available.Add (ex);
                }
            }

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

        return ret;
    }


    private bool mouseInInfluenceArea(InfluenceArea area){
        return true;
    }

    private void adaptate(){
        if (!this.aad.isRectangular() && this.aad.getInfluenceArea () != null) {
            Mesh mesh = new Mesh ();
            List<Vector3> vertices = new List<Vector3> ();

            foreach (Vector2 v in this.aad.getPoints()) {
                vertices.Add (new Vector3 (v.x / 100f, v.y / 100f, this.transform.position.z));
            }

            mesh.SetVertices (vertices);

            List<int> triangles = new List<int> ();
            for (int i = 2; i < aad.getPoints ().Count; i++) {
                triangles.AddRange(new int[] { 0, i - 1, i });
            }

            mesh.SetTriangles (triangles,0);

            this.GetComponent<MeshFilter> ().mesh = mesh;
        }
    }
}
