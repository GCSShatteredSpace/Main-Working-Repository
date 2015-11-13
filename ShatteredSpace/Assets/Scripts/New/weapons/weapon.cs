﻿using UnityEngine;
using System;

public class weapon : MonoBehaviour
{
	public turnManager tManager;
	public boardManager bManager;
	public functionManager SS;
	
	// There is a protection level thing that I don't know how to sort out
	int damage;
	int range;
	int delay;	// The number of steps damage is generated after the weapon fired
	player master; // The player that the weapon belong to...
	string weaponName; 
	string description; 
	int fireTime; 

	int shotsPlanned;
	int numOfShots;	// Maximum time you can fire this weapon
	
	bool fired;
	Vector2 targetPosition;
	/*Note: intentionally did not include too many required fields in the abstract class. Can add more later if necessary */ 
	
	public weapon(string name, string description,
		int damage, int range, int delay, int shots) /* initiates a basic weapon object. Can override in a 
                                                                                    subclass to construct a weapon with more fields */                                        
	{
		this.weaponName = name;
		this.description = description;
		this.damage = damage;
		this.range = range;
		this.delay = delay;
		this.numOfShots = shots;
	}
	
	void Awake(){
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		tManager = gameController.GetComponent<turnManager> ();
		bManager = gameController.GetComponent<boardManager> ();
		SS = gameController.GetComponent<functionManager> ();
	}
	
	void Update()
	{
		if (tManager.getTime() == fireTime && fired) {
			print ("FireTime = "+fireTime.ToString());
			generateDamage();
		}
	}
	
	public virtual void fireWeapon(Vector2 pos,int time){
		print ("Weapon fired!");
		fired = true;
		// Cause damage is generated at the end of current step
		fireTime = time+delay;
		targetPosition = pos;

		// The turn has started, so reset for the next turn
		setShotsPlanned(0);
	}
	
	public void generateDamage(){
		print ("generate damage!");
		damageInfo newDamage = new damageInfo();
		newDamage.damageAmount = damage;
		newDamage.attacker = master;
		bool hit = bManager.bomb (targetPosition,newDamage);
		// Since the damage is generated, the weapon can take a rest
		fired = false;
		master.weaponHit ();
	}
	
	public int getFireTime()
	{
		return this.fireTime;
	}
	
	public int getDamage()
	{
		return this.damage; 
	}

	// Can be overrided by special ranged weapon like sniperCannon
	public virtual bool isInRange(int distance)
	{
		return distance<=range && distance!=0; 
	}

	// Can be overrided by weapons with overheat
	public virtual bool readyToFire(){
		return numOfShots > shotsPlanned;
	}

	public void planToFire(){
		shotsPlanned += 1;
	}

	public void cancelFire(){
		shotsPlanned -= 1;
	}
	
	public float getDelay()
	{
		return this.delay;
	}
	
	public string getName()
	{
		return this.weaponName;
	}
	
	public string getInfo()
	{
		return this.description; 
	}

	public void setFired(bool value)
	{
		fired = value;
	}

	public player getMaster(){
		return master;
	}

	public int getRange(){
		return range;
	}

	public void setMaster(player p){
		master = p;
	}

	public bool hasFired(){
		return fired;
	}

	// Default weapons are not passive
	public virtual bool isPassive(){
		return false;
	}

	public void setTargetPos(Vector2 newPos){
		this.targetPosition = newPos;
	}

	public void setShotsPlanned(int num){
		shotsPlanned = num;
	}

	public string getDescription(){
		return description;
	}
}