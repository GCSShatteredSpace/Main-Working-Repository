using UnityEngine;
using System.Collections;

public class cameraMovement : MonoBehaviour {

	GameObject myPlayer;
	bool playerIsSet = false;

	void Update () {
		if (playerIsSet) {
			this.transform.LookAt(myPlayer.transform);
		}
	}

	public void setPlayer(GameObject p){
		myPlayer = p;
		playerIsSet = true;
	}
}
