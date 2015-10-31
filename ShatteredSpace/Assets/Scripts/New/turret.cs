using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class turret : MonoBehaviour {
	
	statsManager dataBase;
	boardManager bManager;

	Vector2 turretPos;
	int energy;

	void Awake () {
		GameObject gameController = GameObject.Find ("gameController");
		dataBase = GameObject.Find ("stats").GetComponent<statsManager> ();
		bManager = gameController.GetComponent<boardManager> ();

		energy = dataBase.turretStartEnergy;
	}

	public void takeDamage(damageInfo damage){
		energy -= damage.damageAmount;
		if (energy <= 0)
			destroyTurret ();
	}

	public void setPos(Vector2 pos){
		turretPos = pos;
	}

	void destroyTurret(){
		Destroy(this.gameObject);
		bManager.destroyTurret (turretPos);
	}

}
