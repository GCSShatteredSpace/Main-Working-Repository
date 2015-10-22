using UnityEngine;

public class action {
	public Vector2 movement;
	public Vector2 attack;
	public int weaponId;
	public Vector2 extraMovement;	// You get extraMovement when you fire a weapon with recoil
	// Movement points to where you are at the end of turn
	// Use movement-extraMovement to find out where you were when you fired the weapon
}