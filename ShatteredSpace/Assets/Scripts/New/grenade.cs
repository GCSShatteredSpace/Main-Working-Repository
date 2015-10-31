using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class grenade : weapon { 
	// The basic weapon for explosive technology
	// Generates AOE splash damage
	// Upgrades: Increased range, magnetic grenade, multiple grenades
	
	public List<Vector2> bombPos = new List<Vector2> ();
	public int maxBombs;
	public int splatter;
	
	public grenade():base("grenade","grenade",4,4,-1
	                      // -1 stands for delay until end of turn but actually we'll never use it
	                      // We can't even say if time==-1 because that's when turn actually end
	                      // We are looking for a time when players stop moving but turn hasn't really ended
	                      ){ 
		maxBombs = 1;
	}

	// Overwrite!
	void update(){
		if (tManager.endOfPlayerMovement() && fired) {	// If it's time for bombs to fall
			print ("Grenades fall!");
			foreach(Vector2 pos in bombPos){
				generateDamage(pos);
			}
		}
	}

	// Overwrite!
	public void fireWeapon(Vector2 pos,int time){
		print ("Grenade in the air!");
		fired = true;
		bombPos.Add(pos);
	}

	// Overwrite!
	public void generateDamage(Vector2 pos){
		print ("generate grenade damage!");
		damageInfo newDamage = new damageInfo();
		newDamage.damageAmount = damage;
		newDamage.attacker = master;
		// Creating the central damage
		bManager.bomb (pos,newDamage);
		// Creating the splatter damage
		newDamage.damageAmount = splatter;
		for (int i=0; i<6; i++) {
			newDamage.push=SS.direction[i];
			bManager.bomb(pos+SS.direction[i],newDamage);	
		}

		// Since the damage is generated, the weapon can take a rest
		fired = false;
		master.weaponHit ();
	}
	
}