using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class playerWpnMenu : MonoBehaviour {

	List<int> weaponIDs = new List<int>();
	List<playerWeaponBtn> buttons = new List<playerWeaponBtn>();
	[SerializeField] GameObject buttonObject;
	[SerializeField] Transform canvas;

	player myPlayer;
	bool playerIsSet= false;	// There must be better ways to do it but I only know the most straight forward one.

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

	public void setPlayerWeapon(){
		int weaponID = 0;
		for(int i=0; i < buttons.Count; i++) {
			if(buttons[i].isChosen()){
				weaponID = buttons[i].getWeaponID();
				break;
			}
		}
		if (playerIsSet)
			myPlayer.setWeapon (weaponID);
	}

	public void setMenuState(){
		// So actually the menu doesn't know which weapon is chosen
		// It just turns off everyone and let the button being clicked upon chose itself
		foreach (playerWeaponBtn button in buttons) {
			button.setChosen(false);
		}
	}

	public void setMyPlayer(player p){
		myPlayer = p;
		playerIsSet = true;
	}
}
