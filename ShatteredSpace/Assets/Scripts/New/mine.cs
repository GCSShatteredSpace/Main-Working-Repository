using UnityEngine;
using System.Collections;

public class mine : weapon
{
    static private int DAMAGE = 10;
    static private int RANGE = 2;
    static private int DELAY = 1;
    static private int MAX_DURATION = 3;
    // The most basic weapon
    // Need to add recoil push after upgrades
    public mine()
        : base("mine", "mine", 5, 5, 1)
    {
    }

    void Update()
    {
        if (tManager.getTime() > this.getFireTime() && fired) //need to be able to access these elements of the superclass? ... is there super keyword? 
        {
            if (tManager.getTime() < this.getFireTime() + MAX_DURATION)
            {
                print("mine is generating damage");
                generateDamage();
            }
            else
            {
                fired == false;
            }
        }
    }

    void generateDamage()
    {
        print("generate damage!");
        damageInfo newDamage = new damageInfo();   /// have a special mine damageinfo, with callback to signal if it hit??
        newDamage.damageAmount = damage;
        newDamage.attacker = master;
        bManager.bomb(targetPosition, newDamage);
        // Since the damage is generated, the weapon can take a rest
        master.weaponHit();
    }


}
