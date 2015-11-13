using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class displayer : MonoBehaviour {

	statsManager database;
	inputManager iManager;

	player myPlayer;
	int wpnID;

	[SerializeField] Text weaponName;
	[SerializeField] Text range;
	[SerializeField] Text damage;
	[SerializeField] Text description;

	Button buildBtn;

	void Start(){
		buildBtn = this.gameObject.GetComponentInChildren<Button> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
		iManager = GameObject.Find ("gameController").GetComponent<inputManager> ();
	}

	void Update(){
		if (myPlayer == null) {
			myPlayer = iManager.getMyPlayer ();
		}
	}
	
	public void display(int weaponID){
		wpnID = weaponID;
		weapon currWeapon = database.weapons [weaponID];

		weaponName.text = currWeapon.getName ().ToString();
		range.text = currWeapon.getRange ().ToString();
		damage.text = currWeapon.getDamage ().ToString ();
		description.text = currWeapon.getDescription ();

		if (myPlayer.hasWeapon (weaponID)) {
			buildBtn.interactable = false;
		} else {
			buildBtn.interactable = true;
		}
	}

	public void buildWeapon(){
		myPlayer.takeDamage (10);
		myPlayer.addWeapon (wpnID);
		buildBtn.interactable = false;
	}
}
