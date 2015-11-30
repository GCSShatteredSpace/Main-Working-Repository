using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class networkController : MonoBehaviour {
	
	[SerializeField] Text connectionText;
	[SerializeField] Vector2[] spawnPoints;
	[SerializeField] Camera sceneCamera;
	[SerializeField] GameObject serverWindow;
	[SerializeField] InputField username;
	[SerializeField] InputField roomName;
	[SerializeField] InputField roomList;
	[SerializeField] Text messageWindow;

	functionManager SS;
	inputManager iManager;
	turnManager tManager;

	PhotonView photonView;
	
	GameObject thisPlayer;

	Queue<string> messages = new Queue<string>();
	const int messageCount = 100;
	// The size increment for the textbox for each message
	const int lineHeight = 10;
	int numPlayers;

	void Start () {
		photonView = GetComponent<PhotonView> ();

		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		iManager = gameController.GetComponent<inputManager> ();
		tManager = gameController.GetComponent<turnManager> ();
		SS = gameController.GetComponent<functionManager> ();
		//dataBase = gameController.GetComponent<statsManager> ();

		// Photon stuff
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.ConnectUsingSettings ("0.2");
		StartCoroutine ("UpdateConnectionString");
		PhotonNetwork.autoJoinLobby = true;
	}
	
	IEnumerator UpdateConnectionString () {
		// Show connection status at the lower left corner
		while(true){
			connectionText.text = PhotonNetwork.connectionStateDetailed.ToString ();
			yield return null;
		}
	}

	void OnJoinedLobby(){
		// Show a UI for the player to join a room
		serverWindow.SetActive (true);
	}

	public void joinRoom(){
		PhotonNetwork.player.name = username.text;
		RoomOptions roomOptions = new RoomOptions(){ isVisible = true, maxPlayers = 2 };
		PhotonNetwork.JoinOrCreateRoom (roomName.text, roomOptions, TypedLobby.Default);
	}
	
	void OnReceivedRoomListUpdate(){
		print ("new room!");
		roomList.text = "";
		RoomInfo[] rooms = PhotonNetwork.GetRoomList ();
		foreach(RoomInfo room in rooms)
			roomList.text += room.name + "\n";
	}
	
	void OnJoinedRoom(){
		serverWindow.SetActive (false);
		StartSpawnProcess (0f);
		StopCoroutine ("UpdateConnectionString");
		connectionText.text = "";
	}

	//the code is devised based on a multiplayer FPS tutorial so the players are spawned over and over again
	//now it looks like game should end when one player dies, but maybe the rules will be changed so I decide to keep this
	void StartSpawnProcess (float respawnTime){
		sceneCamera.enabled = false;
		spawnPlayer ();
	}
	
	void spawnPlayer(){
		Vector2 spawnHex = spawnPoints [PhotonNetwork.playerList.Length - 1];
		Vector2 spawnPosition = SS.hexPositionTransform(spawnHex);
		thisPlayer = PhotonNetwork.Instantiate ("RealSSPlayer", spawnPosition,Quaternion.identity,0);
		thisPlayer.GetComponent<player>().setPosition(spawnHex);
		//Sets an absolute id for each player in terms of join order
		if (PhotonNetwork.playerList.Length == 1) {
			thisPlayer.GetComponent<player> ().setID (0);
		} else {
			thisPlayer.GetComponent<player> ().setID (-1);
		}
		photonView.RPC ("incrementPlayers", PhotonTargets.Others);

		iManager.startNewTurn(spawnHex);

		player p = thisPlayer.GetComponent<player> ();
		tManager.addPlayer (p);
		p.addPlayerList (p.GetComponent<PhotonView> (),spawnHex);
		p.SendNetworkMessage += AddMessage;
	}

	//Existing players increment numplayers
	[PunRPC]
	void incrementPlayers(){
		numPlayers++;
		photonView.RPC ("updatePlayers", PhotonTargets.Others, numPlayers);
	}

	//numplayers get updated for everyone
	//the new player gets a newid 
	[PunRPC]
	void updatePlayers(int i){
		numPlayers = i;
		if (thisPlayer.GetComponent<player>().getID() == -1) {
			thisPlayer.GetComponent<player> ().setID (numPlayers);
		}
	}

	void AddMessage(string message)
	{
		photonView.RPC ("AddMessage_RPC", PhotonTargets.All, message);
	}
	
	[PunRPC]
	void AddMessage_RPC(string message)
	{
		messages.Enqueue (message);
//		if(messages.Count > messageCount)
//			messages.Dequeue();

		messageWindow.text = "";
		foreach(string m in messages)
			messageWindow.text += m + "\n";
		messageWindow.text = messageWindow.text.Substring (0, messageWindow.text.Length - 1);
	}

}