using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mine : weapon
{
	static private int DAMAGE = 10;
	static private int RANGE = 2;
	static private int DELAY = 1;
	static private int MAX_DURATION = 3;

	static private int SPLASH_DAMAGE = 5;
	
	int time = -1;

	[SerializeField] GameObject mineModel;
	List<GameObject> mines = new List<GameObject>();
	List<Vector2> minePos = new List<Vector2>();
	List<int> deployedTurn = new List<int>();

	// A very special weapon
	public mine()
		: base("Mine", "explosive", "mine", 10, 1, 0, 1)
	{
	}

	void Awake(){
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		tManager = gameController.GetComponent<turnManager> ();
		bManager = gameController.GetComponent<boardManager> ();
		SS = gameController.GetComponent<functionManager> ();
	}
	
	void Update()
	{
		if (tManager.getTime ()!=time && mines.Count!=0)
		{	
			print("I'm a Mine!");
			for(int i=0;i<mines.Count;i++)
			{
				print("Mine " + i.ToString() + " is generating damage!");
				if(tManager.getTurn() > deployedTurn[i] + MAX_DURATION){
					// Pretend it hits something so it self destructs
					generateDamage(i,true);
				}else if (tManager.getTurn()>deployedTurn[i]){
					generateDamage(i,false);
				}

			}
			time = tManager.getTime();
		}
	}

	public override void fireWeapon(Vector2 pos,int time)
	{
		print ("Mine deployed!");

		// This weapon is special cause it only cares about turns, not the step time
		GameObject newMine = Instantiate (mineModel, SS.hexPositionTransform (pos), Quaternion.identity) as GameObject;
		mines.Add (newMine);
		deployedTurn.Add (tManager.getTurn());
		minePos.Add (pos);
		print ("mines.Count="+mines.Count.ToString());
		// Since the weapon takes care of itself, the player don't have to wait for it to hit
		this.getMaster().weaponHit();
		this.setShotsPlanned(0);
	}
	
	void generateDamage(int index,bool hit)
	{
		print("Mine generates damage!");
		damageInfo newDamage = new damageInfo();   /// have a special mine damageinfo, with callback to signal if it hit??
		newDamage.damageAmount = this.getDamage();
		newDamage.attacker = this.getMaster();
		newDamage.weaponFired = this;
		newDamage.type = "direct";

		hit = bManager.bomb(minePos[index], newDamage)||hit;

		newDamage.type = "splash";
		// Now generate the splash damage
		newDamage.damageAmount = SPLASH_DAMAGE;
		for (int i=0; i<6; i++)
		{
			hit=bManager.bomb(minePos[index]+SS.direction[i],newDamage)||hit;
		}

		if (hit){
			print ("Self destruct!");
			minePos.RemoveAt(index);
			Destroy(mines[index]);
			mines.RemoveAt(index);
			deployedTurn.RemoveAt(index);
		}
	}
	
}