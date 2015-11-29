using UnityEngine;
using System.Collections;

public class deflectorShield : weapon { 

	private static int maxDamage = 3;

	public deflectorShield():base("Deflector Shield","field","Deflector Shield makes you immune" +
		"to all damage less or equal to " + maxDamage.ToString(),0,0,0,isDefensive: true){
	}

	void Update(){
		print (this.isDefeniveWeapon());
	}

	public override bool blockDamage(int amount, weapon source){
		bool blocked = amount <= maxDamage;
		if (blocked) {
			print ("Reduced damage from " + amount.ToString () + " to 0!");
		} else {
			print ("Block failed!");
		}
		return blocked;
	}
}
