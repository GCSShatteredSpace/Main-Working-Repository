using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class turret : MonoBehaviour {

	public Vector2 turretPosition;
	public Vector2 playerPosition;
	public GameObject gameController;
	private actionController actrl;
	public GameObject actionController;

	public GameObject statsCtrl;
	public statsController dataBase;
	
	public weaponProperties hitWeapon;
	public int moveindex;
	public int maxAction;
	public Vector3 positionOffset;
	public Vector2[,] toInstantiate;
	public weaponProperties[,] weapons;
	public List<int> attackWeapon = new List <int> ();

	
	public int energy;
	public int exp;

	void Start () {
		gameController = GameObject.Find ("GameController(Clone)");
		actionController = GameObject.Find ("actionController");
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
		actrl = actionController.GetComponent<actionController> ();
		energy = dataBase.turretHealth;
	}
	
	void OnTriggerEnter(Collider other) {
		GameObject otherObject = other.gameObject;
		if (otherObject.tag == "damage") {
			hitWeapon = otherObject.GetComponent<weaponProperties> ();
			energy = energy - hitWeapon.damage;
			if (energy<=0){
				actrl.destoryTurret(turretPosition);
				Destroy (this.gameObject);
			}
		}
	}
}
