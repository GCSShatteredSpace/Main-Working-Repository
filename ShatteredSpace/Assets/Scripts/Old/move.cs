using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Just a public class that packs all of a players' actions in one type

public class move : MonoBehaviour {
		public List<Vector2>attack=new List<Vector2>();
		public List<Vector2>movement = new List<Vector2>();
		public List<int>weapon = new List<int>();
		public List<Vector2>extra = new List<Vector2>();

		public move(List<Vector2>atk,List<Vector2>mov,List<int>wpn,List<Vector2>ext){
			attack = atk;
			movement = mov;
			weapon = wpn;
			extra = ext;
		}

	public void clear(){
		attack.Clear();
		movement.Clear ();
		weapon.Clear ();
		extra.Clear ();
	}
}
