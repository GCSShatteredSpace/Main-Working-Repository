﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class inputManager : MonoBehaviour {

	functionManager SS;
	turnManager tManager;
	statsManager database;
	player myPlayer;
	[SerializeField] GameObject buildButton;

	bool myPlayerIsSet;

	public Stack<action> commands = new Stack<action>();
	int moveIndex;
	Vector2 playerPosition;	// Where players end up after each step
	Vector2 firePosition;	// Sometimes players fire at a position and move because of recoil
	Vector2 noAttack = new Vector2 (.5f, .5f);

	bool isTargeting;				// Click and drag to pick targets
	Vector3 targetStart;
	Vector3 targetEnd;
	[SerializeField]LineRenderer targetLine;
	int currentWeapon;
	bool commandable;
	int maxSteps;

	bool freezedTurn = false;
	float inputStartTime = 0;
	bool timerEnded = false;

	void Start () { 
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		tManager = GetComponent<turnManager> ();
		SS = GetComponent<functionManager> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();

		targetLine = GetComponent <LineRenderer> ();
		myPlayerIsSet = false;

		// Only temporary
		commandable = false;
		buildButton.SetActive (false);
		maxSteps = database.maxSteps;
		// Only temporary
	}

	public void setMyPlayer(player p){
		myPlayer = p;
		myPlayerIsSet = true;
	}

	void Update () {
		if (Input.GetKey ("space") && commandable) {
			// Temporary code
			sendCommands();
		}
	}
	public void sendCommands(){
		myPlayer.setActionSequence (commands);
		commandable = false;
		buildButton.SetActive (false);
	}

	public void resetCommands(Vector2 pos){
		playerPosition = pos;
		firePosition = playerPosition;
		moveIndex = 0;
		
		commands.Clear ();
		
		action newAction = new action();
		newAction.movement = playerPosition;
		newAction.attack = noAttack;
		newAction.weaponId = 0;
		
		commands.Push (newAction);
		
		targetEnd = noAttack;
		targetStart = playerPosition;
		targetLine.enabled = false;
		commandable = true;

		if (myPlayerIsSet) myPlayer.getWeapon ().setShotsPlanned (0);
	}

	public void startNewTurn(Vector2 pos){  // Reset after each turn
		resetCommands (pos);
		inputStartTime = Time.time;
		buildButton.SetActive (true);
		maxSteps = database.maxSteps;
		// Antibody grenade
		if (freezedTurn) {
			freezedTurn = false;
			myPlayer.isFreezed = false;
		}
		if (myPlayer.isFreezed) {
			maxSteps = 0;
			freezedTurn = true;
		}
	}

	public void moveCommand(Vector2 pos) {	// Add a movement step
		//Debug.Log("Command added!");
		moveIndex++;
		action newAction = new action ();
		newAction.attack = noAttack;
		newAction.movement = pos;
		playerPosition = pos;

		commands.Push (newAction);

		targetLine.enabled = false;
	}

	public void attackCommand(Vector2 pos){		// Choose the attack target for this step
		//Debug.Log("Attack!");
		action lastAction = commands.Pop();
		if (lastAction.attack == noAttack)
			myPlayer.getWeapon ().planToFire ();
		lastAction.attack = pos;
		lastAction.weaponId = currentWeapon;

		commands.Push(lastAction);

		targetStart = lastAction.movement;
		targetEnd = lastAction.attack;

		drawLine ();
	}

	public void cancelCommand(){
		cancelAttack ();
		if (commands.Count!=1){		// If it isn't step 0
			//print("Command removed!");
			moveIndex--;
			commands.Pop();
			//print(commands.Count);
			action lastAction = commands.Peek();
			playerPosition = lastAction.movement;
			firePosition = playerPosition - lastAction.extraMovement;
			// Set the target display
			targetLine.enabled=false;
			targetStart = firePosition;
			targetEnd = lastAction.attack;

			if (lastAction.attack!=new Vector2(.5f,.5f)){
				drawLine();
			}
		}
	}

	// This is separated from cancelCommand because of sevral reasons:
	// First is that you cannot cancel the very first movement, which is staying still
	// Second is that you might change weapon after you planned your attack. 
	// In which case you refresh your attack but keep your movement
	public void cancelAttack(){		
		
		action lastAction = commands.Pop();
		lastAction.movement = lastAction.movement - lastAction.extraMovement;
		lastAction.extraMovement=Vector2.zero;
		playerPosition = lastAction.movement;
		if (lastAction.attack != noAttack) {
			myPlayer.getWeapon ().cancelFire ();
		}

		lastAction.attack = noAttack;
		lastAction.weaponId = 0;
		commands.Push(lastAction);

		targetLine.enabled = false;

	}

	public void startTargeting(Vector2 pos){
		targetStart = pos;
		isTargeting = true;
		//Debug.Log("Start targeting!");
	}

	public void endTargeting(){
		isTargeting = false;
		//Debug.Log("End targeting!");
	}

	public bool inTarget(Vector2 pos){
		foreach (action element in commands){
			if(element.attack==pos)return true;
		}
		return false;
	}

	public bool inMovement(Vector2 pos){
		foreach (action element in commands){
			if(element.movement==pos)return true;
		}
		return false;
	}	

	void drawLine(){
		//Debug.Log("Draw line!");
		targetLine.enabled = true;
		targetLine.SetPosition (0, SS.hexPositionTransform(targetStart)+Vector3.back);
		targetLine.SetPosition (1, SS.hexPositionTransform(targetEnd)+Vector3.back);
	}

	public void gainTechnology(string technology){
		myPlayer.gainTechnology (technology);
	}

	// This temporarily sets the maxsteps in one turn
	// Can be used by recoil upgrade, antibody grenade and 
	public void setMaxSteps(int steps){
		maxSteps = steps;
	}
	
	// A bunch of "get" functions

	public bool isInTargetMode(){
		return isTargeting;
	}

	public bool isCommandable(){
		return commandable;
	}

	public player getMyPlayer(){
		if (myPlayerIsSet) {
			return myPlayer;
		} else {
			return null;
		}
	}

	// This is redundent
	public int getWeaponId(){
		return currentWeapon;
	}

	public bool hasMoveLeft(){
		return moveIndex<maxSteps;
	}

	public bool hasAttackLeft(){
		return myPlayer.getWeapon().readyToFire();
	}

	public Vector2 getPlayerPosition(){
		return playerPosition;
	}

	// Interaction with menus
	public void setMenu(bool value){
		if (value){
			commandable = false;
		}else{
			commandable = true;
		}
	}
	public float getTurnStartTime(){
		return inputStartTime;
	}
}
