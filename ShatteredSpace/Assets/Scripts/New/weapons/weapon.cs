using UnityEngine;
using System;

public class weapon : MonoBehaviour
{
	public turnManager tManager;
	public boardManager bManager;
	public functionManager SS;

	int turn;
	int time;

	// There is a protection level thing that I don't know how to sort out
	int damage;
	int range;
	int delay;	// The number of steps damage is generated after the weapon fired
	player master; // The player that the weapon belong to...
	string technology;
	string weaponName; 
	string description; 
	int fireTime; 
	public bool overheated = false;
	bool hasOverheat;
	public int overheatCapacity = 1;
	bool isDefensive;

	int playerExpGain = 2;
	int reducedPlayerExpGain = 1;	// This happens when the oppoent blocked the attack

	int shotsPlanned;
	int numOfShots;	// Maximum time you can fire this weapon
	
	public bool fired;
	public bool firedInTurn;
	public int fireCount;
	Vector2 targetPosition;

	
	public weapon(string name, string technology, string description,
		int damage, int range, int delay, int shots = 1, 
	    bool hasOverheat = false, bool isDefensive = false) /* initiates a basic weapon object. Can override in a 
                                                                                    subclass to construct a weapon with more fields */                                        
	{
		this.weaponName = name;
		this.technology = technology;
		this.description = description;
		this.damage = damage;
		this.range = range;
		this.delay = delay;
		this.numOfShots = shots;
		this.hasOverheat = hasOverheat;
		this.isDefensive = isDefensive;
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
		if (tManager.getTime() == fireTime && fired) {
			print ("FireTime = "+fireTime.ToString());
			generateDamage();
		}
		if (tManager.getTurn () != turn){
			if (tManager.getTime () != time){
				if (tManager.getTime () == -1){
					// End of current turn!
					if (!firedInTurn){
						fireCount = 0;
					}
					this.overheated = (hasOverheat && (fireCount >= overheatCapacity));
					firedInTurn = false;
				}
				time = tManager.getTime();
			}
		}
	}
	
	public virtual void fireWeapon(Vector2 pos,int time){
		print ("Weapon fired!");
		fired = true;
		firedInTurn = true;
		fireCount += 1;
		// Cause damage is generated at the end of current step
		fireTime = time + delay;
		targetPosition = pos;

		// The turn has started, so reset for the next turn
		setShotsPlanned(0);
	}

	public void generateDamage(){
		print ("generate damage!");
		damageInfo newDamage = new damageInfo();
		newDamage.damageAmount = getDamage();
		newDamage.attacker = master;
		newDamage.weaponFired = this;
		newDamage.type = "direct";
		bool hit = bManager.bomb (targetPosition,newDamage);
		// Since the damage is generated, the weapon can take a rest
		fired = false;
		master.weaponHit ();
	}

	public virtual void hitPlayer(string damageType, player p){
		master.gainExp(technology,playerExpGain);
		directHit (p);
	}

	public virtual void hitPlayerBlocked(string damageType, player p){
		master.gainExp(technology,reducedPlayerExpGain);
	}

	public int getFireTime()
	{
		return this.fireTime;
	}

	// This can be modified for random damage, upgraded damage, etc.
	public virtual int getDamage()
	{
		return this.damage; 
	}

	// Can be overrided by special ranged weapon like sniperCannon
	public virtual bool isInRange(int distance)
	{
		return distance<=range && distance!=0; 
	}

	// Can be overrided by weapons with overheat
	public virtual bool readyToFire(){
		return numOfShots > shotsPlanned && !overheated;
	}

	public void planToFire(){
		shotsPlanned += 1;
		print ("ShotsPlanned: " + shotsPlanned.ToString());
	}

	public void cancelFire(){
		shotsPlanned -= 1;
		print ("ShotsPlanned: " + shotsPlanned.ToString());
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

	public void setFired(bool value)
	{
		fired = value;
	}

	public player getMaster(){
		return master;
	}

	public int getRange(){
		return range;
	}

	public void setMaster(player p){
		master = p;
	}

	public bool hasFired(){
		return fired;
	}

	public string getTechnology(){
		return technology;
	}

	// Default weapons are not passive
	public virtual bool isPassive(){
		return false;
	}

	public virtual void directHit(player p){
		return;
	}

	public virtual int getSplashDamage(){
		return 0;
	}

	public virtual bool blockDamage(int amount, weapon source){
		return false;
	}

	public bool isDefeniveWeapon(){
		return isDefensive;
	}

	public void setTargetPos(Vector2 newPos){
		this.targetPosition = newPos;
	}

	public void setShotsPlanned(int num){
		print ("ShotsPlannedSetTo " + num.ToString());
		shotsPlanned = num;
	}

	public string getDescription(){
		return description;
	}
}
