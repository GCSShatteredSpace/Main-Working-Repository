using UnityEngine;
using System.Collections;

public class weaponTile : MonoBehaviour {
	public Renderer rend;
	
	public int itemIndex;
	private bool chosen;

	public Color mouseOver;
	public Color chosenColor;
	public Color normalColor;
	public Color wunengColor;	//wuneng=impotent in chinese :P
	public bool valid;
	public bool mouseOn;

	public string weaponName;
	public string description;
	public weaponProperties stats;

	public GameObject controller;
	private menu mCtrl;
	public GameObject statsCtrl;
	public statsController dataBase;

	void Start() {
		rend = GetComponent<Renderer>();
		controller = GameObject.Find ("GameController(Clone)");
		mCtrl=this.gameObject.GetComponentInParent<menu>();
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
		rend.material.color = normalColor;
	}

	void OnMouseEnter() {
		mouseOn = true;
	}

	void OnMouseDown() {
		if (valid&&(mCtrl.current!=itemIndex)) {
			mCtrl.current=itemIndex;
			mCtrl.changeSelection();
		}
	}

	void OnMouseExit() {
		mouseOn = false;
	}
	
	void Update(){
		if (!valid) {
			rend.material.color = wunengColor;	
		}else if(mCtrl.current==itemIndex){
			rend.material.color = chosenColor;
		}else if(mouseOn){
			rend.material.color = mouseOver;
		}else{
			rend.material.color = normalColor;
		}
	}


}
