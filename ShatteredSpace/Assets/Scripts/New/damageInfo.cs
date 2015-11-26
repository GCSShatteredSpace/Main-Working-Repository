﻿using UnityEngine;
using System.Collections;

public class damageInfo : MonoBehaviour {
	// Damage generated by turret is a edge case
	public bool isTurretDamage = false;

	public int damageAmount;
	public player attacker;
	public weapon weaponFired;
    public string type; // direct or splash damage
	public Vector2 push = Vector2.zero; // If the damage contains a force that pushes or pulls the player
    const int exp = 1;

	public void applyToPlayer(player p){
		p.takeDamage(damageAmount);
        if (push!= Vector2.zero){
            p.addMomentum(push);
        }
		if (!isTurretDamage) {
			weaponFired.hitPlayer (type);
		}
	}

    public void applyToTurret(turret t){
        t.takeDamage(damageAmount);
        if (t.getEnergy() <= 0){
            attacker.gainExp(weaponFired.getTechnology(),exp);
        }
    }
}
