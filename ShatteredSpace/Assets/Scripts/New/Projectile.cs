using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Vector2 target;
	private int time;
	[SerializeField] statsManager database;

	// Use this for initialization
	void Awake () {
		database = GameObject.Find ("stats").GetComponent<statsManager>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setTarget(Vector2 pos, float delay)
	{
		float t = delay * database.stepTime;
		Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
		float xcomp = (pos.x - r.position.x) / t;
		float ycomp = (pos.y - r.position.y) / t;
		print ("Shot fired to "+xcomp+","+ycomp);
		r.velocity = new Vector2 (xcomp, ycomp);
	}


}
