using UnityEngine;
using System.Collections;

public class damageInfo : MonoBehaviour {
	public int damageAmount;
	public player attacker;
	public weapon weaponFired;
	public Vector2 push = new Vector2(); // If the damage contains a force that pushes or pulls the player

	public void applyToPlayer(player p){
		p.takeDamage(damageAmount);
	}

}
