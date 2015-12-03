using UnityEngine;
using System.Collections;

public class particleBeam : weapon { 
	public particleBeam():base("Particle Beam","particle","Long range, low random damage.",3,10,0,1,hasOverheat: true){
	}

	public override int getDamage ()
	{
		return Random.Range(0, base.getDamage ()) + 1;
	}
}
