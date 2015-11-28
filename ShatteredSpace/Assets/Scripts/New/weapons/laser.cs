using UnityEngine;
using System.Collections;

public class laser : weapon {
	public laser():base("Laser","particle","No delay but overheats when fired.",4,4,0,1,hasOverheat: true){
	}
}
