using UnityEngine;
using System.Collections;

public class barrier : MonoBehaviour {

	statsManager database;
	private static float RAISED = -0.8f;
	private static float LOWERED = 0f;
	private static int STEPS = 30;

	void Start(){
		database = GameObject.Find ("stats").GetComponent<statsManager> ();
	}

	public IEnumerator moveBarrier(bool rise){
		Vector3 targetPos;
		if (rise) {
			targetPos = new Vector3 (this.transform.position.x,
		                             this.transform.position.y,
		                             RAISED);
		} else {
			targetPos = new Vector3 (this.transform.position.x,
			                         this.transform.position.y,
			                         LOWERED);
		}
		for (int i = 0; i < STEPS; i++){
			this.transform.position = Vector3.MoveTowards(this.transform.position,targetPos,Mathf.Abs(RAISED/STEPS));
			yield return new WaitForFixedUpdate();
		}
	}

}
