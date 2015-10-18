using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class actionController : MonoBehaviour {
	
	public bool action;
	
	public GameObject damageCollider;
	public GameObject controller;
	public GameObject statsCtrl;
	public statsController dataBase;
	
	public weaponProperties hitWeapon;
	[SerializeField] int maxAction;
	public float gameSpeed;
	[SerializeField] Vector3 positionOffset;

	[SerializeField] List<GameObject> players = new List<GameObject> ();
	[SerializeField] List<move> playerMove = new List<move>();
	[SerializeField] List<Vector2[,]> toInstantiate = new List<Vector2[,]>();
	public List<Vector2> playerPosition = new List<Vector2>();
	public List<weaponProperties[,]> weaponsList = new List<weaponProperties[,]>();
	private GameController gctrl;
	

	public bool[] upgradeOn;

	public float time;
	[SerializeField] Text timeDisplay;
	public delegate void SendMessage(string MessageOverlay);
	public event SendMessage SendNetworkMessage;

	public int currentPlayer;

	public bool stop;
	[SerializeField] GameObject cDetector_l;
	[SerializeField] GameObject cDetector_s;

	void Start () {
		controller = GameObject.Find ("GameController(Clone)");
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
		gctrl=controller.GetComponent<GameController>();
		upgradeOn = gctrl.upgradeOn;
		action = false;
		time = -1;
		displayTime ();
	}

	void Update(){
		if (gctrl.readyPlayers == PhotonNetwork.playerList.Length && !action) {
			SendNetworkMessage("Turn starts!");
			StartCoroutine(actionSequence());
		}
	}

	public void addPlayerData(GameObject player,move Move,Vector2 position){
		players.Add (player);
		player.GetComponent<playerNetworkMover> ().playerIndex = players.Count - 1;
		playerMove.Add (Move);
		print ("current position: " + position);
		playerPosition.Add (position);
	}

	public void clearPlayerData(){
		players.Clear ();
		playerMove.Clear ();
		playerPosition.Clear ();
		maxAction = 0;
		toInstantiate.Clear ();
		weaponsList.Clear ();
		currentPlayer = 0;
	}
	
	public IEnumerator actionSequence () {
		action = true;
		time = 0;
		displayTime ();
		int i = 0;
		bool isActive;
		for (currentPlayer=0; currentPlayer<players.Count; currentPlayer++) {
			initializeData (currentPlayer);
		}
		for (i=0; i<=maxAction; i++) {
			time=i+1;
			displayTime ();
			isActive=false;
			for(currentPlayer=0;currentPlayer<players.Count;currentPlayer++){
				StartCoroutine(playerAction(currentPlayer,i));
				if(playerMove[currentPlayer].movement.Count>i||weaponsList[currentPlayer][i,0]!=null){
					isActive = true;
				}
			}
			if (isActive){
				yield return new WaitForSeconds(1);
			}
		}
	
		for (currentPlayer=0; currentPlayer<players.Count; currentPlayer++) {
			if(players[currentPlayer].GetPhotonView().isMine) gctrl.clearSetup (playerPosition[currentPlayer]);
		}
		time = -1;
		displayTime ();
		action = false;
		maxAction = 0;
		yield return new WaitForSeconds (1);
	}

	void initializeData(int currentPlayer){ //start processing attack data received to this part of the program
		
		int i,j=0;
		int d;

		maxAction = findMaxAction (playerMove);

		Vector2[,] attackPlan = new Vector2[10,maxAction+1]; 
		weaponProperties[,] weapons = new weaponProperties[10,maxAction+2]; //size determined by the max weapon delay - I assume it's no larger than 1
		int currentWeapon;
		
		for(i=0;i<maxAction;i++){
			if (i<playerMove[currentPlayer].attack.Count){
				currentWeapon=playerMove[currentPlayer].weapon[i];
			
				if(currentWeapon!=0){
					if(dataBase.weapons[currentWeapon].delay==Mathf.Infinity) {
						d = maxAction-i;
					}else{
						d = Mathf.RoundToInt(dataBase.weapons[currentWeapon].delay);
					}
				
					for(j=0;j<=maxAction;j++){
						if (attackPlan[i+d,j]==Vector2.zero){
							attackPlan[i+d,j]=playerMove[currentPlayer].attack[i];
							weapons[i+d,j]=dataBase.updateWeaponStats(currentWeapon,upgradeOn);
							break;
						}
					}
				}
			}else{

			}
		}
		weaponsList.Add (weapons);
		toInstantiate.Add (attackPlan);
		return;
		
	}

	public IEnumerator playerAction (int currentPlayer,int i) {
		float moveTime = 0.2f;
		int j = 0;
		GameObject cdObject;
		while (weaponsList[currentPlayer][i,j]!=null){
			print ("prepare");
			fireWeapon(weaponsList[currentPlayer][i,j],toInstantiate[currentPlayer][i,j]); //need correction! this function should include player's info
			j=j+1;
		}
		if(i<playerMove[currentPlayer].movement.Count){
			print ("move= "+playerMove[currentPlayer].movement[i]+" extra= "+playerMove[currentPlayer].extra[i]);
			StartCoroutine(attemptToMove(currentPlayer,playerPosition[currentPlayer],playerMove[currentPlayer].movement[i]-playerMove[currentPlayer].extra[i],false));
			yield return new WaitForSeconds(moveTime);
			if(playerMove[currentPlayer].extra[i]!=new Vector2(0,0)){
				StartCoroutine(attemptToMove(currentPlayer,playerPosition[currentPlayer],playerMove[currentPlayer].extra[i],false));
			}
			yield return new WaitForSeconds(1-moveTime);
		}else{
			StartCoroutine(attemptToMove(currentPlayer,playerPosition[currentPlayer],new Vector2(0,0),false));
		}
	}

	public IEnumerator attemptToMove(int currentPlayer,Vector2 position,Vector2 direction,bool small){
		GameObject cdObject = new GameObject();
		GameObject cDetector = new GameObject ();
		if (!small) {
			cDetector = cDetector_l;	
		}else{
			cDetector = cDetector_s;
		}
		if(direction!=new Vector2 (0,0)){
			Vector3 dir=gctrl.positionTransform(direction);
			cdObject = Instantiate(cDetector,gctrl.positionTransform(position+direction),gctrl.vector3ToQuaternion(dir)) as GameObject;
		}else{
			cdObject = Instantiate(cDetector,gctrl.positionTransform(position),Quaternion.Euler(0,90,0)) as GameObject;
		}
		collisionDetector collider = cdObject.GetComponentInChildren<collisionDetector>();
		collider.direction = direction;
		yield return new WaitForSeconds(0.01f);
		bool collision = collider.collision;
		Vector2 result = collider.result;
		if (gctrl.isOutOfBounds (position + direction)) {
			collision=true;
			result = direction*-1;
		}
		if(collision){
			print ("position: " + position + " Direction: " + direction);
			print("result: "+result);
			StartCoroutine(moveStep(currentPlayer,gctrl.positionTransform(position)+gctrl.positionTransform(direction)/2+positionOffset,gameSpeed));
			yield return new WaitForSeconds(0.1f);
			Destroy(cdObject);
			StartCoroutine(attemptToMove(currentPlayer,position+direction,result,true));
		}else{
			StartCoroutine(moveStep(currentPlayer,gctrl.positionTransform(position+direction)+positionOffset,gameSpeed));
			playerPosition[currentPlayer]=position+direction;
			yield return new WaitForSeconds(0.5f);
		}
	}

	public IEnumerator moveStep(int currentPlayer, Vector3 target, float speed){
		int j=0;
		float step = speed * Time.fixedDeltaTime;
		float d = Vector3.Distance (players[currentPlayer].transform.position, target);
		while(j<=d/step+1){
			players[currentPlayer].transform.position = Vector3.MoveTowards(players[currentPlayer].transform.position,target,step);
			j++;
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		
	}
	


	public void destoryTurret(Vector2 position){
		GameObject dmgGlobe = Instantiate (damageCollider, gctrl.positionTransform (position), Quaternion.identity)as GameObject;
		weaponProperties dmgWeapon = dmgGlobe.GetComponent<weaponProperties>();
		dmgWeapon.damage = -dataBase.turretEnergy;
		dmgGlobe.tag = "pickup";
	}

	void fireWeapon(weaponProperties weapon, Vector2 position){ //calculate what type of collision sphere to generate
		bomb (weapon, position);
		string sp = weapon.special;
		weaponProperties tempWeapon = this.gameObject.AddComponent<weaponProperties>();
		tempWeapon.giveValue (weapon);
		if(sp.IndexOf("aoe")!=-1){
			string temp=sp.Substring(sp.IndexOf("aoe")+3);
			int aoeSize = int.Parse(temp.Substring(0,temp.IndexOf(",")));
			print ("Aoesize="+aoeSize);
			temp = temp.Substring(temp.IndexOf(",")+1);
			int aoeDamage = int.Parse(temp.Substring(0,temp.IndexOf(";")));
			temp = temp.Substring(sp.IndexOf(";")+1);
			tempWeapon.damage = aoeDamage;
			if (sp.IndexOf("push")!=-1){
				for (int i=0;i<6;i++){
					tempWeapon.special = "go" + Mathf.RoundToInt(dataBase.directions[i].x).ToString()+","+ Mathf.RoundToInt(dataBase.directions[i].y).ToString()+";";
					bomb (tempWeapon,position+dataBase.directions[i]);
				}
				print(tempWeapon.special);
			}
		}else{

		}

	}

	IEnumerator wait(){
		yield return new WaitForSeconds (1f);
	}

	void bomb(weaponProperties weapon, Vector2 position){	//where the collision sphere is actually created
		GameObject dmgGlobe = Instantiate (damageCollider, gctrl.positionTransform (position), Quaternion.identity)as GameObject;
		weaponProperties dmgWeapon = dmgGlobe.GetComponent<weaponProperties>();
		damageGlobe dmgInfo = dmgGlobe.GetComponent<damageGlobe>();
		dmgWeapon.giveValue (weapon);
		dmgInfo.origin = this.gameObject;
		print("dmgGenerated");
	}

	int findMaxAction(List<move> moves){
		int max = 0;
		for (int i=0; i<moves.Count; i++) {
			if (moves[i].attack.Count>max)	max=moves[i].attack.Count;
		}
		//print ("max action is");
		//print (max);
		return max;
	}

	void displayTime(){
		timeDisplay.text = "Time:" + time.ToString ();
	}
}
