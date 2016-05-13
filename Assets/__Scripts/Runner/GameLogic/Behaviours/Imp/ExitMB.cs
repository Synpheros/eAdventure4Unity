using UnityEngine;
using System.Collections;

public class ExitMB : MonoBehaviour, Interactuable {

    private Exit ed;
    public Exit exitData{
		get { return ed; }
		set { ed = value; }
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void exit(){
		//Game.Instance.hideMenu ();
        if (ConditionChecker.check (ed.getConditions ())) {
            Game.Instance.Execute (new EffectHolder (ed.getEffects ()));
            Game.Instance.setCursor ("default");
            Game.Instance.renderScene (ed.getNextSceneId (), ed.getTransitionTime (), ed.getTransitionType ());

            if (ed.getPostEffects () != null)
                Game.Instance.Execute (new EffectHolder (ed.getPostEffects ()));
        } else if (ed.isHasNotEffects ())
            Game.Instance.Execute (new EffectHolder (ed.getNotEffects ()));
	}

	void OnMouseEnter(){
		Game.Instance.setCursor ("over");
		//GetComponent<Renderer> ().material.color = new Color (1f, 0f, 0f, 0.2f);
	}
	
	void OnMouseExit() {
		Game.Instance.setCursor ("default");
		//GetComponent<Renderer> ().material.color = new Color (1f, 0f, 0f, 0f);
	}

    public InteractuableResult Interacted (RaycastHit hit = new RaycastHit()){
        exit ();
        return InteractuableResult.DOES_SOMETHING;
    }
}
