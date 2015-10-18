using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	private Renderer rend;

	private Vector2 firePosition;
	private Vector2 playerPosition;
	public Vector2 tilePosition;

	private bool chosen;
	private bool current;
	private bool currenttgt;
	private bool rightHold;

	[SerializeField] Color mouseOver;
	[SerializeField] Color chosenColor;
	[SerializeField] Color attackColor;	
	[SerializeField] Color wunengColor;	//wuneng is chinese for invalid
	[SerializeField] Color jumpColor;

	[SerializeField] bool valid;
	[SerializeField] bool shootable;
	[SerializeField] bool mouseon;

	public GameObject controller;
	[SerializeField] GameController gctrl;
	public GameObject statsCtrl;
	[SerializeField] statsController dataBase;

	private string sp;
	private string temp;

	void Start() {

		rend = GetComponent<Renderer>();

		//establish connection with other scripts

		controller = GameObject.Find ("GameController(Clone)");
		gctrl=controller.GetComponent<GameController>();
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
		playerPosition = gctrl.playerPosition;//they should be in sync at all times
	}

	void OnMouseEnter() {
		if (!gctrl.commandable)
						return;
		if (gctrl.hold && shootable) {			//if in targeting mode the current target becomes red
			gctrl.targetEnd = tilePosition;
	 		gctrl.attackCommand(tilePosition);
			rend.material.color = attackColor;
		}
		mouseon = true;
	}

	void OnMouseOver(){
		if (!gctrl.commandable)
			return;
		if (Input.GetButton ("Fire2") && (current || tilePosition == firePosition)) {
			rightHold = true;
		} else {
			if(rightHold){
				gctrl.cancelCommand ();
				rightHold = false;
			}
		}
	}

	void OnMouseDown() {
		if (!gctrl.commandable)
			return;

		if (firePosition==tilePosition) {
			gctrl.hold = true;					//enter targeting mode
			gctrl.targetStart = tilePosition;		
		}
	}

	void OnMouseExit() {
		mouseon = false;
		rightHold = false;		
	}

	void OnMouseUp(){
		gctrl.hold = false;
		print(tilePosition);
		if (valid && mouseon) {

			gctrl.moveCommand(tilePosition);
		}
	}

	void Update(){ 								//this part is waht determines the display on each tile
		playerPosition = gctrl.playerPosition;	
		firePosition = gctrl.firePosition;		//in the case of weapons with recoil effect, fireposition!=playerposition

		if (near (playerPosition, tilePosition)&&gctrl.moveindex<gctrl.maxSteps) {
			valid = true;
		} else {
			valid = false;
		}

		if (tilePosition == playerPosition){   //current is prioritised over other colors 
			current = true;
			chosen = true;
		}else{
			current=false;
			if(gctrl.playerMovement.Contains(tilePosition)){ //mark the trail with mouseover color
				chosen = true;
			}else{
				chosen = false;
			}
		}

		if (!gctrl.commandable) {
			mouseon=false;
		}

		if (gctrl.playerAttack.Contains(tilePosition)) {
			rend.material.color = attackColor;
		}else if (current) {
			rend.material.color = chosenColor;
		}else if(chosen||(mouseon&&valid)){
				rend.material.color = mouseOver;
		}else if(firePosition==tilePosition){
				rend.material.color = jumpColor;
		}else{
				rend.material.color = Color.white;
		}

		//some special cases of targeting

		sp=dataBase.weapons[gctrl.currentWeapon].special;
		if (gctrl.hold) {
			if(distance(firePosition,tilePosition)>dataBase.weapons[gctrl.currentWeapon].range
			   ||distance(firePosition,tilePosition)==0){
				rend.material.color = wunengColor;
				shootable = false;
			}else{
				//look for weapons that have thrust effects
				temp = gctrl.getSp(sp,"move");
				shootable = true;
				if (temp!=null && mouseon){
					int amount = int.Parse(temp.Substring(0,temp.IndexOf(";"))); //get the thrust amount(it's usually 1)
					if(align(firePosition,tilePosition)){
						Vector2 difference = normal(tilePosition-firePosition) ;
						Vector2 newPosition;
						newPosition = firePosition + difference*-amount;
						gctrl.extraMovement[gctrl.moveindex]=difference*-amount;
						if (newPosition!=playerPosition){
							gctrl.playerMovement[gctrl.playerMovement.Count-1]=newPosition;
							gctrl.playerPosition = newPosition;
						}
					}else{
						gctrl.extraMovement[gctrl.moveindex]=Vector2.zero;
						gctrl.playerMovement[gctrl.playerMovement.Count-1] = firePosition;
						gctrl.playerPosition = firePosition;
					}
				}
			}


			//look for specially ranged weapons
				temp = gctrl.getSp(sp,">");
				if (temp!=null){
					int amount = int.Parse(temp.Substring(0,temp.IndexOf(";")));
					if(distance(firePosition,tilePosition)<amount){
						rend.material.color = wunengColor;
						shootable = false;
					}
				}
		
		}
	}

	bool near(Vector2 v1,Vector2 v2){
		if(Mathf.Abs(Vector2.Distance(v1,v2))==1 || v1==v2+new Vector2(1,-1)||v2==v1+new Vector2(1,-1)){
			return true;
		}else{
			return false;
		}
	}

	float distance(Vector2 v1, Vector2 v2){
		float x;
		float y;
		if (v1.x > v2.x) {
			x=v1.x-v2.x;
			y=v1.y-v2.y;
		}else{
			x=v2.x-v1.x;
			y=v2.y-v1.y;
		}
		if (y>=0) {
			return x+y; 		
		}else if(x>=-y){
			return x;
		}else{
			return -y;
		}
	}

	bool align(Vector2 v1,Vector2 v2){
		Vector2 v0 = v1 - v2;
		if(v0.x==0||v0.y==0||v0.x==-v0.y){
			return true;
		}else{
			return false;
		}
	}

	Vector2 normal(Vector2 v){ //return the identity vector along the 6 directions
		int x, y;
		if (v.x == 0) {
			x = 0;		
		} else if (v.x > 0) {
			x = 1;
		} else {
			x=-1;		
		}
		if (v.y == 0) {
			y = 0;		
		} else if (v.y > 0) {
			y = 1;
		} else {
			y =-1;		
		}
		return new Vector2 (x, y);
	}
}
