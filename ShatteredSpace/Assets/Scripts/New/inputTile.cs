using UnityEngine;
//using UnityEngine.GUI;
using System.Collections;

public class inputTile : MonoBehaviour {
	Renderer rend;
	[SerializeField] Texture2D warning;

	float warningSize = 35f;
	const int warningRatio = 60;
	const float vertOffset = 3f;

	Vector2 playerPosition;
	public Vector2 tilePosition = new Vector2();

	bool chosen;	// The tile is in movement path
	bool current;
	bool inTarget;
	bool rightHold;
	bool isValidTarget;
	bool hasDamage;
	bool dangerous;
	[SerializeField] bool valid;

	[SerializeField] Color mouseOverColor;
	[SerializeField] Color chosenColor;
	[SerializeField] Color attackColor;
	[SerializeField] Color invalidColor;
	[SerializeField] Color dangerColor;
	[SerializeField] Color damageColor;
	[SerializeField] Color jumpColor;
	
	[SerializeField] bool mouseOn;

	inputManager iManager;
	boardManager bManager;
	statsManager dataBase;
	functionManager SS;
	
	int weapon;
	player myPlayer;

	void Start() {
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		iManager = gameController.GetComponent<inputManager> ();
		bManager = gameController.GetComponent<boardManager> ();
		SS = gameController.GetComponent<functionManager> ();
		dataBase = gameController.GetComponent<statsManager> ();

		rend = GetComponent<Renderer>();
	}

	void OnMouseEnter() {
		if (!iManager.isCommandable())
						return;
		if (iManager.isInTargetMode() && isValidTarget) {     // Target selection mode
	 		iManager.attackCommand(tilePosition);// Everytime the mouse enters another tile the attack position is refreshed
		}
		if (!iManager.isInTargetMode()){
			mouseOn = true;
		}else{mouseOn = false;}
	}
	
	void OnMouseExit() {  // Clears the state on previous tile
		mouseOn = false;
		rightHold = false;		
	}

	void OnMouseOver(){
		if (!iManager.isCommandable())
			return;
		if (Input.GetButton ("Fire2") && (current)) {   // Detect right MouseUp event manually
			rightHold = true;
		} else {
			if(rightHold){
				iManager.cancelCommand();   // Right click cancels one command at a time
				rightHold = false;
			}
		}
	}

	void OnMouseDown() {
		if (!iManager.isCommandable())
			return;
		if (current && iManager.hasAttackLeft()) {
			//int wpnId = iManager.getWeaponId();
			//weapon wpn = myPlayer.getWeapon(wpnId);
			//if(wpn.readyToFire()){
				iManager.startTargeting(tilePosition);					// Enters targeting mode
			//}
		}
	}

  // The MouseUp event only works on left mouse button
	void OnMouseUp(){     // Mouseup implies a Mousedown action, which means the player clicked on the tile
		iManager.endTargeting();
		if (valid && mouseOn) {   // MouseUp always trigger on the tile where MouseDown happens!
			iManager.moveCommand(tilePosition);   // Left mouse click choses a new movement step
		}
	}

	void Update(){ 								//this part is what determines the display on each tile

		playerPosition = iManager.getPlayerPosition();	
			//in the case of weapons with recoil effect, fireposition!=playerposition

	    setState();
	    setAppearance();
	}
	
	void setState(){  // Sets the states of the current tile
	  	if (SS.isNear(playerPosition, tilePosition) && iManager.hasMoveLeft()) {
			valid = true;
		} else {
			valid = false;
		}
		// Check if the tile is in movement path
		if (tilePosition == playerPosition){  
			current = true;
			chosen = true;
		}else{
			current=false;
			if(iManager.inMovement(tilePosition)){ // Mark the trail with mouseover color
				chosen = true;
			}else{
				chosen = false;
			}
		}

		if (iManager.inTarget (tilePosition)) {
			inTarget = true;
		} else {
			inTarget=false;
		}

		if (bManager.getTile (tilePosition).hasDamage()) {
			hasDamage = true;
		} else {
			hasDamage = false;
		}

		// If the player chose to stay here, s/he is in trouble
		if (current && bManager.isDangerous (tilePosition)) {
			// Let's unenable it for now because it gets in the way of our awesome menu
			//dangerous = true;
		} else {
			dangerous = false;
		}

		if (iManager.isInTargetMode()) {
		  int wpnId = iManager.getWeaponId();
		  myPlayer = iManager.getMyPlayer();
		  weapon wpn = myPlayer.getWeapon();
		  if (!wpn.isInRange(SS.getDistance(tilePosition,playerPosition))
		  	|| bManager.isBlocked(playerPosition,tilePosition)){
		    isValidTarget = false;
		  }else{
		  	isValidTarget = true;
		  }
		}

	}
	
	void setAppearance(){
		clear ();
	  	if (inTarget && iManager.isCommandable ()) {
			setTarget ();
		}
		if (hasDamage) {
			setDamage();
		}
		if (current) {
			setCurrent();
		}else if(chosen||(mouseOn&&valid)){
			setMouseOver();
		}

		if (dangerous) {
			//setDangerous();
		}
		// In target selection mode
		if (iManager.isInTargetMode() && !isValidTarget) {
		    setInvalid();
		}
	}

	// The actual part where the appearance of tile is changed
	// We can do something fancy here in the future

	void setTarget(){
		rend.material.color = attackColor;
	}

	void setCurrent(){
		rend.material.color = chosenColor;
	}

	void setMouseOver(){
		rend.material.color = mouseOverColor;
	}

	void setInvalid(){
		rend.material.color = invalidColor;
	}

	void setDamage(){
		rend.material.color = damageColor;
	}

	void OnGUI(){
		if (!iManager.isCommandable()) {
			return;
		}
		if (dangerous) {
			Vector3 center = SS.hexPositionTransform (tilePosition);
			Rect bounds = new Rect (Screen.width/2 + center.x*warningRatio - warningSize / 2, 
			                        Screen.height/2 - center.y*warningRatio - warningSize / 2 - vertOffset,
			                        warningSize, warningSize);

			GUI.DrawTexture (bounds, warning);
		}
	}

	void clear(){
		rend.material.color = Color.white;
	}
}
