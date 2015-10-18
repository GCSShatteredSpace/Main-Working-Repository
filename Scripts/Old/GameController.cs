using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	//currently this monobehaviour has too many public variables
	//we might want to sort that out
	[SerializeField] float piecesize;
	[SerializeField] int gridsize;
	public int moveindex;
	[SerializeField] GameObject piece;
	[SerializeField] GameObject player;
	[SerializeField] GameObject turret;
	[SerializeField] Vector2[] turretSpawnPoint = new Vector2[5];
	public Vector2 playerPosition;
	public Vector2 firePosition;

	public menu playerMenu;

	//list and arrays - I don't know which one is better!
	public List<Vector2> playerMovement = new List <Vector2> ();
	public List<Vector2> playerAttack = new List <Vector2> ();
	public List<int> attackWeapon = new List <int> ();
	public Vector2[] extraMovement = new Vector2[10];

	
	const float ratio = 0.866f;
	Transform boardHolder;
	int step;
	public bool hold;
	public Vector3 targetStart;
	public Vector3 targetEnd;
	LineRenderer targetLine;

	public bool[] upgradeOn;

	public int currentWeapon;
	public bool commandable;
	public int maxSteps;

	public int readyPlayers;

	const float pi = 3.1415f;

	void Start(){
		player = GameObject.Find ("actionController");
		playerMenu = GameObject.Find ("currentWeaponMenu").GetComponent<menu>();
		targetLine = GetComponent <LineRenderer> ();
		BoardSetup ();
		clearSetup (new Vector2(0,0));
		commandable = true;

	}

	void BoardSetup () {							//create a virtual board for the player to make commands
		boardHolder = new GameObject ("Board").transform;
		Vector3 pieceposition;
		for(int i=1;i<gridsize*2;i++){
			for(int j=1;j<2*gridsize-Mathf.Abs(gridsize-i);j++){
				//crazy math formulas for the actuall positions, don't delve in if you value your life!
				pieceposition= new Vector3 (piecesize*(Mathf.Abs(gridsize-i)*0.5f+j-gridsize),piecesize*ratio*(i-gridsize),0f);
				GameObject instance = Instantiate(piece,pieceposition,Quaternion.identity) as GameObject;
				Tile tile = instance.GetComponent<Tile>();
				//the same craziness here
				tile.tilePosition=(new Vector2(j-0.5f*(gridsize+i-Mathf.Abs(i-gridsize)),i-gridsize));
				instance.transform.SetParent (boardHolder);
			}
		}
		for (int i=0; i<=5; i++) {
			GameObject newTurret = Instantiate(turret,positionTransform(turretSpawnPoint[i]),Quaternion.Euler(0,90,90)) as GameObject;
			//if we spawn real models, we have to sort the rotation out
			newTurret.GetComponent<turret>().turretPosition=turretSpawnPoint[i];
		}
	}

	public void moveCommand(Vector2 position) {
		moveindex++;
		playerAttack.Insert(moveindex, new Vector3 (.5f, .5f));//(0.5,0.5) basically stands for "nothing"
		attackWeapon.Insert(moveindex,0);
		playerMovement.Insert(moveindex,position);
		playerPosition = position;
		targetLine.enabled = false;
		firePosition = playerPosition;
	}

	public void clearSetup(Vector2 position){  //reset after each round
		playerPosition = position;
		firePosition = playerPosition;
		moveindex = 0;

		playerMovement.Clear ();
		playerAttack.Clear ();
		attackWeapon.Clear ();

		extraMovement=new Vector2[10];
		playerMovement.Insert(0,playerPosition);
		playerAttack.Insert (0, new Vector2 (.5f, .5f));
		attackWeapon.Insert(0,0);

		targetEnd = new Vector3 (.5f, .5f, 0);
		targetStart = playerPosition;
		targetLine.enabled = false;
		commandable = true;
		readyPlayers = 0;
	}

	public void cancelCommand(){
		if (playerMovement.Count == 1) {
			clearAttack();
			return;
		}

		playerAttack.RemoveAt (moveindex);
		attackWeapon.RemoveAt (moveindex);
		playerMovement.RemoveAt(moveindex);
		extraMovement [moveindex] = Vector2.zero;
		moveindex--;
		playerPosition = playerMovement[moveindex];
		firePosition = playerPosition-extraMovement[moveindex];
		//print (moveindex);

		targetLine.enabled = false;
		targetStart = firePosition;
		targetEnd = playerAttack[moveindex];
		playerMenu.select (attackWeapon[moveindex]);
		if (targetEnd != new Vector3(.5f,.5f,0)){
			drawLine();
		}

	}

	//this is the speacial condition where the player wants to cancel his or her attack command
	//in the first time segment
	public void clearAttack(){
		playerMovement[moveindex]=playerMovement[moveindex]-extraMovement[moveindex];
		extraMovement[moveindex]=Vector2.zero;
		playerPosition = playerMovement [moveindex];
		playerAttack[moveindex]=new Vector2 (.5f, .5f);
		attackWeapon[moveindex]=0;
		targetLine.enabled = false;
		return;
	}

	public void attackCommand(Vector2 position){
		playerAttack[moveindex] = position;
		attackWeapon[moveindex] = currentWeapon;
		drawLine ();
	}

	void drawLine(){
		targetLine.enabled = true;
		targetLine.SetPosition (0, positionTransform (targetStart)+Vector3.back);
		targetLine.SetPosition (1, positionTransform(targetEnd)+Vector3.back);
	}

	public string getSp (string sp,string name){
		if (sp.IndexOf (name) != -1) {
						string temp = sp.Substring (sp.IndexOf (name) + name.Length);
						return temp;
				} else {
			return null;		
		}
	}

	public Vector3 positionTransform (Vector2 position){
		return new Vector3 (piecesize*(position.x+0.5f*position.y),piecesize*ratio*position.y,0);
	}

	public bool isOutOfBounds(Vector2 positon){
		float x = positon.x;
		float y = positon.y;
		if (x >= 0) {
			if(y<=0){
				return (x>=gridsize)||(y<=-gridsize);
			}else{
				return (x+y>=gridsize);
			}	
		}else if(y>=0){
			return (x<=-gridsize)||(y>=gridsize);
		}else{
			return (x+y<=-gridsize);
		}
	}

	//this is used to spwan a collision detector with correct rotation
	//we probably will need to use this more when creating projectile animations
	public Quaternion vector3ToQuaternion(Vector3 v){
		float x = v.x;
		float y = v.y;
		float theta1;
		if (x>0||(x==0&&y>0)){
			theta1 = Mathf.Atan (y / x);
		}else{
			theta1 = pi+Mathf.Atan (y / x);
		}
		Quaternion q = new Quaternion();
		q = Quaternion.Euler(0,0,Mathf.Rad2Deg*theta1);
		return q;
	}
}
  