using UnityEngine;
using System.Collections;

public class damageGlobe : MonoBehaviour {

	public weaponProperties stats;
	public Renderer rend;
	public GameObject origin;

	public GameObject action; 
	public actionController actrl;

	public float t;

	void Start () {
		action = GameObject.Find("actionController");
		actrl = action.GetComponent<actionController> ();
		stats = this.GetComponent<weaponProperties> ();
		rend = GetComponent<Renderer> ();
		if (stats.damage <0) {
			rend.material.color = Color.green;		
		} else {
			rend.material.color = Color.red;		
		}
		t = actrl.time;
	}



	void Update () {
		if (this.gameObject.tag=="damage"&&(actrl.time >= t + stats.lifeTime||actrl.time==-1)) { //if this is a gernerated instance as a damage globe, blow up after lifetime
			if(origin.tag == "player"){

			}
			Destroy(this.gameObject);		
		}
	}

}
