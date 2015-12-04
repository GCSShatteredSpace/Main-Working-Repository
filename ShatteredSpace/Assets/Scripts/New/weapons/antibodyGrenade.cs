using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class antibodyGrenade : weapon { 

	public List<Vector2> bombPos = new List<Vector2> ();
	public int maxBombs = 1;
	static private int SPLASHDAMAGE = 1;

	public antibodyGrenade():base("Antibody Grenade","explosive","Antibody Grenade: Cling onto players that are directly hit, unabling them to move the next turn.",4,2,-1,1){
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
		newDamage.weaponFired = this;
		newDamage.type = "direct";
		// Creating the central damage
		bManager.bomb (pos,newDamage);
		// Creating the splatter damage
		newDamage.type = "splash";
		newDamage.damageAmount = SPLASHDAMAGE;
		for (int i=0; i<6; i++) {
			bManager.splashBomb(pos+SS.direction[i],newDamage);	
		}
		
		// Since the damage is generated, the weapon can take a rest
		this.setFired(false);
		this.getMaster().weaponHit ();
	}

	public override void directHit(player p){
		p.freezeMovement ();
	}

	public override int getSplashDamage ()
	{
		return SPLASHDAMAGE;
	}
}
