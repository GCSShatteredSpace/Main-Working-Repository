using UnityEngine;
using System.Collections;
namespace Completed{
public class Loader : MonoBehaviour {
		public GameObject gameManager;          //GameManager prefab to instantiate.
		public GameObject soundManager;         //SoundManager prefab to instantiate.

		//I have no idea why we need this loader...
		//But I guess it has something to do with scene transitions so we better not touch it!
		
		void Awake ()
		{
			//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
			if (gameManager.activeInHierarchy==false)
				
				//Instantiate gameManager prefab
				Instantiate(gameManager);
			
			//Check if a SoundManager has already been assigned to static variable GameManager.instance or if it's still null
			//if (soundManager.instance == null)
				
				//Instantiate SoundManager prefab
			//	Instantiate(soundManager);
		}
}
}