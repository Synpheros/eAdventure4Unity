using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecuenceManager {

	//##################################################
	//################# SINGLETON PART #################
	//##################################################

	private static SecuenceManager instance;
	public static SecuenceManager Instance {
		get {
			if(instance == null)
				instance = new SecuenceManager();
			return instance; }
	}

	//####################################################
	//################# SECUENCE MANAGER #################
	//####################################################

	public enum SMStates { NOTHING, RUNNING, WAITING};

	private SMStates state = SMStates.NOTHING;
	public SMStates State{ get { return state;} }
	private Stack<Secuence> secuences;
	private bool hasNewSecuences;

	private SecuenceManager(){
		this.state = SMStates.NOTHING;
		this.hasNewSecuences = false;
		this.secuences = new Stack<Secuence> ();
	}  

	public void Execute(Secuence secuence){
		this.secuences.Push (secuence);
		if (this.state == SMStates.NOTHING)
			Continue ();
		else
			this.hasNewSecuences = true;
	}

	public bool Continue(){
		bool wait = false;
		while (!wait || secuences.Count > 0) {
			Secuence current = secuences.Peek ();
			wait = current.execute ();
			if(!wait)
				secuences.Pop ();
			else
				this.state = SMStates.WAITING;
		}

		if (this.hasNewSecuences) {
			this.hasNewSecuences = false;
			wait = Continue ();
		}

		return wait;
	}
}