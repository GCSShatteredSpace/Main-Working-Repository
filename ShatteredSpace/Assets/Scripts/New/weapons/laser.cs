using UnityEngine;
using System.Collections;

public class laser : weapon { 
	// Just a place holder

	bool overheat;
	int turnFired;

	public laser():base("Laser","No delay but overheats when fired.",4,4,0,1){
	}

	void Update(){
		base.Update();
		if(overheat && tManager.getTurn() == turnFired + 2){
			overheat = false;
		}
	}

	public override void fireWeapon(Vector2 pos, int time){
		base.fireWeapon(pos, time);
		overheat = true;
		turnFired = tManager.getTurn();
	}


	public override bool readyToFire(){
		return base.readyToFire() && !overheat;
	}


}
