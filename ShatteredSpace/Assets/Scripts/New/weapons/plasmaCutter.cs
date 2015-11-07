using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class plasmaCutter : weapon
{
    static private int DAMAGE = 10;
    static private int RANGE = 2;
    static private int DELAY = 1;
    
    bool overheat = false;
    bool weaponOn = false;
    int weaponHit = 0;
    int time;
    int turn;
    // The maximum times you can attack before overheating
    int overheatCapacity = 1;

    // A passive weapon
    public plasmaCutter()
        : base("plasmaCutter", "plasmaCutter", 6, 0, 0, 0)
    {
    }

    public override bool isPassive(){
        return true;
    }

    void Awake(){
        // This part is necessary for any spawned prefab
        // This will change to "gameController(Clone)" if we decide to instantiate the gameController
        GameObject gameController = GameObject.Find ("gameController");
        tManager = gameController.GetComponent<turnManager> ();
        bManager = gameController.GetComponent<boardManager> ();
        SS = gameController.GetComponent<functionManager> ();

        time = tManager.getTime();
        turn = tManager.getTurn();
    }
    
    void Update()
    {
        if (tManager.getTurn () != turn){
            if (tManager.getTime () != time){
                if (tManager.getTime () == -1){
                    // End of current turn!
                    weaponOn = false;
                    overheat = (weaponHit>=overheatCapacity);
                    weaponHit = 0;
                } else if (weaponOn){
                    generateDamage();
                }
                time = tManager.getTime();
            }
        }
    }

    // Pay attention! This weapon needs to be activated by calling fireWeapon
    // every turn when it is equiped
    public override void fireWeapon(Vector2 pos,int time)
    {
        weaponOn = true;
    }
    
    void generateDamage()
    {
        print("PlasmaCutter generates damage!");
        damageInfo newDamage = new damageInfo();   /// have a special mine damageinfo, with callback to signal if it hit??
        newDamage.damageAmount = this.getDamage();
        newDamage.attacker = this.getMaster();
        bool hit=false;

        Vector2 pos = this.getMaster().getPosition();
        // Generate a ring of damage around the player
        for (int i=0; i<6; i++)
        {
            hit=bManager.bomb(pos+SS.direction[i],newDamage)||hit;
        }

        if (hit){
            weaponHit+=1;
            if (weaponHit>=overheatCapacity) weaponOn = false;
        }
    }

    public override bool readyToFire(){
        return !overheat;
    }
    
}