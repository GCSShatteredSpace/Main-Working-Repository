﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tile : MonoBehaviour {

	bool isPlayer;
	bool isDamage;
	bool isTurret;
	bool isWall;
	bool wallUp;
	bool turretUp;
	List<damageInfo> damageInfos = new List<damageInfo>();
	int playerId;

	public bool hasPlayer(){
		return isPlayer;
	}

	public bool hasDamage(){
		return isDamage;
	}

	public bool hasTurret(){
		return isTurret;
	}

	public bool hasWall(){
		return isWall;
	}

	public bool turretIsActivated(){
		return turretUp;
	}

	public bool wallIsActivated(){
		return wallUp;
	}

	public int getPlayer(){
		return playerId;
	}

	public List<damageInfo> getDamage(){
		return damageInfos;
	}

	public void setPlayer(int playerIndex){
		playerId = playerIndex;
		isPlayer = true;
	}

	public void clearPlayer(){
		isPlayer = false;
	}
	
	public void addDamage(damageInfo damage){
		damageInfos.Add (damage);
		isDamage = true;
	}

	// Once it's set to be a turret spwan point, it cannot be changed
	public void setTurretSpawnPoint(){
		isTurret = true;
	}

	// Once it's set to be a blast sheld, it cannot be changed
	public void setBlastShield(){
		isWall = true;
	}
	
	public void activateTurret(bool isTrue){
		turretUp = isTrue;
	}
	
	public void activateWall(bool isTrue){
		wallUp = isTrue;
	}
	
	public void clearDamage(){
		damageInfos.Clear();
	}
}