﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerWeaponBtn : MonoBehaviour {

	[SerializeField] Color chosenColor;
	[SerializeField] Color normalColor;
	[SerializeField] Color chosenMouseOver;
	[SerializeField] Color normalMouseOver;

	[SerializeField] statsManager database;

	[SerializeField] int weaponID;

	[SerializeField] Button button;
	[SerializeField] Text btnText;

	[SerializeField] bool chosen = false;

	void Start(){
//		button = this.gameObject.GetComponent<Button> ();
//		btnText = this.gameObject.GetComponentInChildren<Text> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
	}

	public void setChosen(bool value){
		chosen = value;
		display ();
	}

	public void setWpnID(int wpnID){
		weaponID = wpnID;
		btnText.text = database.weapons [wpnID].getName ();
	}

	void display(){
		ColorBlock btnColors = button.colors;
		if (chosen) {
			btnColors.normalColor = chosenColor;
			btnColors.highlightedColor = chosenColor;	// This looks better
			button.colors = btnColors;
		} else {
			btnColors.normalColor = normalColor;
			btnColors.highlightedColor = normalMouseOver;
			button.colors = btnColors;
		}
	}

	public bool isChosen(){
		return chosen;
	}

	public int getWeaponID(){
		return weaponID;
	}
}
