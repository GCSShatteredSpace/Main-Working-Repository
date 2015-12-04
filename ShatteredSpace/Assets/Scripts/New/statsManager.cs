using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class blastShield: MonoBehaviour{

	private static int MAXTURN = 2;

	boardManager board;
	public List<Vector2> barrierPos;
	List<GameObject> barriers = new List<GameObject>();
	bool activated;
	int activatedTurn;
	
	public bool hit(Vector2 pos){
		return barrierPos.Contains (pos);
	}

	public bool isActivated(){
		return activated;
	}

	public void addBarrier(GameObject obj){
		barriers.Add (obj);
	}

	public void addListPos(List<Vector2> listPos){
		barrierPos = listPos;
	}

	public void rise(){
		activated = true;
		foreach (GameObject b in barriers) {
			print (b.GetComponent<barrier>());
			StartCoroutine (b.GetComponent<barrier>().moveBarrier (true));
		}
		foreach (Vector2 v in barrierPos) {
			board.setBarrier(v,true);
		}
	}

	public void lower(){
		activated = false;
		foreach (GameObject b in barriers) {
			print (b.GetComponent<barrier>());
			StartCoroutine (b.GetComponent<barrier>().moveBarrier (false));
		}
		foreach (Vector2 v in barrierPos) {
			board.setBarrier(v,false);
		}
	}

	public void update(){
		if (activated){
			activatedTurn += 1;
			if (activatedTurn >= MAXTURN){
				activatedTurn = 0;
				lower();
			}
		}
	}

	public void setBoard(boardManager b){
		board = b;
	}
}

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
	public int maxSteps;

	public List<blastShield> blastShields = new List<blastShield>(); // Barrier numbers may vary
	public List<Vector2> turretSpawnPoints;

    public int mapSize;
	public int turnTimer;

	public List<weapon> weapons;

	void Awake(){

		blastShield b;
		List<Vector2> posList = new List<Vector2> ();
		posList.Add (new Vector2 (3, 1));
		posList.Add (new Vector2 (3, 0));
		posList.Add (new Vector2 (4, -1));
		posList.Add (new Vector2 (5, -2));
		b = this.gameObject.AddComponent<blastShield> ();
		b.addListPos (posList);
		blastShields.Add (b);
		posList = new List<Vector2> ();

		posList.Add (new Vector2 (0, 3));
		posList.Add (new Vector2 (-1, 4));
		posList.Add (new Vector2 (1, 3));
		posList.Add (new Vector2 (2, 3));
		b = this.gameObject.AddComponent<blastShield> ();
		b.addListPos (posList);
		blastShields.Add (b);
		posList = new List<Vector2> ();

		posList.Add (new Vector2 (3, -5));
		posList.Add (new Vector2 (3, -4));
		posList.Add (new Vector2 (3, -3));
		posList.Add (new Vector2 (4, -3));
		b = this.gameObject.AddComponent<blastShield> ();
		b.addListPos (posList);
		blastShields.Add (b);
		posList = new List<Vector2> ();

		posList.Add (new Vector2 (-2, -3));
		posList.Add (new Vector2 (-1, -3));
		posList.Add (new Vector2 (0, -3));
		posList.Add (new Vector2 (1, -4));
		b = this.gameObject.AddComponent<blastShield> ();
		b.addListPos (posList);
		blastShields.Add (b);
		posList = new List<Vector2> ();

		posList.Add (new Vector2 (-3, -1));
		posList.Add (new Vector2 (-3, 0));
		posList.Add (new Vector2 (-4, 1));
		posList.Add (new Vector2 (-5, 2));
		b = this.gameObject.AddComponent<blastShield> ();
		b.addListPos (posList);
		blastShields.Add (b);
		posList = new List<Vector2> ();

		posList.Add (new Vector2 (-4, 3));
		posList.Add (new Vector2 (-3, 3));
		posList.Add (new Vector2 (-3, 4));
		posList.Add (new Vector2 (-3, 5));
		b = this.gameObject.AddComponent<blastShield> ();
		b.addListPos (posList);
		blastShields.Add (b);
		posList = new List<Vector2> ();

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
