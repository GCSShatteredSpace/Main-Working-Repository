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
	[SerializeField] InputField messageWindow;

	functionManager SS;
	inputManager iManager;
	turnManager tManager;

	PhotonView photonView;
	
	GameObject thisPlayer;
	Queue<string> messages = new Queue<string>();
	const int messageCount = 6;

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
		// This is flawed in many ways
		// The player should be spawned according to the number of players in the room
		// Instead of the number of all players on the network
		// This is the first thing I'm going to fix
		Vector2 spawnPosition = SS.hexPositionTransform(spawnPoints[PhotonNetwork.playerList.Length - 1]);
		thisPlayer = PhotonNetwork.Instantiate ("SSplayer", spawnPosition,Quaternion.identity,0);
		thisPlayer.GetComponent<player>().setPosition(spawnPoints[PhotonNetwork.playerList.Length - 1]);
		iManager.startNewTurn(spawnPoints[PhotonNetwork.playerList.Length - 1]);
		// This is flawed!
		player p = thisPlayer.GetComponent<player> ();
		tManager.addPlayer (p);
		p.addPlayerList (p.GetComponent<PhotonView> ());
		photonView.RPC ("resendState", PhotonTargets.Others);
		//player.GetComponent<playerNetworkMover> ().RespawnMe += StartSpawnProcess;
		//player.GetComponent<playerNetworkMover> ().SendNetworkMessage += AddMessage;
	}

	[PunRPC]
	void resendState(){
		player p = thisPlayer.GetComponent<player> ();
		p.addPlayerList (p.GetComponent<PhotonView> ());
	}
	void AddMessage(string message){
		photonView.RPC ("AddMessage_RPC", PhotonTargets.All, message);
	}

	// This is where the message actually gets sent
	// Notice that it's a RPC, so it's called on every client
	[PunRPC]
	void AddMessage_RPC(string message){
		messages.Enqueue (message);
		if(messages.Count > messageCount)
			messages.Dequeue();
		
		messageWindow.text = "";
		foreach(string m in messages)
			messageWindow.text += m + "\n";
	}
}