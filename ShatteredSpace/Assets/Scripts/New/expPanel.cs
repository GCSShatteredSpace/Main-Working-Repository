using UnityEngine;
using System.Collections;

public class expPanel : MonoBehaviour {

	[SerializeField]GameObject display;
	public void showDisplay(){
		display.SetActive (true);
	}
}
