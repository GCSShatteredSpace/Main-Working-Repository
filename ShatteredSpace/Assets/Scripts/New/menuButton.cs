using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class menuButton : MonoBehaviour {

	[SerializeField] GameObject children;
	statsManager database;

	public List<int> weaponIDList;	// This is for top-level menu
	[SerializeField] int weaponID;	// This is for the submenu
	Text btnText;

	void Start () {
		btnText = this.gameObject.GetComponentInChildren<Text> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
	}

	public void showText(int wpnID){
		weapon currWeapon = database.weapons [wpnID];
		btnText.text = currWeapon.getName();
		weaponID = wpnID;
	}
	
	public void btnClick(){
		menu next = children.GetComponent<menu> ();
		if (next != null) {
			//print("Display!Plz!");
			next.display (weaponIDList);
		} else {
			displayer disp = children.GetComponent<displayer> ();
			//print("DisplayWeapon!Plz!");
			disp.display (weaponID);
		}
	}
}
