using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class player : MonoBehaviour {
	
	[SerializeField]inputManager iManager;
	[SerializeField]turnManager tManager;
	[SerializeField]functionManager SS;
	[SerializeField]statsManager database;
	GameObject myPlayer;
	
	// Networking stuff
	PhotonView photonView;
	public delegate void SendMessage(string MessageOverlay);
	public event SendMessage SendNetworkMessage;
	[SerializeField] public int playerIndex;
	[SerializeField] int playerGameID;

	// Turn management stuff
	bool finishedTurn;
	int waitCount;	// Keeps track of the num of weapons that are not done
	Stack<action> actions = new Stack<action>();
	Stack<action> copy = new Stack<action> ();
	
	
	// Gameplay stuff
	[SerializeField] Vector2 playerPosition;
	int energy;
	Dictionary<string, int> exp = new Dictionary<string, int>();
	Dictionary<string, bool> hasTech = new Dictionary<string, bool>();	
	Dictionary<string, GameObject> techPanel = new Dictionary<string, GameObject>();	
	int time;
	int turn;
	
	// Animation stuff
	[SerializeField] float speed;

	// Weapon stuff
	weapon currWeapon;
	
	List<weapon> weapons = new List<weapon> ();
	List<int> weaponList = new List<int>();
	playerWpnMenu playerMenu;
	
	// Special stuff
	Vector2 momentum = Vector2.zero;	// This is the momentum caused by explosions
	public bool isFreezed = false;
	
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
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
		
		myPlayer = this.gameObject;
		
		energy = database.playerStartEnergy;
		exp.Add("momentum",0);
		exp.Add("explosive",0);
		exp.Add("particle",0);
		exp.Add("field",0);

		hasTech.Add("momentum",true);
		hasTech.Add("explosive",true);
		hasTech.Add("particle",false);
		hasTech.Add("field",false);	

		techPanel.Add("momentum",GameObject.Find("momentumPanel"));
		techPanel.Add("explosive",GameObject.Find("explosivePanel"));
		techPanel.Add("particle",GameObject.Find("particlePanel"));
		techPanel.Add("field",GameObject.Find("fieldPanel"));
		
		time = -1;
		turn = 0;
		waitCount = 0;
		// This is not a permanent solution
		// But it sure ensure that all movements (espeacially ones involving collisions)
		// are done in a stepTime
		speed = 3 / database.stepTime;
		// Temp
		
		photonView = PhotonView.Get (this);
		
		// You have to add it as a component for the Update and Start methods to run
		// Pretty disturbing if you think about it...
		// All players start with blaster
		//print ("typeof:" + currWeapon.GetType().ToString ());
		//distinguishes which player is to be controlled
		if (photonView.isMine) {
			iManager.setMyPlayer (this);
			playerIndex = 0;
			playerMenu = GameObject.Find("currentWeaponMenu").GetComponent<playerWpnMenu>();
			playerMenu.setMyPlayer(this);
			// Add blaster as starting weapon
			addWeapon (0);
			setWeapon (0);
			GameObject.Find ("player1Energy").GetComponent<Text> ().text = PhotonNetwork.player.name + ": " + 
								this.energy.ToString ();
			GameObject.Find("Main Camera").GetComponent<cameraMovement>().setPlayer(this.gameObject);
		} else {
			playerIndex = 1;
			// The other player gets a different database for its set of weapons (they are different instances)
			database = Instantiate(database);
			playerMenu = GameObject.Find("oppoentWeaponMenu").GetComponent<playerWpnMenu>();
			GameObject.Find ("player2Energy").GetComponent<Text>().text = PhotonNetwork.otherPlayers[0].name + ": " + 
								this.energy.ToString();
		}
	}
	
	void Update(){
		// Display energy
		if (this.playerIndex == 0) {
			GameObject.Find ("player1Energy").GetComponent<Text> ().text = PhotonNetwork.player.name + ": " + 
				this.energy.ToString (); 
		}
		else {
			GameObject.Find ("player2Energy").GetComponent<Text> ().text = PhotonNetwork.otherPlayers[0].name + ": " + 
				this.energy.ToString ();
		}

		if (tManager.getTurn()==turn) return;
		if (tManager.getTime()==time) return;

		// Everything happens in between!
		//print ("Player Time = " + time.ToString ());
		// If we don't syncronize time first, weapons will be fired one step early
		time = tManager.getTime();
		// Start of turn
		if (time == 0){
			if (currWeapon.isPassive() && currWeapon.readyToFire()){
				currWeapon.fireWeapon(Vector2.zero,time);
			}
			//printMovement();
		}
		startStep ();
		// End of turn
		if (time == -1) {
			turn = tManager.getTurn ();
			//print ("Player end of turn!");
			//print("Turn:" + turn.ToString());
		}
	}

	// For debugging
	void printMovement(){
		print ("Player" + playerIndex.ToString ());
		foreach(action item in actions){
			print (item.movement);
		}
	}
	
	void startStep (){
		//print ("Prepare to move!");
		if (actions.Count > 0) {
			action currentAction = actions.Pop();
			//			print ("current attack: "+currentAction.attack.ToString());
			if(currentAction.attack!=new Vector2(0.5f,0.5f)){
				currWeapon.fireWeapon(currentAction.attack,time);
				playerMenu.revealWeapon(weapons.IndexOf(currWeapon));
				// Everytime we fire a weapon we have to wait for it to hit something
				waitCount+=1;
			}
			if (actions.Count > 0){
				//		print (this.playerIndex + " " + actions.Peek().movement.ToString());
				Vector2 velocity = actions.Peek().movement-currentAction.movement;
				// Action contain position while we need velocity!
				tManager.attemptToMove (velocity,currentAction.extraMovement,playerIndex);
			} else {
				tManager.attemptToMove (momentum,Vector2.zero,playerIndex);
			}
		} else {
			// No action left to do!
			//print ("No action left! WaitCount: " + waitCount.ToString());
			if (!finishedTurn){
				tManager.stopMovement();
			}

			finishedTurn = true;
			// Even if it's not moving, it should tell turnManager because the other player might still be moving
			// Notice that one player can be done with actions and still move because they are knocked away
			tManager.attemptToMove (momentum,Vector2.zero,playerIndex);
			if (waitCount == 0 && momentum == Vector2.zero){
				tManager.finishAction(playerIndex);
			}
			momentum = Vector2.zero;
		}
	}
	
	public void resetTurn(){
		finishedTurn = false;
		momentum = Vector2.zero;
	}
	
	// Called by turn manager
	public IEnumerator moveStep(List<Vector2> vSequence){
		float time;
		for (int i=0; i<vSequence.Count; i++) {
			if(vSequence[i]!=Vector2.zero){
				time=SS.abs(vSequence[i])/speed;
				// Up till this point everything only exists in data
				// move displays the data
				StartCoroutine(move(playerPosition+vSequence[i],time));
				yield return new WaitForSeconds(time);
			}
			
		}
	}
	
	// Where the player's position actually changes
	IEnumerator move(Vector2 target, float time){
		//print ("move!");
		//print ("Target:" + target.ToString ());
		//print ("Time estimate:");
		//print (time);
		
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
		//Debug.Log("Difference:");       // Trying to see if it works!
		//Debug.Log(Time.time-currTime-time);
	}

	// -----------------------
	// Weapon-related actions
	// -----------------------

	// The weapon tells the player that it's done
	public void weaponHit(){
		//		print ("Weapon has hit!");
		waitCount -= 1;
	}
	
	public void takeDamage(int amount){
		// This RPC seems redundant, but it handles building weapons
		if (photonView.isMine) {
			photonView.RPC ("networkTakeDamage", PhotonTargets.All, amount);
		}
	}
	
	[PunRPC]
	void networkTakeDamage(int amount){
		energy -= amount;
		SendNetworkMessage (PhotonNetwork.player.name + " took " + amount.ToString() + " damage!");
		SendNetworkMessage (PhotonNetwork.player.name + " has " + energy.ToString() + " energy!");
		if (energy <= 0) die();
	}

	public void gainExp(string technology, int amount){
		// This RPC seems redundant, but it handles building weapons
		if (photonView.isMine) {
			photonView.RPC ("networkGainExp", PhotonTargets.All, technology, amount);
		}
	}
	
	[PunRPC]
	void networkGainExp(string technology, int amount){
		exp[technology] += amount;

		SendNetworkMessage (PhotonNetwork.player.name + " gained " + amount.ToString() + " exp!");
		//SendNetworkMessage (PhotonNetwork.player.name + " has " + energy.ToString() + " exp!");
		if (photonView.isMine) {
			if (exp [technology] >= database.upgradeExp) {
				showNewUpgrades ();
				exp [technology] %= database.upgradeExp;
			}
			Slider display = techPanel[technology].GetComponent<expPanel>().getSlider();
			display.value = exp[technology];
		}else{
			exp [technology] %= database.upgradeExp;
		}
	}

	void showNewUpgrades(){
		
	}

	// TODO:This should be a network thing
	public void gainTechnology(string technology){
		hasTech [technology] = true;
		GameObject panel = techPanel[technology];
		Text techText = panel.GetComponentInChildren<Text>();
		Color temp = techText.color;
		temp.a = 255f;
		techText.color = temp;
		expPanel display = panel.GetComponent<expPanel> ();
		display.showDisplay();
	}
	
	void die(){
		SendNetworkMessage(PhotonNetwork.player.name + " is destroyed and revived!");
		takeDamage(energy - database.playerStartEnergy);
		SendNetworkMessage(PhotonNetwork.player.name + " has " + energy.ToString() + " energy!");
	}
	
	// ---------------------------------------
	// Initializing the player in the network
	// ---------------------------------------
	
	public void addPlayerList(PhotonView v, Vector2 startPos){
		v.RPC ("addPlayer", PhotonTargets.OthersBuffered, v.viewID, startPos);
	}
	
	//Existing players recieve data about the new player that has joined
	[PunRPC]
	void addPlayer(int id, Vector2 startPos){
		player p = (PhotonView.Find(id).gameObject.GetComponent<player>());
		p.setPosition (startPos);
		//initPlayer ();
		tManager.addPlayer (p);
	}

	[PunRPC]
	void transferID(int i){
		playerGameID = i;
	}
	
	// ----------------------------------------------
	// Sending and receiving commands to the network
	// ----------------------------------------------

	//sets actions and transfers actions to dummy players on other clients
	// Called by inputManager
	public void setActionSequence(Stack<action> commands){
		actions.Clear ();
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
		print ("player"+playerIndex.ToString()+" got ready!");
		tManager.getReady();
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
	}	

	// -------------
	// Weapon stuff
	// -------------

	// TODO: make the players fire multiple weapons in same turn
	public void setWeapon(int weaponIndex){
		photonView.RPC ("networkSetWeapon", PhotonTargets.AllBuffered, weaponIndex);
	}
	
	[PunRPC]
	void networkSetWeapon(int weaponIndex){
		int i;
		for (i = 0; i < weaponList.Count; i++){
			if (weaponList[i] == weaponIndex){
				// Player doesn't store weapons in the order of weapon index
				if (currWeapon != weapons [i]){
					currWeapon = weapons [i];
					if (photonView.isMine) iManager.resetCommands(playerPosition);
				}
				return;
			}
		}
	}
	
	public void addWeapon(int wpnID){
		photonView.RPC ("networkAddWeapon", PhotonTargets.AllBuffered, wpnID);
	}
	
	[PunRPC]
	void networkAddWeapon(int wpnID){
		weaponList.Add (wpnID);

		// Can you believe that this actually generates a clone of stats? lol
		weapon newWpn = database.weapons[wpnID];

		newWpn.setMaster (this);
		weapons.Add (newWpn);
		//Assuming player is not allowed to buy additional blasters(wpnID = 0)
		if (wpnID != 0) {
			playerMenu.addWeapon (wpnID);
		}
	}

	// ----------------
	// Special actions
	// ----------------

	// Trapped by gravity trap
	void getTrapped(){}

	// Stunned by antibody grenade
	public void freezeMovement(){
		if (photonView.isMine) {
			if (!isFreezed) {
				isFreezed = true;
				print ("I'm freezed!");
			}
		}
	}

	// Instantly move to another position
	void teleport(){}

	// -------------------
	// Get-set functions
	// -------------------

	public weapon getWeapon(int wpnID = -1){
		if (wpnID == -1)
			return currWeapon;
		else return weapons [weaponList.IndexOf (wpnID)];
	}
	
	public Vector2 getPosition(){
		//print ("player pos" + playerPosition);
		return playerPosition;
	}
	
	public void setPosition(Vector2 pos){
		playerPosition = pos;
	}
	
	// Called by explosive damages that pushes player away
	public void addMomentum(Vector2 push){
		// Momentum shouldn't really add
		// Cause it will break the game
		momentum = push;
		print("Gained momentum!");
	}
	
	public int getID(){
		return playerGameID;
	}
	
	public void setID(int i){
		playerGameID = i;
		photonView.RPC ("transferID", PhotonTargets.OthersBuffered, i);
	}
	
	public int getPlayerIndex(){
		return playerIndex;
	}
	
	public bool hasWeapon(int wpnID){
		if (weaponList.Contains (wpnID)) {
			return true;
		}else{
			return false;
		}
	}
}