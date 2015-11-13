using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class menu : MonoBehaviour {

	statsManager database;
	menuButton[] buttons;

	void Start () {
		buttons = this.gameObject.GetComponentsInChildren<menuButton> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void display(List<int> weaponIDList){
		for (int i = 0; i < weaponIDList.Count; i++){
			buttons[i].showText(weaponIDList[i]);
		}
	}
}
