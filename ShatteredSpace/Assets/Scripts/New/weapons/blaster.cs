using UnityEngine;
using System.Collections;

public class blaster : weapon { 
	// The most basic weapon
	// Need to add recoil push after upgrades
	[SerializeField] GameObject shot;
	[SerializeField] GameObject explosion;

	public blaster():base("Blaster","momentum","The simplest weapon... ever",5,5,1,1){
	}

	public override void fireWeapon(Vector2 target, int time)
	{
		base.fireWeapon(target, time);
		print (SS.hexPositionTransform(this.getMaster().getPosition())+this.projOffset);
		GameObject instance = Instantiate(shot, SS.hexPositionTransform(this.getMaster().getPosition())+this.projOffset, Quaternion.identity) as GameObject;
		Projectile p = instance.GetComponent<Projectile>();
		p.setTarget(SS.hexPositionTransform(target), this.getDelay());
		proj = p;
	}

	public override void projEnd()
	{
		bManager.addExplosion(explosion, proj.gameObject.GetComponent<Rigidbody2D>().position);
		base.projEnd();
	}
}
