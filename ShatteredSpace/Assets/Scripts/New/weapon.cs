using UnityEngine;
using System;

public class weapon : MonoBehaviour
{
	[SerializeField]turnManager tManager;
	boardManager bManager;

	// There is a protection level thing that I don't know how to sort out
    public int damage;
    public int range;
    public int delay;	// The number of steps damage is generated after the weapon fired
	public player master; // The player that the weapon belong to...
	public string weaponName; 
    public string description; 
	private int fireTime; 
	private bool canFire;

	private bool fired;
	private Vector2 targetPosition;
    /*Note: intentionally did not include too many required fields in the abstract class. Can add more later if necessary */ 
    
	public weapon(string name, string description,int damage, int range, int delay) /* initiates a basic weapon object. Can override in a 
                                                                                    subclass to construct a weapon with more fields */                                        
	{
        this.weaponName = name;
        this.description = description;
        this.damage = damage;
        this.range = range;
        this.delay = delay;
    }

	void Awake(){
		// This part is necessary for any spawned prefab
		// This will change to "gameController(Clone)" if we decide to instantiate the gameController
		GameObject gameController = GameObject.Find ("gameController");
		tManager = gameController.GetComponent<turnManager> ();
		bManager = gameController.GetComponent<boardManager> ();
	}

	void Update()
	{
		if (tManager.getTime() == fireTime && fired) {
			print ("FireTime = "+fireTime.ToString());
			generateDamage();
		}
	}

	public void fireWeapon(Vector2 pos,int time){
		print ("Weapon fired!");
		fired = true;
		fireTime = time+delay;
		targetPosition = pos;
	}

	void generateDamage(){
		print ("generate damage!");
		damageInfo newDamage = new damageInfo();
		newDamage.damageAmount = damage;
		newDamage.attacker = master;
		bManager.bomb (targetPosition,newDamage);
		// Since the damage is generated, the weapon can take a rest
		fired = false;
		master.weaponHit ();
	}

	public int getFireTime()
	{
		return this.fireTime;
	}
	
    public int getDamage()
    {
        return this.damage; 
    }

    public bool isInRange(int distance)
    {
        return distance<=range && distance!=0; 
    }

	public bool readyToFire(){
		return canFire;
	}

    public float getDelay()
    {
        return this.delay;
    }

    public string getName()
    {
        return this.weaponName;
    }

    public string getInfo()
    {
        return this.description; 
    }
}
