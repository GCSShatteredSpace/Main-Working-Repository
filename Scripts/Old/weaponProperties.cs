using UnityEngine;
using System.Collections;

public class weaponProperties : MonoBehaviour {
	public int damage;
	public int range;
	public float delay;
	public string special;
	public int expReturn;
	public int spAmount;

	public float lifeTime;

	public string weaponName;
	public string description;

	//this is for adding the upgrade onto the weapon stats
	
	public weaponProperties add (weaponProperties original, weaponProperties upgrade){
		weaponProperties final = new weaponProperties();
		final.damage = original.damage + upgrade.damage;
		final.special = string.Concat (original.special, upgrade.special);
		final.spAmount = original.spAmount + upgrade.spAmount;
		final.range = original.range + upgrade.range;
		final.delay = original.delay + upgrade.delay;
		return final;
	}

	//it should be a construct function:

	public void giveValue (weaponProperties weapon){
		damage = weapon.damage;
		delay = weapon.delay;
		range = weapon.range;
		special = weapon.special;
		spAmount = weapon.spAmount;
		weaponName = weapon.weaponName;
		lifeTime = weapon.lifeTime;
		expReturn = weapon.expReturn;
	}
}
