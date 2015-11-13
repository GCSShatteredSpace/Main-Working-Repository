using UnityEngine;
using System.Collections;



public class AnimationController : MonoBehaviour {
    public Transform particleEffect;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    public void explode(Vector2 pos, Quaternion rot)
    {
        GameObject a=GameObject.Instantiate(particleEffect.gameObject, pos, rot) as GameObject;
    }
}
