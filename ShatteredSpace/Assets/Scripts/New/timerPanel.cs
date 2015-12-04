using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class timerPanel : MonoBehaviour {
	[SerializeField] GameObject timer;

	public Image getImage(){
		return timer.GetComponent<Image> ();
	}

}
