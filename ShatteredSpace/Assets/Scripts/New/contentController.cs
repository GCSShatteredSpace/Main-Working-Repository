using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class contentController : MonoBehaviour {

	RectTransform rt;
	Text txt;
	
	void Start () {
		rt = gameObject.GetComponent<RectTransform>(); // Acessing the RectTransform 
		txt = gameObject.GetComponent<Text>(); // Accessing the text component
	}
	
	void Update () {
		rt.sizeDelta = new Vector2(rt.rect.width, txt.preferredHeight); // Setting the height to equal the height of text
	}
}
