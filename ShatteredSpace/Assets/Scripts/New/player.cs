using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class player : MonoBehaviour {
	
	[SerializeField]inputManager iManager;
	[SerializeField]turnManager tManager;
	[SerializeField]functionManager SS;
	[SerializeField]statsManager dataBase;

	public delegate void sendMessage(string MessageOverlay);
	public event sendMessage SendNetworkMessage;
	
	GameObject myPlayer;
	string playerName;
	
	Vector2 playerPosition;
	int energy;
	int exp;
	int time;
	int turn;
	Stack<action> actions = new Stack<action>();
	//LinkedList<action> actStack; 	// I don't know how it works, it keeps on giving me bugs!
	
	[SerializeField] int playerIndex;
	[SerializeField] float speed;
	bool finishedTurn;
	int waitCount;	// Keeps track of the num of weapons that are not done
	// Temporary
	weapon currWeapon;

	void Start () {
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		iManager = gameController.GetComponent<inputManager> ();
		tManager = gameController.GetComponent<turnManager> ();
		SS = gameController.GetComponent<functionManager> ();
		dataBase = GameObject.Find ("stats").GetComponent<statsManager> ();

    	myPlayer = this.gameObject;
    	time=-1;
    	turn=0;
		// This is not a permanent solution
		// But it sure ensure that all movements (espeacially ones involving collisions)
		// are done in a stepTime
		speed = 3 / dataBase.stepTime;
		// Temp
		// You have to add it as a component for the Update and Start methods to run
		// Pretty disturbing if you think about it
		currWeapon = this.gameObject.AddComponent<blaster> ();
		currWeapon.master = this;
		// This should be on playerNetworkMover or something
		PhotonView pview = new PhotonView();
		if (pview.isMine) {
			iManager.setMyPlayer(this);
			playerIndex=0;
		}
	}
	
	void Update(){
		if (tManager.getTurn()==turn) return;
		if (tManager.getTime()==time) return;
		// Everything happens in between!
		print ("Player Time = " + time.ToString ());

		// If we don't syncronize time first, weapons will be fired one step early
		time=tManager.getTime();
		startStep ();

		if (time == -1) {
			turn = tManager.getTurn ();
			print ("Player end of turn!");
			print("Turn:" + turn.ToString());
		}
	}
	
	void startStep (){
		print ("Prepare to move!");
		if (actions.Count > 0 && !finishedTurn) {
			action currentAction = actions.Pop();
			print ("current attack: "+currentAction.attack.ToString());
			if(currentAction.attack!=new Vector2(0.5f,0.5f)){
				currWeapon.fireWeapon(currentAction.attack,time);
				// Everytime we fire a weapon we have to wait for it to hit something
				waitCount+=1;
			}
			if (actions.Count>0){
				Vector2 velocity = actions.Peek().movement-currentAction.movement;
				// Attention! Action contain position while we need velocity!
				tManager.attemptToMove (velocity,currentAction.extraMovement,playerIndex);
			}
		} else {
			// No action left to do!
			actions.Clear();
			print ("No action left! WaitCount: "+waitCount.ToString());
			finishedTurn=true;
			// Even if it's not moving, it should tell turnManager because the other player might still be moving
			// Notice that one player can be done with actions and still move because they are knocked away
			tManager.attemptToMove (Vector2.zero,Vector2.zero,playerIndex);
			if (waitCount==0){
				tManager.finishAction(playerIndex);
			}
		}
	}

	public void resetTurn(){
		finishedTurn = false;
	}

	// The weapon tells the player that it's done
	public void weaponHit(){
		waitCount -= 1;
	}

	void receiveDamage(){
		
	}
  	
	// Called by turn manager
	public IEnumerator moveStep(List<Vector2> vSequence){
		print ("Move step!");
		float time;
		for (int i=0; i<vSequence.Count; i++) {
			if(vSequence[i]!=Vector2.zero){
				time=SS.abs(vSequence[i])/speed;
				print (vSequence[i]);
				// Up till this point everything only exists in data
				// move displays the data
				StartCoroutine(move(playerPosition+vSequence[i],time));
				yield return new WaitForSeconds(time);
			}

		}
	}

  	IEnumerator move(Vector2 target, float time){
		print ("move!");
		print ("Target:" + target.ToString ());
		print ("Time estimate:");
		print (time);

		Vector3 startPos = SS.hexPositionTransform(playerPosition);
		Vector3 endPos =  SS.hexPositionTransform(target);
		float d = Vector3.Distance(startPos,endPos);
		
		float v = d/time * Time.fixedDeltaTime;
		int step = Mathf.FloorToInt(time/Time.fixedDeltaTime)+1;
		float currTime = Time.time;

		// Actually making the move in the board
		playerPosition = target;
		for(int i = 0;i<step;i++){    
			myPlayer.transform.position = Vector3.MoveTowards(startPos,endPos,v);
			startPos=myPlayer.transform.position;
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		Debug.Log("Difference:");       // Trying to see if it works!
		Debug.Log(Time.time-currTime-time);
	}

	public weapon getWeapon(int weaponId){
		return null;
	}

	public Vector2 getPosition(){
		return playerPosition;
	}

	public void setPosition(Vector2 pos){
		playerPosition = pos;
	}
	
	public void setActionSequence(Stack<action> commands){
		foreach (action item in commands) {
			actions.Push(item);
		}
	}

	public void setAction(){

	}
}