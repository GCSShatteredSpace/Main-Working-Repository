using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class grenade : weapon { 
	// The basic weapon for explosive technology
	// Generates AOE splash damage
	// Upgrades: Increased range, magnetic grenade, multiple grenades
	
	public List<Vector2> bombPos = new List<Vector2> ();
	public int maxBombs;
	static private int SPLASHDAMAGE = 2;
	
	public grenade():base("Grenade","Has splash damage of 2",4,4,-1,1
	                      // -1 stands for delay until end of turn but actually we'll never use it
	                      // We can't even say if time==-1 because that's when turn actually end
	                      // We are looking for a time when players stop moving but turn hasn't really ended
	                      ){ 
		maxBombs = 1;
	}

	void Update(){
		if (tManager.endOfPlayerMovement() && this.hasFired()) {	// If it's time for bombs to fall
			print ("Grenades fall!");
			foreach(Vector2 pos in bombPos){
				generateDamage(pos);
			}
			bombPos.Clear();
			this.setFired(false);
		}
	}

	public override void fireWeapon(Vector2 pos,int time){
		print ("Grenade in the air!");
		this.setFired(true);
		bombPos.Add(pos);

		this.setShotsPlanned (0);
	}

	// Overwrite!
	void generateDamage(Vector2 pos){
		print ("generate grenade damage!");
		damageInfo newDamage = new damageInfo();
		newDamage.damageAmount = this.getDamage();
		newDamage.attacker = this.getMaster();
		// Creating the central damage
		bManager.bomb (pos,newDamage);
		// Creating the splatter damage
		newDamage.damageAmount = SPLASHDAMAGE;
		for (int i=0; i<6; i++) {
			newDamage.push=SS.direction[i];
			bManager.bomb(pos+SS.direction[i],newDamage);	
		}

		// Since the damage is generated, the weapon can take a rest
		this.setFired(false);
		this.getMaster().weaponHit ();
	}


	
}