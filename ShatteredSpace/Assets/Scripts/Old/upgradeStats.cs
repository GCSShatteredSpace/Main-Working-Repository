using UnityEngine;
using System.Collections;

public class upgradeStats : MonoBehaviour {

	public weaponProperties upgrade;
	public GameObject controller;
	public statsController sctrl;
	public string upgradeName;
	public bool[] isValid;
	public bool installed;

	public int damage;
	public int range;
	public int delay;
	public string special;
	public int spAmount;

	//I haven't examined this bit for a long time
	//Just pretend it doesn't exist
	//This is supposed to be somewhere to store info on all the upgrade

	void Start () {
		upgrade = this.gameObject.AddComponent <weaponProperties>();
		upgrade.damage = damage;
		upgrade.range = range;
		upgrade.delay = delay;
		upgrade.special = special;
		upgrade.spAmount = spAmount;
		controller = GameObject.Find("gameData");
		sctrl = controller.GetComponent<statsController> ();
		isValid = new bool[sctrl.maxWeapons];
	}

}
