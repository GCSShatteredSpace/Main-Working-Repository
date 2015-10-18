using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//the "Build weapon" button

public class shopButton : MonoBehaviour {
	public Renderer rend;

	public Vector2 playerPosition;
	public Vector2 tilePosition;
	private bool chosen;
	public bool current;
	public Color mouseOver;
	public Color chosenColor;
	public Color wunengColor;
	public bool valid;
	public bool mouseon;
	public bool mouseDown;

	public GameObject controller;
	public GameObject statsCtrl;
	public statsController dataBase;
	public GameObject currentWeapons;
	public menu playerMenu;

	public int weaponId;

	void Start() {
		rend = GetComponent<Renderer>();
		controller = GameObject.Find ("GameController(Clone)");
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
		playerPosition = new Vector2 (0, 0);
		valid = true;
		mouseon = false;
	}

	void OnMouseOver(){
		mouseon = true;
	}

	void OnMouseUp() {
		if (valid&&(!current)&&mouseon) {
			build();
		}
		mouseDown = false;
	}

	void OnMouseDown() {
		if (valid) {
			mouseDown = true;
			rend.material.color = chosenColor;
		}
	}

	void OnMouseExit() {
		mouseon = false;
		mouseDown = false;
	}

	void Update(){ 
		if (playerMenu.weapons.Find((x) => { return x == weaponId; })==weaponId){
			current = true;
		} else {
			current = false;
		}
		if (!valid) {
			rend.material.color = wunengColor;	
		}else if(current||mouseDown){
			rend.material.color = chosenColor;
		}else if(mouseon){
			rend.material.color = mouseOver;
		}else{
			rend.material.color = Color.white;
		}
	}
		
	void build(){
		playerMenu.addItem (weaponId);
	}
}
