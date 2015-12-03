using UnityEngine;
using System.Collections;

public class blastArray : weapon { 

	public bool weaponOn = false;
	// The ceilling and floor of possible damage reduction
	private static int MAX_REDUCTION = 4;
	private static int MIN_REDUCTION = 1;

	public blastArray():base("Anti-missile Blast Array","momentum","Reduces single momentum damage by 4",0,0,1,1,isDefensive:true){
	}

	public override bool blockDamage(int amount, weapon source){
		bool blocked = false;
		int damage = amount;
		if (source.getTechnology() == "explosive" && source.getName() != "Mine") {
			blocked = true;
			damage = source.getSplashDamage();
			this.getMaster().takeDamage(damage);
		}else if (source.getTechnology() == "momentum"){
			blocked = true;
			damage = Mathf.Max(0,amount - Random.Range(MIN_REDUCTION,MAX_REDUCTION + 1));
			this.getMaster().takeDamage(damage);
		}
		if (blocked) {
			print ("Reduced damage from " + amount.ToString () + " to " + damage.ToString () + "!");
		} else {
			print ("Block failed!");
		}
		return blocked;
	}

}
