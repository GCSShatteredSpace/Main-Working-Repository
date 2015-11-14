using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct FireInstance
{
	public bool generatedDamage;
	public int fireTime;
	public Vector2 targetPosition;
	
	public FireInstance(int fireTime, Vector2 pos)
	{
		this.fireTime = fireTime;
		if(pos == null){
			this.targetPosition = new Vector2(0, 0);
		}
		else{
			this.targetPosition = pos;
		}
		this.generatedDamage = false;
	}
}

public class minigun : weapon
{
	static private int DAMAGE = 2;
	static private int RANGE = 2;
	static private int DELAY = 1;
	static private int MAXSHOTS = 3;
	
	private List<FireInstance> shots;
	// The most basic weapon
	// Need to add recoil push after upgrades
	private int numDmgGenerated = 0;
	
	
	public minigun()
		: base("Minigun", "Can shoot " + MAXSHOTS.ToString() +" times in one turn", DAMAGE, RANGE, DELAY, MAXSHOTS)
	{
		shots = new List<FireInstance>();
	}
	
	void Update()
	{
		if (numDmgGenerated < shots.Count)
		{
			for (int i = 0; i < shots.Count; i++)
			{
				FireInstance shot = shots[i];
				if (tManager.getTime() == shot.fireTime && !shot.generatedDamage)
				{
					print("FireTime = " + shot.fireTime.ToString());
					setTargetPos(shot.targetPosition);
					generateDamage();
					shot.generatedDamage = false;
					print("dmg generated");
					numDmgGenerated = 0;
				}
			}
		}
	}
	
	public override void fireWeapon(Vector2 pos, int time)
	{
		print("*****Weapon fired!*********");
		int fireTime = time + DELAY;
		
		FireInstance shot = new FireInstance(fireTime, pos);
		shots.Add(shot);
		print(shots.Count.ToString());
		
		setShotsPlanned(0);
	}
}