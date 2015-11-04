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
	PhotonView photonView;
	
	GameObject myPlayer;
	string playerName;
	
	[SerializeField] Vector2 playerPosition;
	int energy;
	int exp;
	int time;
	int turn;
	Stack<action> actions = new Stack<action>();
	Stack<action> copy = new Stack<action> ();
	//LinkedList<action> actStack; 	// I don't know how it works, it keeps on giving me bugs!
	
	[SerializeField] public int playerIndex;
	[SerializeField] int playerGameID;
	[SerializeField] float speed;
	bool finishedTurn;
	int waitCount;	// Keeps track of the num of weapons that are not done
	// Temporary
	weapon currWeapon;

	void Awake () {
		initPlayer ();
	}

	void initPlayer(){
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
		photonView = PhotonView.Get (this);

		//distinguishes which player is to be controlled
		if (photonView.isMine) {
			iManager.setMyPlayer (this);
			playerIndex = 0;
		} else {
			playerIndex = 1;
		}
	}

	void Update(){
		if (tManager.getTurn()==turn) return;
		if (tManager.getTime()==time) return;
		// Everything happens in between!
		print ("Player Time = " + time.ToString ());
		startStep ();


		// What about making a list of delegates?
		time=tManager.getTime();
		if (time == -1) {
			turn = tManager.getTurn ();
			print ("Player end of turn!");
			print("Turn:" + turn.ToString());
		}
	}

	public void addPlayerList(PhotonView v, Vector2 startPos){
		v.RPC ("addPlayer", PhotonTargets.OthersBuffered, v.viewID, startPos);
	}

	//Existing players recieve data about the new player that has joined
	[PunRPC]
	void addPlayer(int id, Vector2 startPos){
		player p = (PhotonView.Find(id).gameObject.GetComponent<player>());
		p.setPosition (startPos);
		initPlayer ();
		tManager.addPlayer (p);
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
			if (actions.Count > 0){
				Vector2 velocity = actions.Peek().movement-currentAction.movement;
				// Attention! Action contain position while we need velocity!
				tManager.attemptToMove (velocity,currentAction.extraMovement,playerIndex);
			}
		} else {
		// No action left to do!
			actions.Clear();
			print ("No action left! WaitCount: "+waitCount.ToString());
			finishedTurn = true;
			if (finishedTurn && waitCount==0){
				tManager.finishAction(playerIndex);
			}
		}
	}

	public void resetTurn(){
		finishedTurn = false;
	}

	public void weaponHit(){
		waitCount -= 1;
	}

	void receiveDamage(){
		
	}
  	
	// Called by turn manager
	public IEnumerator moveStep(List<Vector2> vSequence){
		print ("Move step!");
		float time;
		print ("Count " + vSequence.Count);
		for (int i=0; i<vSequence.Count; i++) {
			print ("why");
			print (vSequence[i]);
			if(vSequence[i]!=Vector2.zero){
				time=SS.abs(vSequence[i])/speed;
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
		print ("player pos" + playerPosition);
		return playerPosition;
	}

	public void setPosition(Vector2 pos){
		playerPosition = pos;
	}

	public int getID(){
		return playerGameID;
	}

	public void setID(int i){
		playerGameID = i;
		photonView.RPC ("transferID", PhotonTargets.OthersBuffered, i);
	}

	[PunRPC]
	void transferID(int i){
		playerGameID = i;
	}

	//sets actions and transfers actions to dummy players on other clients
	public void setActionSequence(Stack<action> commands){
		foreach (action item in commands) {
			actions.Push(item);
		}
		if (photonView.isMine) {
			//reverse the stack
			Stack<action> rev = new Stack<action>();
			foreach(action item in commands){
				rev.Push(item);
			}
			foreach (action item in rev) {
				photonView.RPC ("transferAction", PhotonTargets.Others, item.movement, item.attack,
			                item.weaponId, item.extraMovement);
			}
			photonView.RPC ("setSequence", PhotonTargets.Others);
		}
		resetTurn ();
	}

	[PunRPC]
	void transferAction(Vector2 mov, Vector2 att, int wId, Vector2 extra){
		action a = new action ();
		a.movement = mov;
		a.attack = att;
		a.weaponId = wId;
		a.extraMovement = extra;
		copy.Push (a);
	}

	[PunRPC]
	void setSequence(){
		setActionSequence (copy);
		copy.Clear ();
		tManager.getReady ();
	}	
}