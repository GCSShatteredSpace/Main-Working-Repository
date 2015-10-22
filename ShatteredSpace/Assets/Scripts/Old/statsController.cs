using UnityEngine;
using System.Collections;

public class statsController : MonoBehaviour {
	public int techCount;
	public int weaponPerTech;

	public weaponProperties[] weapons;
	public upgradeStats[] upgrades;
	
	public int maxWeapons;
	public int maxUpgrades;
	public int weaponCost;

	public int startingEnergy;
	public int turretEnergy;

	public int turretDamage;
	public int turretHealth;

	public Vector2[] directions;
	
	
	public weaponProperties updateWeaponStats(int weaponIndex, bool[] available){
		int i, l = available.Length;
		weaponProperties w = weapons[weaponIndex];
		for(i=0;i<l;i++){
			if (available[i]){
				w=w.add(w,upgrades[i].upgrade);
			}
		}
		return w;
	}
}
