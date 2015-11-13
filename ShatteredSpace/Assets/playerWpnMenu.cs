﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class playerWpnMenu : MonoBehaviour {

	List<int> weaponIDs = new List<int>();
	List<playerWeaponBtn> buttons = new List<playerWeaponBtn>();
	[SerializeField] GameObject buttonObject;
	[SerializeField] Transform canvas;

	// The positions for generating a new button
	const int spacing = 50;
	int posY = -50 ;


	void Start () {
		weaponIDs.Add (0);
		buttons.Add(this.gameObject.GetComponentInChildren<playerWeaponBtn>());
	
		buttons [0].setChosen (true);
		buttonObject = buttons [0].gameObject;
	}

	public void addWeapon(int wpnID){
		GameObject newBtnObject = Instantiate (buttonObject) as GameObject;
		newBtnObject.transform.SetParent (canvas);
		RectTransform pos = newBtnObject.GetComponent<RectTransform> ();
		playerWeaponBtn button = newBtnObject.GetComponent<playerWeaponBtn> ();

		pos.localPosition = new Vector3 (0, posY, 0);
		posY -= spacing;
		button.setChosen (false);

		weaponIDs.Add (wpnID);
		buttons.Add (button);
		button.setWpnID (wpnID);
	}

	public int getBtnID(){
		for(int i=0; i < buttons.Count; i++) {
			if(buttons[i].isChosen()){
				return i;
			}
		}
		Debug.Log ("No weapon selected!?");
		return 0;
	}

	public void setMenuState(){
		// So actually the menu doesn't know which weapon is chosen
		// It just turns off everyone and let the button being clicked upon chose itself
		foreach (playerWeaponBtn button in buttons) {
			button.setChosen(false);
		}
	}


}
