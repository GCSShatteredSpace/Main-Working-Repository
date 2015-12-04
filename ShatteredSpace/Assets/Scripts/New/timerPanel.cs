using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class timerPanel : MonoBehaviour {
	[SerializeField] GameObject timer;
	[SerializeField] inputManager iManager;
	[SerializeField] statsManager database;
	[SerializeField] player myPlayer;
	Color green = Color.green;
	Color red = Color.red;

	void Start(){
		GameObject gameController = GameObject.Find ("gameController");
		iManager = gameController.GetComponent<inputManager> ();
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
		myPlayer = iManager.getMyPlayer ();

	}

	//Manager the timer bar
	void Update(){
		if (iManager.isCommandable ()) {
			float timeElapsed = getTime ();
			float timeRemaining = database.turnTimer - timeElapsed;
			if (timeRemaining < -2) { //Make UI feel better. Turn does not instantly end
				iManager.sendCommands ();
			} else {
				Color res = Color.Lerp (green, red, 1 - timeRemaining / 15);
				getImage ().color = res;
				//leaves bit of timer for player to see when time about to run out
				if(timeRemaining < 0){
					timeRemaining = 0;
				}
				//timer does not start moving immediately to make UI feel better
				getImage().fillAmount = ((timeRemaining + 1)/ 15);
			}
		} else {
			getImage ().fillAmount = 0;
		}
	}

	//Time since start of input phase
	public float getTime(){
		return Time.time - iManager.getTurnStartTime();
	}

	public Image getImage(){
		return timer.GetComponent<Image> ();
	}

}
