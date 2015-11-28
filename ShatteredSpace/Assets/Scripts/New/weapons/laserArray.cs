using UnityEngine;
using System.Collections;

public class laserArray : weapon { 

	public laserArray():base("Anti-missile Laser Array","particle","Anti-missile Laser Array",0,0,1,1,hasOverheat: true,isDefensive: true){
	}

	public override bool blockDamage(int amount, weapon source){
		if (fireCount >= overheatCapacity) {
			print ("Laser array overheated");
			return false;
		}
		bool blocked = false;
		int damage = amount;

		if (source.getTechnology() == "explosive" && source.getName() != "Mine") {
			blocked = true;
			damage = source.getSplashDamage();
			this.getMaster().takeDamage(damage);
		}else if (source.getTechnology() == "momentum"){
			blocked = true;
			damage = 0;
		}
		if (blocked) {
			print ("Reduced damage from " + amount.ToString () + " to " + damage.ToString () + "!");
		} else {
			print ("Block failed!");
		}
		fireCount += 1;
		firedInTurn = true;
		return blocked;
	}
}
