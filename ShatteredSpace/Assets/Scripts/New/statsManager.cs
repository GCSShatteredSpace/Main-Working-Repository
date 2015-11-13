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

	public int mapSize;

	public List<weapon> weapons;

	void Start(){
		// Momentum weapons 0~3
		weapons.Add (new blaster ());
		weapons.Add (new sniperCannon ());
		weapons.Add (new minigun ());
		weapons.Add (new blastArray ());

		// Explosive weapons 4~7
		weapons.Add (new grenade());
		weapons.Add (new mine());
		weapons.Add (new antibodyGrenade());
		weapons.Add (new combustionThruster());

		// Particle weapons 8~11
		weapons.Add (new laser());
		weapons.Add (new laserArray());
		weapons.Add (new particleBeam());
		weapons.Add (new plasmaCutter());

		// Field weapons 12~15
		weapons.Add (new gravityTrap());
		weapons.Add (new deflectorShield());
		weapons.Add (new shockCannon());
		weapons.Add (new thermalField());
	}
}
