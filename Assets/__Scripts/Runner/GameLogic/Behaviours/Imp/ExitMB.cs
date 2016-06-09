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
		showHand(true);
	}

	void OnMouseExit() {
		showHand(false);
	}

	public void showHand(bool show){
		if (show)
			Game.Instance.setCursor ("over");
		else 
			Game.Instance.setCursor ("default");
	}

    public InteractuableResult Interacted (RaycastHit hit = new RaycastHit()){
        exit ();
        return InteractuableResult.DOES_SOMETHING;
    }
}
