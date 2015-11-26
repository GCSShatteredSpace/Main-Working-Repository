using UnityEngine;
using System.Collections;

public class sniperCannon : weapon { 
    // Long range, high damage weapon
    // Doesn't work at short range
    static private int maxRange = 8;
    static private int minRange = 5;

    public sniperCannon():base("Sniper Cannon","momentum","Cannot shoot targets within " + minRange.ToString()+ " range.",8,maxRange,1,1){
    }

    public override bool isInRange(int distance){
        return distance<=maxRange && distance>=minRange;
    }
}