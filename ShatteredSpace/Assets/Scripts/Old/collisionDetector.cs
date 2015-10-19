using UnityEngine;
using System.Collections;

public class collisionDetector : MonoBehaviour {
	public Vector2 direction;
	public Vector2 result;
	public Vector2 position;
	public bool collision;
	public bool cantMove;
	[SerializeField]GameController gctrl;

	void Start(){
		StartCoroutine (wait());
	}

	void OnTriggerEnter(Collider other) {
		//
		//the collider doesn't work on players, just the colliders they generate
		if (other.gameObject.tag == "Player"||other.gameObject.tag == "damage"||other.gameObject.tag == "pickup") 
						return;
		//this might need to change!
		//also, using layers is a better idea!
		//
		print ("collision");
		collision = true;
		if (other.gameObject.GetComponent<collisionDetector> () != null) {
			print ("collision with other player");
			collisionDetector otherMovement = other.gameObject.GetComponent<collisionDetector> ();
			result = otherMovement.direction;
		} else {
			print ("collision with still object");
			result = direction * -1;
		}
	}

	IEnumerator wait(){
		yield return new WaitForSeconds (0.02f);
		Destroy (this.gameObject);
	}
}
