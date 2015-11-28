using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class expPanel : MonoBehaviour {

	[SerializeField]GameObject display;
	public void showDisplay(){
		display.SetActive (true);
	}

	public Slider getSlider(){
		return display.GetComponent<Slider> ();
	}
}
