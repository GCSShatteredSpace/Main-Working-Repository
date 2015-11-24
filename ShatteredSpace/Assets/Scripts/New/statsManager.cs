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

    public int mapSize;

	public List<weapon> weapons;

	void Start(){
		// Momentum weapons 0~3
		weapons.Add (this.gameObject.AddComponent<blaster> ());
		weapons.Add (this.gameObject.AddComponent<sniperCannon> ());
		weapons.Add (this.gameObject.AddComponent<minigun> ());
		weapons.Add (this.gameObject.AddComponent<blastArray> ());

		// Explosive weapons 4~7
		weapons.Add (this.gameObject.AddComponent<grenade>());
		weapons.Add (this.gameObject.AddComponent<mine>());
		weapons.Add (this.gameObject.AddComponent<antibodyGrenade>());
		weapons.Add (this.gameObject.AddComponent<combustionThruster>());

		// Particle weapons 8~11
		weapons.Add (this.gameObject.AddComponent<laser>());
		weapons.Add (this.gameObject.AddComponent<laserArray>());
		weapons.Add (this.gameObject.AddComponent<particleBeam>());
		weapons.Add (this.gameObject.AddComponent<plasmaCutter>());

		// Field weapons 12~15
		weapons.Add (this.gameObject.AddComponent<gravityTrap>());
		weapons.Add (this.gameObject.AddComponent<deflectorShield>());
		weapons.Add (this.gameObject.AddComponent<shockCannon>());
		weapons.Add (this.gameObject.AddComponent<thermalField>());
	}
}
