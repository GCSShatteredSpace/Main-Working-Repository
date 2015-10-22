using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Absolutly disgusting code!!!!
//Basically this one class can act as different kind of menus all at once
//Somehow when I wrote this it was the only way of getting things done
//I think we can do much better using a menu superclass and subclasses

public class menu : MonoBehaviour {
	public int itemCount;
	public GameObject tile;
	public List<GameObject> items;
	public List<int> weapons;
	public int current;

	public GameObject controller;
	private GameController gctrl;
	public GameObject statsCtrl;
	public statsController dataBase;

	public float itemHeight;

	public bool busy;
	public string kind;

	public menu sub;
	public displayer description;
	public GameObject subMenuObject;

	void Start () {
		controller = GameObject.Find ("GameController(Clone)");
		gctrl=controller.GetComponent<GameController>();
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
		if (kind == "armory") {
			armory ();
		}else if (kind == "sub") {
			subMenu(1);		
		}else if (kind=="player"){
			playerMenu();
		}
	}
	
	public void addItem(int weaponId){
		if (current == 0)
						current = 1;
		GameObject temp = Instantiate (tile, new Vector3 (transform.position.x, transform.position.y-itemHeight*itemCount, transform.position.z), Quaternion.identity)as GameObject;
		TextMesh text = temp.GetComponentInChildren<TextMesh> ();
		text.text = dataBase.weapons [weaponId].weaponName;
		items.Insert (itemCount,temp);
		weapons.Insert (itemCount, weaponId);
		temp.transform.SetParent (transform);
		itemCount++;
		weaponTile menuItem = temp.GetComponent<weaponTile> ();
		menuItem.itemIndex = itemCount;
		menuItem.valid = true;
	}

	public void clearItem(){
		int i;
		for (i=0; i<items.Count; i++) {
			Destroy(items[i].gameObject);		
		}
		items.Clear();
		itemCount = 0;
	}

	void armory(){
		int i;
		for (i=1; i<=dataBase.techCount; i++) {
			addItem (0);
		}
		sub = subMenuObject.GetComponent<menu> ();
		sub.subMenu(current);
	} 

	void subMenu(int currentTech){
		clearItem ();
		int i;
		for (i=(currentTech-1)*dataBase.weaponPerTech+1; i<=currentTech*dataBase.weaponPerTech; i++) {
			addItem (i);
		}
		description = subMenuObject.GetComponent<displayer> ();
		description.display(weapons[current-1]);
	}

	void playerMenu (){
		addItem (1);
		addItem (2);
		gctrl.currentWeapon = 1;
	}

	public void select(int weaponId){
		for(int i=0;i<weapons.Count;i++){
			if (weapons[i]==weaponId){
				current=i+1;
				break;
			}
		}
	}

	public void changeSelection(){
		if (kind == "armory") {
			sub.subMenu(current);
		} else if (kind == "sub") {
			description.display(weapons[current-1]);
		}else if (kind =="player"){
			gctrl.currentWeapon = weapons[current-1];
			gctrl.clearAttack();
		}
	}
}
