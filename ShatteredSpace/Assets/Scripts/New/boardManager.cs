using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


/*
 * boardManager - contains functions that manages the board
 */
public class boardManager : MonoBehaviour {

    public Vector2[] direction;
    const float ratio = 0.8666f; // Sqrt(3)/2
	float tileSize;
    [SerializeField] statsManager dataBase;
    [SerializeField] functionManager SS;    // Just call it SS for sake of laziness
    [SerializeField] Vector2[] turretSpawnPoint = new Vector2[5];
    [SerializeField] Vector2[] barrierSpawnPoint = new Vector2[12]; // Barrier numbers may vary

	[SerializeField] GameObject gameTile;

	tile[,] board;


    void Start(){
        tileSize = dataBase.tileSize;
		generateHexMap (dataBase.mapSize);
    }

	// This will be changed since we won't be satisfied with just a hexagon in the future
	// But the formula for locating tiles is useful!
	void generateHexMap (int size){
		board = new tile[size * 2-1, size * 2-1];
		for (int i=0; i<size * 2-1; i++) {
			for (int j=0; j<size * 2-1; j++) {
				board[i,j]=new tile();
			}
		}
		Transform boardHolder = new GameObject ("Board").transform;
		Vector3 pieceposition;
		for(int i=1;i<size*2;i++){
			for(int j=1;j<2*size-Mathf.Abs(size-i);j++){

				//crazy math formulas for the actuall positions, don't delve in if you value your life!
				pieceposition= new Vector3 (tileSize*(Mathf.Abs(size-i)*0.5f+j-size),tileSize*ratio*(i-size),0f);
				GameObject instance = Instantiate(gameTile,pieceposition,Quaternion.identity) as GameObject;
				inputTile tileInfo = instance.GetComponent<inputTile>();

				//the same craziness here
				tileInfo.tilePosition=new Vector2(j-0.5f*(size+i-Mathf.Abs(i-size)),i-size);
				instance.transform.SetParent (boardHolder);
			}
		}
	}
	
    public Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd){
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }

    /*
     * Returns whether the projectile is blocked by barriers
     */
    public bool isBlocked(Vector2 firePosition, Vector2 targetPosition){
        int count=0;
        Vector3 firePos = SS.hexPositionTransform(firePosition);
        Vector3 targetPos = SS.hexPositionTransform(targetPosition);
        Vector3 barrierPos;
        float dis;
        for (int i=0; i<barrierSpawnPoint.Length; i++) {
			barrierPos = SS.hexPositionTransform (barrierSpawnPoint [i]);
			if ((firePos.x <= barrierPos.x) && 
				(targetPos.x >= barrierPos.x) &&
				(firePos.y <= barrierPos.y) && 
				(targetPos.y >= barrierPos.y)) { //make sure the barrier is inside the parallelogram
				dis = DistancePointLine (barrierPos, firePos, targetPos);
				if (dis < tileSize / 2) {
					return true; //the projectile cut through one 
				} else if (dis == tileSize / 2) {   // Nice try! There's actually a (Mathematically speaking) simple solution:
					count++;                    // if dis==piecesize/2 (or almostEqual) nudge the target a little bit away from the current barrier
				}                               // And run this thing again!
			}
			if (count > 1) { //the projectile cuts through the tangent of multiple barriers
				//actually this idea is bugged in some barrier formations
				return true;
			}
		}
        return false;
    }

    /*
     * Returns whether the tile is occupied by barriers or turrets
     */
    public bool isOccupied(Vector2 pos){
        for (int i=0;i<turretSpawnPoint.Length;i++){
            if (pos.x == turretSpawnPoint[i].x && pos.y == turretSpawnPoint[i].y){
                return true;        // Oh but what if the turret is destroyed? >:-)
            }
        }
        for (int i=0;i<barrierSpawnPoint.Length;i++){
            if (pos.x == barrierSpawnPoint[i].x && pos.y == barrierSpawnPoint[i].y){
                return true;        // What if the shields are not deployed? >:-)
            }
        }
        return false;
    }

    /*
     * Check if pos is in range of any turrets(statsManager.turretRange)
     */
    public bool isDangerous(Vector2 pos){
        int range = dataBase.turretRange;
        float turretX;
        float turretY;
        for (int i=0;i<turretSpawnPoint.Length;i++){
            if (SS.getDistance(turretSpawnPoint[i],pos)<=range){    // Use the functions in SS!
                return true;
            }
        }
        return false;        
    }

	List<int> vecToBoard(Vector2 v){
		int x = Mathf.RoundToInt(v.x);
		int y = Mathf.RoundToInt(v.y);
		int newx, newy;
		newx = x + dataBase.mapSize - 1;
		newy = dataBase.mapSize - 1 - y;
		return new List<int>(){newx,newy};
	}

	public void bomb(Vector2 position,damageInfo damage){
		List<int> pos = vecToBoard (position);
		//print ("bomb!");
		//print (new Vector2(pos[0],pos[1]));
		board [pos[0], pos[1]].addDamage (damage);
	}

	/*
     * Copied from http://answers.unity3d.com/questions/62644/distance-between-a-ray-and-a-point.html
     * Calculates the distance between a point and a line
     */
	public float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd){
		return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
	}

	public Vector2[] turretGetter(){
		return turretSpawnPoint;
	}
	
	public Vector2[] barrierGetter(){
		return barrierSpawnPoint;
	}

	public tile getTile(Vector2 position){
		List<int> pos = vecToBoard (position);
		return board [pos[0], pos[1]];
	}
}
