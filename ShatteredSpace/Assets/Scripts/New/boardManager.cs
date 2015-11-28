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
    const float epsilon = 0.01f; // Good enough for our purposes
	float tileSize;
    int tn;
    int time;


    [SerializeField] statsManager dataBase;
    [SerializeField] functionManager SS;    // Just call it SS for sake of laziness
    [SerializeField] turnManager  turn;
    [SerializeField] Vector2[] turretSpawnPoint = new Vector2[5];
    [SerializeField] int[] turretSpawnTimers = new int[5];
    [SerializeField] Vector2[] barrierSpawnPoint = new Vector2[12]; // Barrier numbers may vary

	[SerializeField] GameObject gameTile;
	[SerializeField] GameObject turretGameObj;
	[SerializeField] GameObject blastShieldGameObj;
    [SerializeField] GameObject energy;
	[SerializeField] List<GameObject> explosions=new List<GameObject>();

    tile[,] board;

    List<player> players = new List<player>();
	List<turret> turrets = new List<turret>();

    void Start(){
        tileSize = dataBase.tileSize;
		generateHexMap(dataBase.mapSize);
		generateMapObjects();
    }

	public void addExplosion(GameObject expl, Vector3 position)
	{
		GameObject explosionInstance=Instantiate(expl, position, Quaternion.identity) as GameObject;
		explosions.Add (explosionInstance);
	}

    void Update()
    {
		for(int i = 0; i < explosions.Count; i++)
		{
			if(explosions[i].GetComponent<ParticleSystem>().isStopped && !explosions[i].GetComponent<ParticleSystem>().IsAlive())
			{
				Destroy(explosions[i]);
				explosions.RemoveAt(i);
			}
		}
        if (turn.getTime() == time) return;
		// Time/step based events
		time = turn.getTime();
        for (int i = 0; i < turretSpawnPoint.Length-1; i++)
		{
            List<int> temp = vecToBoard(turretSpawnPoint[i]);
            tile myTile = board[temp[0], temp[1]];
			if (myTile.hasEnergy() && occupiedByPlayer(turretSpawnPoint[i])) {
				getPlayer(turretSpawnPoint[i]).takeDamage(-1 * dataBase.turretStartEnergy);
				myTile.setEnergy(false);
				GameObject.Destroy(myTile.getEnergy());
			}
		}

		// Turn-based events
		if (turn.getTurn() == tn) return;
		for (int i = 0; i < turretSpawnPoint.Length-1; i++)
		{
			// Spawning a turret while a player's on the spot would cause infinite recursion
			// And that crashes the game, as you might have expected
			if (turretSpawnTimers[i] >= dataBase.turretRespawnTime && !occupiedByPlayer(turretSpawnPoint[i]))
			{
				List<int> temp = vecToBoard(turretSpawnPoint[i]);
				tile myTile = board[temp[0], temp[1]];
				if (myTile.hasEnergy()) {
					myTile.setEnergy(false);
					GameObject.Destroy(myTile.getEnergy());
				}
				spawnTurret(turretSpawnPoint[i]);
				turretSpawnTimers[i] = 0;
			}
			else if (turretSpawnTimers[i] >= 1)
			{
				turretSpawnTimers[i]++;
			}
		}
		// This makes sure that the event only happens once per turn
		tn = turn.getTurn();
	}
	
	// Always use this function when dealing with in-game vectors!
	List<int> vecToBoard(Vector2 v){
		int x = Mathf.RoundToInt(v.x);
		int y = Mathf.RoundToInt(v.y);
		int newx, newy;
		newx = x + dataBase.mapSize - 1;
		newy = dataBase.mapSize - 1 - y;
		return new List<int>(){newx,newy};
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

    void spawnTurret(Vector2 pos)
    {
        Vector3 spawnPosition = SS.hexPositionTransform(pos);
        List<int> temp = vecToBoard(pos);
        int x = temp[0], y = temp[1];
        board[x, y].activateTurret(true);
        GameObject instance = Instantiate(turretGameObj, spawnPosition, Quaternion.LookRotation(Vector3.up)) as GameObject;
        turret currTurret = instance.GetComponent<turret>();
        board[x, y].setTurret(currTurret);
        turrets.Add(currTurret);
        currTurret.setPos(pos);
    }

	void generateMapObjects(){
		// Spawn turrets
		foreach (Vector2 pos in turretSpawnPoint) {
            spawnTurret(pos);
        }
		// Spawn balst shields
		foreach (Vector2 pos in barrierSpawnPoint) {
			Vector3 spawnPosition = SS.hexPositionTransform(pos);
			List<int> temp = vecToBoard(pos);
			int x=temp[0],y=temp[1];
			board[x,y].activateWall(true);
			GameObject instance = Instantiate(blastShieldGameObj,spawnPosition,Quaternion.identity) as GameObject;
		}
	}

	public void destroyTurret(Vector2 pos){
		List<int> temp = vecToBoard (pos);
        tile myTile = board[temp[0], temp[1]];
        myTile.activateTurret (false);
        myTile.setEnergy(true);
        Vector3 spawnPosition = SS.hexPositionTransform(pos);
		GameObject instance = Instantiate (energy, spawnPosition, Quaternion.LookRotation (Vector3.up)) as GameObject;
		myTile.setEnergy (true);		// wow
		myTile.setEnergy (instance);	// so rigor
        int spawnIndex=0;
        while (turretSpawnPoint[spawnIndex] != pos) spawnIndex++;
        turretSpawnTimers[spawnIndex] = 1;
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
        Vector3 firePos = SS.hexPositionTransform(firePosition);
        Vector3 targetPos = SS.hexPositionTransform(targetPosition);
        Vector3 barrierPos;
        float dis;
		bool possibleBlock = false;
        for (int i=0; i<barrierSpawnPoint.Length; i++) {
			barrierPos = SS.hexPositionTransform (barrierSpawnPoint [i]);
			if (isInBound(firePos.x,firePos.y,
                targetPos.x,targetPos.y,
                barrierPos.x,barrierPos.y)) { //make sure the barrier is inside the parallelogram
				dis = DistancePointLine (barrierPos, firePos, targetPos);
				if (dis < tileSize / 2 - epsilon) {
					return true; //the projectile cut through one 
				} else if (almostEqual(dis,tileSize / 2)) { // We need to be careful when the line is tangent to the circle
					possibleBlock = true;
				}                              
			}
		}
		if (possibleBlock) {
			Vector2 tempTgt1,tempTgt2;
			tempTgt1=targetPosition+Vector2.left * 0.5f;	// Nudge the target a little bit to the left
			tempTgt2=targetPosition+Vector2.right * 0.5f;	// Nudge the target a little bit to the right
			// If both ways are blocked, it's blocked; If not, it's not blocked
			return isBlocked(firePosition,tempTgt1) && isBlocked(firePosition,tempTgt2);
		}
		return false;
    }

    bool almostEqual(float a, float b){
        return Mathf.Abs(a-b)<=epsilon;
    }

    bool isInBound(float x1, float y1, float x2, float y2, float x,float y){
        float temp;
        if (x1>x2){temp = x1; x1 = x2; x2 = temp;}
        if (y1>y2){temp = y1; y1 = y2; y2 = temp;}
        return (x1 < x + epsilon) && 
                (x2 > x - epsilon) && 
                (y1 < y + epsilon) && 
                (y2 > y - epsilon);
    }
    /*
     * Returns whether the tile is occupied by barriers or turrets
     */
    public bool isOccupied(Vector2 pos){
		// This is modified using the tile class
		int x, y;
		List<int> temp = vecToBoard (pos);
		x = temp [0];y = temp [1];
		if (board [x,y].wallIsActivated()||board [x,y].turretIsActivated()) {
			return true;
		}else{
			return false;
		}
    }

    /*
     * Check if pos is in range of any turrets(statsManager.turretRange)
     */
    public bool isDangerous(Vector2 pos){
        int range = dataBase.turretRange;
        float turretX;
        float turretY;
		List<int> turretPos;

        for (int i = 0;i < turretSpawnPoint.Length;i++){
			turretPos = vecToBoard(turretSpawnPoint[i]);
			if (SS.getDistance(turretSpawnPoint[i],pos) <= range 
			    && board [turretPos [0], turretPos [1]].turretIsActivated ()){    // Use the functions in SS!
                return true;
            }
        }
        return false;        
    }

	public List<turret> getAttackingTurrets(Vector2 pos){
		List<turret> attackers = new List<turret>();
		int range = dataBase.turretRange;
		float turretX;
		float turretY;
		for (int i=0;i<turretSpawnPoint.Length;i++){
			if (SS.getDistance(turretSpawnPoint[i],pos)<=range){  
				if(getTile (turretSpawnPoint[i]).turretIsActivated()){
					attackers.Add (getTile (turretSpawnPoint[i]).getTurret ());
				}
			}
		}
		return attackers;        
	}

	public bool bomb(Vector2 position,damageInfo damage){
		bool hit = false;
		List<int> pos = vecToBoard (position);
		//print ("bomb!");
		//print ("Bomb:" + (new Vector2(pos[0],pos[1])).ToString());

        // If damage is applied to a turret
		if (board [pos [0], pos [1]].turretIsActivated ()) {
			damage.applyToTurret(board [pos [0], pos [1]].getTurret());
			hit = true;
		}
        // If damage is applied to a player
		if (occupiedByPlayer(position)) {
			damage.applyToPlayer(getPlayer(position));
			hit = true;
		}
		board [pos[0], pos[1]].addDamage (damage);
		return hit;
	}

	// This wipes all the damage on the board for the next step
	public void cleanBoard(){
		foreach (tile t in board) {
			t.clearDamage();
		}
	}

	bool occupiedByPlayer(Vector2 pos){
		if (players.Count > 0) {
			foreach (player p in players) {
				if (p.getPosition () == pos) return true;
			}
		}
		return false;
	}

	player getPlayer(Vector2 pos){
		if (players.Count > 0) {
			foreach (player p in players) {
				if (p.getPosition () == pos) return p;
			}
		}
		return null;
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

    public void setPlayers(List<player> playerList){
        players=playerList;
    }

	public tile getTile(Vector2 position){
		List<int> pos = vecToBoard (position);
		return board [pos[0], pos[1]];
	}

	public void doTurretDamage(Vector2 v){
		if (isDangerous (v)) {
			List<turret> turretList = this.getAttackingTurrets (v);
			foreach (turret t in turretList) {
				print ("One turret attacks!");
				this.bomb (v,t.getDamage());
			}
		}
	}
}
