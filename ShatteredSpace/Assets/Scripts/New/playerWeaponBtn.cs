using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerWeaponBtn : MonoBehaviour {

	[SerializeField] Color chosenColor;
	[SerializeField] Color normalColor;
	[SerializeField] Color overheatColor;
	[SerializeField] Color overheatMouseOver;
	[SerializeField] Color overheatPressed;
	[SerializeField] Color chosenMouseOver;
	[SerializeField] Color normalMouseOver;
	[SerializeField] Color normalPressed;
	[SerializeField] Color textColor;

	[SerializeField] statsManager database;

	[SerializeField] int weaponID;

	[SerializeField] Button button;
	[SerializeField] Text btnText;

	[SerializeField] bool chosen = false;

	public player myPlayer;
	bool playerIsSet = false;

	void Start(){
//		button = this.gameObject.GetComponent<Button> ();
//		btnText = this.gameObject.GetComponentInChildren<Text> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
	}

	void Update(){
		display ();
	}

	public void setChosen(bool value){
		chosen = value;
		if (value) {
			this.gameObject.GetComponentInParent<playerWpnMenu>().setPlayerWeapon();
		}
		display ();
	}

	public void setWpnID(int wpnID){
		weaponID = wpnID;
		btnText.text = "???";
	}

	void display(){
		ColorBlock btnColors = button.colors;
		if (playerIsSet) {
			if (myPlayer.getWeapon(weaponID).overheated){
				// Set color to awesome red color
				btnColors.normalColor = overheatColor;
				btnColors.highlightedColor = overheatMouseOver;
				btnColors.pressedColor = overheatPressed;
				button.colors = btnColors;
				btnText.color = Color.white;
				return;
			}		
		}
		btnText.color = textColor;
		btnColors.pressedColor = normalPressed;
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

	public void reveal(){
		btnText.text = database.weapons [weaponID].getName ();
	}

	public void setMyPlayer(player p){
		myPlayer = p;
		playerIsSet = true;
	}

}
