using UnityEngine;
using System.Collections;

//The display pannel
//Nothing really interesting

public class displayer : MonoBehaviour {

	public GameObject statsCtrl;
	public statsController dataBase;

	public int current;

	public TextMesh weaponName;	
	public TextMesh range;
	public TextMesh damage;
	public TextMesh special;

	public shopButton button;

	void Start () {
		statsCtrl = GameObject.Find("gameData");
		dataBase = statsCtrl.GetComponent<statsController> ();
	}


	public void display (int weapon) {
		current = weapon;
		weaponName.text = dataBase.weapons [weapon].weaponName;
		range.text = dataBase.weapons [weapon].range.ToString();
		damage.text = dataBase.weapons [weapon].damage.ToString();
		special.text = dataBase.weapons [weapon].special;
		button.weaponId = weapon;
	}
}
