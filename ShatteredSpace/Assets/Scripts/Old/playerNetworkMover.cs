using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class playerNetworkMover : Photon.MonoBehaviour {
	
	[SerializeField] GameObject controller;
	[SerializeField] GameController gctrl;
	[SerializeField] actionController actrl;
	[SerializeField] statsController dataBase;
	[SerializeField] GameObject statsCtrl;
	public delegate void Respawn(float time);
	public event Respawn RespawnMe;
	public delegate void sendMessage(string MessageOverlay);
	public event sendMessage SendNetworkMessage;

	public int playerIndex; //changes every turn, depending on the order of players getting ready
	
	[SerializeField] List<Vector2> playerMovement;
	[SerializeField] List<Vector2> playerAttack;
	[SerializeField] int moveIndex;
	[SerializeField] move playerMove;

	public bool waiting;
	public bool send;

	public int energy;
	public int exp;
	public Text energyDisplay;
	public Text expDisplay;
	public bool[] upgradeOn;
	
	void Start () {
		controller = GameObject.Find ("GameController(Clone)");
		gctrl=controller.GetComponent<GameController>();
		statsCtrl = GameObject.Find("gameData");
		actrl = GameObject.Find ("actionController").GetComponent<actionController> ();
		dataBase = statsCtrl.GetComponent<statsController> ();
		energy = dataBase.startingEnergy;
		if(photonView.isMine)
		{
			//GetComponent<Player>().enabled = true;
			GetComponent<SphereCollider>().enabled = true;
			energyDisplay = GameObject.Find("energy").GetComponent<Text>();
			energyDisplay.text = "Energy:"+energy.ToString();
			expDisplay = GameObject.Find ("exp").GetComponent<Text>();
			expDisplay.text = "Exp:0";
		}
		else{
			send = false;
		}
		waiting = true;
	}

	void OnTriggerEnter(Collider other) {
		//print("collision!");
		weaponProperties hitWeapon;
		GameObject otherObject = other.gameObject;
		if (otherObject.tag == "damage" || otherObject.tag == "pickup") {
			hitWeapon = otherObject.GetComponent<weaponProperties> ();
			photonView.RPC ("takeDamage",PhotonTargets.All,hitWeapon.damage);
			if(hitWeapon.special.IndexOf("go")!=-1){
				string sp = hitWeapon.special;
				string temp = sp.Substring(sp.IndexOf("go")+2);
				int x = int.Parse(temp.Substring(0,temp.IndexOf(",")));
				temp = temp.Substring(temp.IndexOf(",")+1);
				int y = int.Parse(temp.Substring(0,temp.IndexOf(";")));
				print (new Vector2(x,y));
				Destroy(other.gameObject);
				photonView.RPC ("getBlown",PhotonTargets.All,new Vector2(x,y));
			}
			Destroy (otherObject);
		} else {
		}
	}

	[PunRPC]
	public void takeDamage(int amount){
		energy = energy - amount;
		SendNetworkMessage(PhotonNetwork.playerName+" takes "+amount.ToString()+" damage!");
		if(photonView.isMine){
			energyDisplay.text = "Energy:"+energy.ToString();
		}
	}

	[PunRPC]
	void getBlown(Vector2 direction){		//might be flawed
		for (int i=0;i<actrl.playerPosition.Count;i++) {
			if(i!=playerIndex)StartCoroutine (actrl.attemptToMove(i,actrl.playerPosition[i],new Vector2(0,0),false));
		}
		StartCoroutine (actrl.attemptToMove(playerIndex,actrl.playerPosition[playerIndex],direction,false));
	}

	[PunRPC]
	void getReady(string name,Vector2 position)
	{
		if (gctrl.readyPlayers == 0) {
			actrl.clearPlayerData();
		}
		gctrl.readyPlayers++;
		print(name+" got ready!");
		if (photonView.isMine)SendNetworkMessage(name + " got ready!");  //This is buggy but I don't know why
		actrl.addPlayerData(this.gameObject,playerMove,position);
	}

	//when I add all the data at the same time I get errors
	//I don't know what's going on, so I used the most stupid way to get it done
	//In case you don't know, RPC stands for 'remote procedural call'
	//Basically, the function gets called on every instance of the object on the network
	//For more information go check the tutorial:
	//http://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/merry-fragmas-multiplayer-fps-2

	[PunRPC]
	void addWeapon(int wpn)
	{
		playerMove.weapon.Add(wpn);
	}

	[PunRPC]
	void addTarget(Vector2 tgt)
	{
		playerMove.attack.Add(tgt);
	}

	[PunRPC]
	void addMovement(Vector2 mov)
	{	
		playerMove.movement.Add(mov);
	}

	[PunRPC]
	void addExtra(Vector2 ext)
	{
		playerMove.extra.Add(ext);
	}

	[PunRPC]
	public void getData(Vector2 num){
		if (true) {
			transform.position = gctrl.positionTransform(num);		
		}
	}

	[PunRPC]
	public void clearData(){
		playerMove=new move(new List<Vector2>(),new List<Vector2>(), new List<int>(), new List<Vector2>());
	}
}