using UnityEngine;
using System.Collections;

public class damageInfo : MonoBehaviour {
	public int damageAmount;
	public player attacker;
	public weapon weaponFired;
	public Vector2 push = Vector2.zero; // If the damage contains a force that pushes or pulls the player

	public void applyToPlayer(player p){
		p.takeDamage(damageAmount);
        if (push!= Vector2.zero){
            p.addMomentum(push);
        }
	}

}
