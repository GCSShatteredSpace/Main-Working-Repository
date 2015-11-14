using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class turret : MonoBehaviour {

	public static int TURRET_DAMAGE = 0;

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

	public damageInfo getDamage(){
		damageInfo d = new damageInfo ();
		d.damageAmount = TURRET_DAMAGE;
		return d;
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
