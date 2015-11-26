using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class statsManager : MonoBehaviour {

	// These are all important game-related parameters
	public float tileSize;
	public float stepTime;
	public int turretRange;
	public int playerStartEnergy;
	public int turretStartEnergy;
    public int turretRespawnTime;
	public int turretDamage;
	public int upgradeExp;

    public int mapSize;

	public List<weapon> weapons;

	void Awake(){
		// Momentum weapons 0~3
		weapons.Add (this.gameObject.GetComponent<blaster> ());
		weapons.Add (this.gameObject.GetComponent<sniperCannon> ());
		weapons.Add (this.gameObject.GetComponent<minigun> ());
		weapons.Add (this.gameObject.GetComponent<blastArray> ());
									 
		// Explosive weapons 4~7	 
		weapons.Add (this.gameObject.GetComponent<grenade>());
		weapons.Add (this.gameObject.GetComponent<mine>());
		weapons.Add (this.gameObject.GetComponent<antibodyGrenade>());
		weapons.Add (this.gameObject.GetComponent<combustionThruster>());
									
		// Particle weapons 8~11	 
		weapons.Add (this.gameObject.GetComponent<laser>());
		weapons.Add (this.gameObject.GetComponent<laserArray>());
		weapons.Add (this.gameObject.GetComponent<particleBeam>());
		weapons.Add (this.gameObject.GetComponent<plasmaCutter>());
									 
		// Field weapons 12~15	   
		weapons.Add (this.gameObject.GetComponent<gravityTrap>());
		weapons.Add (this.gameObject.GetComponent<deflectorShield>());
		weapons.Add (this.gameObject.GetComponent<shockCannon>());
		weapons.Add (this.gameObject.GetComponent<thermalField>());
	}
}
