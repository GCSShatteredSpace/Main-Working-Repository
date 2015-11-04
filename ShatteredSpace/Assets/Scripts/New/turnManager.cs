using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class turnManager : MonoBehaviour {

	[SerializeField]statsManager dataBase;
	[SerializeField]boardManager bManager;
	[SerializeField]inputManager iManager;
	public delegate void sendMessage(string MessageOverlay);
	public event sendMessage sendNetworkMessage;
	IEnumerator clockCoroutine;

	int readyPlayers;		// If it equals to num of players start turn
	[SerializeField]int finishedPlayers;	// If it equals to num of players end turn
	[SerializeField]int time;
	int turn;
	bool turnStarted;

	Vector2[] currentMovement = new Vector2[2];
	Vector2[] currentExtraMovement = new Vector2[2];
	bool[] readyForStep = new bool[2];
	[SerializeField] List<player> players;

	void Start(){
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		iManager = gameController.GetComponent<inputManager> ();
		time = -1;
		turn = 0;
		clockCoroutine = clock ();
		// A test of one of the most complicated situations
		/*
		Vector2[] testEndPos = new Vector2[2];
		Vector2[] testV = new Vector2[2];
		testEndPos[0] = new Vector2(0,0);
		testEndPos[1] = new Vector2(0,0);
		testV[0] = new Vector2(1,0);
		testV[1] = new Vector2(0,1);
		List<Vector2>[] result = new List<Vector2>[2];
		result = calculateCollision(testEndPos,testV);
		for (int i=0; i<2; i++) {
			print ("Player"+i.ToString());
			for (int j=0;j<result[i].Count;j++){
				print (result[i][j]);
			}
		}
		*/
	}

	void Update(){
		if ((readyPlayers == PhotonNetwork.playerList.Length) && !turnStarted) {
			turn+=1;
			turnStarted=true;
			startTurn();
		}
	}

	public void getReady(){ 
		readyPlayers += 1;
	}
	
	int getReadyPlayers(){	// Let's keep it an int for now
		return readyPlayers;
	}

	void startTurn(){
		print ("Turn starts!");
		StartCoroutine(clock());
	}

	// In-game time!
	IEnumerator clock(){
		print ("clock active!");
		while (turnStarted) {
			// The order is curcial!!
			yield return new WaitForSeconds (dataBase.stepTime);
			time += 1;
			print ("Official Time = " + time.ToString ());
		}
		time = -1;
		print ("clock off!");
	}
	
	void endTurn(){
		print ("end turn!");
		// Note the difference between start and stop coroutine!!
		StopCoroutine ("clock");
		// Clear
		resetTurn ();
	}

	void resetTurn(){
		print ("Turn reset!");
		endCurrentStep ();
		readyPlayers = 0;
		finishedPlayers = 0;
		turnStarted = false;
		iManager.startNewTurn (players[0].getPosition ());
		time = -1;
	}

	public void finishAction (int playerId){
		finishedPlayers++;
		if (finishedPlayers == PhotonNetwork.playerList.Length) endTurn();
	}
	
	public int getTime(){
		return time;
	}
	
	public int getTurn(){
		return turn;
	}

	public void addPlayer(player p){
		players.Add (p);
	}

	// The player tells turnManager the next move
	// When both players have done so, turnManager calculates what will happen in the 
	public void attemptToMove(Vector2 movement, Vector2 extraMovement, int playerId){	// Dummy:1 myPlayer:0
		print ("Attempt to move!");
		currentMovement [playerId] = movement;
		currentExtraMovement [playerId] = extraMovement;
		readyForStep [playerId] = true;
		print ("Number of Players: " + PhotonNetwork.playerList.Length);
		if (PhotonNetwork.playerList.Length == 1) {
			demoCalculateStepSequence();
		}
		if (readyForStep [0] && readyForStep [1]) {
			calculateStepSequence();
		}
	}

	//For tesing purposes only!
	void demoCalculateStepSequence(){
		print ("Demo calculate step sequence!");
		Vector2 nextPos = currentMovement [0] + players [0].getPosition();
		List<Vector2> result = new List<Vector2> ();
		if (bManager.isOccupied (nextPos)) {
			result.Add (currentMovement[0] / 2);
			result.Add (-currentMovement[0] / 2);
			StartCoroutine(players [0].moveStep (result));
		} else {
			result.Add(currentMovement[0]);
			StartCoroutine(players [0].moveStep (result));
		}
		endCurrentStep ();
	}
	
	void calculateStepSequence(){
		print ("Calculate step sequence!");
		List<Vector2>[] velocitySequences= new List<Vector2>[2];
		Vector2[] positions = new Vector2[2];
		Vector2[] newPos = new Vector2[2];
		for (int i=0; i<2; i++) {	
			positions[i] = players[i].getPosition ();
			newPos[i] = positions[i] + currentMovement[i];
		}
		velocitySequences = calculateCollision(newPos,currentMovement);
		StartCoroutine(players[0].moveStep (velocitySequences[0]));
		StartCoroutine(players[1].moveStep (velocitySequences[1]));
		endCurrentStep ();
	}

	// Calculate collision sequence recurrsively
	// Impossible to debug!
	// So it better works!
	List<Vector2>[] calculateCollision(Vector2[] endPos,Vector2[] v){
		List<Vector2>[] result = new List<Vector2>[2];

		// Look for map collision
		for (int i=0; i<2; i++) {
			if (bManager.isOccupied (endPos [i])){
				print ("Map collision!");
				return calculateMapCollision(endPos,v);
			}
		}
		// Look for player collision
		if (endPos [0] != endPos [1] &&
		// See if it's the tricky special case where players swap positions
			!(endPos [0] == endPos [1] - v [1] && endPos [1] == endPos [0] - v [0])) {
			result [0] = new List<Vector2>();
			result [1] = new List<Vector2>();
			result [0].Add (v [0]);
			result [1].Add (v [1]);
			return result;
		} else {
			print ("Player collision!");
			return calculatePlayerCollision(endPos,v);
		}
	}

	List<Vector2>[] calculateMapCollision(Vector2[] endPos,Vector2[] v){
		List<Vector2>[] result = new List<Vector2>[2];
		Vector2[] newV = new Vector2[2];
		Vector2[] nextEndPos = new Vector2[2];
		List<Vector2>[] nextResult = new List<Vector2>[2];

		// Examine collision with map elements
		for (int i=0; i<2; i++) {
			result[i]= new List<Vector2>();
			if (bManager.isOccupied (endPos [i])) {
				newV [i] = -v [i];
				// Going half the distance before collision
				result [i].Add (v [i]/2);
				// Prepare for recursion
				nextEndPos [i] = endPos [i] + newV [i];
			}else{
				nextEndPos [i] = endPos [i];
				newV[i] = Vector2.zero;
			}
		}
		nextResult = calculateCollision(nextEndPos,newV);
		// Correct the first element
		result[0].Add(v[0]/2+nextResult[0][0]);
		result[1].Add(v[1]/2+nextResult[1][0]);
		nextResult[0].RemoveAt(0);
		nextResult[1].RemoveAt(0);
		// Merge the lists
		result = appendLists(result,nextResult);
		return result;
	}

	List<Vector2>[] calculatePlayerCollision(Vector2[] endPos,Vector2[] v){
		List<Vector2>[] result = new List<Vector2>[2];
		List<Vector2>[] nextResult = new List<Vector2>[2];
		Vector2[] newV = new Vector2[2];
		Vector2[] nextEndPos = new Vector2[2];
		newV [1] = v [0];newV [0] = v [1];
		for (int i=0; i<2; i++) {
			result[i] = new List<Vector2>();
			// Going half the distance before collision
			result [i].Add (v [i] / 2);
			// Prepare for recursion
			nextEndPos [i] = endPos [i] + newV [i];
		}
		nextResult = calculateCollision(nextEndPos,newV);
		// Correct the first element
		result [0].Add (v [0] / 2 + nextResult [0] [0]);result [1].Add (v [1] / 2 + nextResult [1] [0]);
		nextResult [0].RemoveAt (0);nextResult [1].RemoveAt (0);
		// Merge the lists
		result = appendLists (result, nextResult);
		return result;
	}

	List<Vector2>[] appendLists(List<Vector2>[] l1, List<Vector2>[] l2){
		List<Vector2>[] result = new List<Vector2>[2];
		result [0] = l1 [0];result [1] = l1 [1];
		for (int i=0; i<2; i++) {
			for (int j=0; j<l2[i].Count; j++) {
				result [i].Add (l2 [i] [j]);
			}
		}
		return result;
	}

	void endCurrentStep (){
		for (int i=0; i<2; i++) {
			readyForStep [i] = false;
			currentMovement [i] = Vector2.zero;
			currentExtraMovement [i] = Vector2.zero;
		}
	}
}
