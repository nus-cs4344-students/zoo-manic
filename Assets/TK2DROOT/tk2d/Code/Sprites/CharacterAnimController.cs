using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

enum DirectionType { Left, Right, Front, Down };

// Since our game is top-down orthographic, the Z-axis is actually the Y-axis
public class CharacterAnimController : MonoBehaviour {

    // Link to the animated sprite
    private tk2dSpriteAnimator anim;

    // State variable to see if the character is walking.
    private bool walking = false;
	
	private float speed = 15.0f;
	
	[SerializeField] GameObject bombGO;
	[SerializeField] int m_bombLimit; 
	[SerializeField] CharacterType characterType; 
	
	[SerializeField] ClientSocket clientSocketScript;
	[SerializeField] Player playerScript; 
	
	private string left_anim;
	private string right_anim;
	private string back_anim;
	private string front_anim;
	
	private bool isEnemy = false;
	
	private DirectionType previousDirection = DirectionType.Left;
	
	private bool isStillMoving = false;
	
	// Datastructure that stores a list of bombs object
	private Dictionary<string, GameObject> bombDict = new Dictionary<string, GameObject>();
	
	private GameObject hudInstance;
	private HUD hudScript;
	private ZooMap zooMapScript;
	
	public bool EnemyPlayer 
	{
        get { return isEnemy; }
        set { isEnemy = value; }
    }
	
	public CharacterType CharacterIcon 
	{
        get { return characterType; }
    }
	
	public float PlayerSpeed 
	{
        get { return speed; }
        set { speed = value; }
    }

	public int BombLimit 
	{
        get { return m_bombLimit; }
        set { m_bombLimit = value; }
    }
	
    // Use this for initialization
    void Start () {
		
		hudInstance = GameObject.Find ("UnityHUDPrefab");
		if(hudInstance)
		{
			hudScript = hudInstance.GetComponent<HUD>();
		}
		
        // This script must be attached to the sprite to work.
        anim = GetComponent<tk2dSpriteAnimator>();
		
		InitAnimation();
		
		clientSocketScript = GameObject.Find("PlayerConnection").GetComponent<ClientSocket>();
		zooMapScript = GameObject.Find ("ZooMap").GetComponent<ZooMap>();
    }
	
	void InitAnimation()
	{
		switch (characterType)
		{
			case CharacterType.Zebra:
				left_anim = "zebra_move_left";
				front_anim = "zebra_move_front";
				back_anim = "zebra_move_back";
				right_anim = "zebra_move_right";
			break;
			
			case CharacterType.Rhino:
				left_anim = "rhino_move_left";
				front_anim = "rhino_move_front";
				back_anim = "rhino_move_back";
				right_anim = "rhino_move_right";
			break;
			
			case CharacterType.Tiger:
				left_anim = "tiger_move_left";
				front_anim = "tiger_move_front";
				back_anim = "tiger_move_back";
				right_anim = "tiger_move_right";
			break;
			
			case CharacterType.Bird:
				left_anim = "bird_move_left";
				front_anim = "bird_move_front";
				back_anim = "bird_move_back";
				right_anim = "bird_move_right";
			break;
		}
	}

    // This is called once the hit animation has compelted playing
    // It returns to playing whatever animation was active before hit
    // was playing.
    void HitCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) {
        if (walking) {
            anim.Play("walk");
        } 
        else {
            anim.Play("idle");
        }
    }

    // Update is called once per frame
    void Update () {
		
		// if other player, we don't care the controls
		if( isEnemy )
		{
			return;
		}
		
        if (Input.GetKeyDown(KeyCode.Space)) 
		{	
			if(hudScript && !hudScript.IsChatEnabled)
			{
				// Can play bomb
				if(m_bombLimit > 0)
					SendPlantBombMessage();
			}
        }

		// Input.GetAxis("Vertical") > 0
		if (Input.GetKey(KeyCode.UpArrow) && !checkHitObstacle("up") ) { MoveUp(0.0f, 0.0f, true); return; }
		
		//else if ( Input.GetAxis("Horizontal") > 0
  		if (Input.GetKey(KeyCode.RightArrow) && !checkHitObstacle("right") ) { MoveRight(0.0f, 0.0f, true); return; }
		
		// else Input.GetAxis("Vertical") < 0 
  		if (Input.GetKey(KeyCode.DownArrow) && !checkHitObstacle("down") ) { MoveDown(0.0f, 0.0f, true); return; }
		
		//else if ( Input.GetAxis("Horizontal") < 0
  		if (Input.GetKey(KeyCode.LeftArrow) && !checkHitObstacle("left") ) { MoveLeft(0.0f, 0.0f, true); return; }
    }
	
		
	public void UpdatePosition(float positionX, float positionY)
	{
		gameObject.transform.position = new Vector3(positionX, positionY, 0);
	}
	
	public void RespawnPlayer(float cellX, float cellY)
	{
		float horizontalPos = ZooMap.GetHorizontalPos(cellX);
		float verticalPos = ZooMap.GetVerticalPos(cellY);
		gameObject.transform.position = new Vector3(horizontalPos, verticalPos, gameObject.transform.position.z);
	}
	
	public void PlantBomb(float horizontalCell, float verticalCell)
	{
		// If player is still moving, don't plant the bomb
		//if(isStillMoving)
		//{
		//	return;
		//}
			
		//float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		//float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		
		float verticalPos = ZooMap.GetVerticalPos(verticalCell);
		float horizontalPos = ZooMap.GetHorizontalPos(horizontalCell);
		
		//Vector3 bombPos = transform.position + new Vector3(0, 0, 0.01f);
		Vector3 bombPos = new Vector3(horizontalPos, verticalPos, 0.01f);
		GameObject bombInstance = Instantiate(bombGO, bombPos, transform.rotation) as GameObject;
		string bombId = (int) horizontalCell + "" + (int) verticalCell;
		
		string cellID = (int) horizontalCell + ":" + (int) verticalCell;
		
		if(bombDict.ContainsKey(bombId) == false)
		{
			bombDict.Add(bombId, bombInstance);

			// bomb is planted in the cell
			zooMapScript.UpdateCellWithBomb(cellID, true);
			
			m_bombLimit--;
		}
	}
	
	public void UpdateBombLeft()
	{
		GameObject hud = GameObject.Find ("UnityHUDPrefab");
		if(hud)
		{
			HUD hudScript = hud.GetComponent<HUD>();
			bool isRangePowerupActivated = hudScript.IsRangeActivated();
			
			// make sure does not exceed 3
			if(isRangePowerupActivated == false)
			{
				if(m_bombLimit < 3)
					m_bombLimit++;
			}
			else
			{
				if(m_bombLimit < 6)
					m_bombLimit++;
			}
			
			hudScript.SetBombLeft(m_bombLimit);
		}
	}
	
	public void ExplodeBomb(float horizontalCell, float verticalCell, long explodeRange)
	{
		string bombId = (int) horizontalCell + "" + (int) verticalCell;
		string cellID = (int) horizontalCell + ":" + (int) verticalCell;
		
		if(bombDict.ContainsKey(bombId))
		{
			GameObject bombObject = (GameObject) bombDict[bombId];
			Bomb bombScript = bombObject.GetComponent<Bomb>();
			
			if(bombScript != null)
			{
				zooMapScript.UpdateCellWithBomb(cellID, false);
				bombScript.Explode((int) explodeRange);
				bombDict.Remove(bombId);
			}
		}
		
	}
	
	public void SendPlantBombMessage()
	{
		// If player is still moving, don't sends update to the server
		//if(isStillMoving)
		//{
		//	return;
		//}
		
		float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		string bombId = (int) horizontalCell + "" + (int) verticalCell;
		
		if(bombDict.ContainsKey(bombId) == false)
		{
			clientSocketScript.SendPlantBombMessage(horizontalCell, verticalCell);
		}
	}
	
	void playAnimation(string animation)
	{	
		anim.Play(animation);
	}
	
	// Raycast to check whether hit any obstacle
	// Return true if hits, otherwise returns false
	bool checkHitObstacle(string direction)
	{
		DirectionRaycasting collisionDetection = gameObject.GetComponent<DirectionRaycasting>();
		bool isCollide = false;
		
		switch (direction)
		{
			case "left":
				isCollide = collisionDetection.collisionLeft;
				break;
			case "right":
				isCollide = collisionDetection.collisionRight;
				break;
			case "up":
				isCollide = collisionDetection.collisionUp;
				break;
			case "down":
				isCollide = collisionDetection.collisionDown;
				break;
		}
		
		return isCollide;
	}
	
	public void SpawnCharacter(float spawnX, float spawnY)
	{
		gameObject.transform.localPosition = new Vector3(spawnX, spawnY, 0);
	}
	
	public void MoveUp(float originalCellX, float originalCellY, bool sendToServer)
	{
		if(previousDirection != DirectionType.Front)
		{
			UpdateRayCasting(DirectionType.Front);
			previousDirection = DirectionType.Front;
		}

		Vector3 startPoint = transform.position;
		
		float verticalCell = 0; 
		float horizontalCell = 0; 
		
		if(sendToServer)
		{
			verticalCell = ZooMap.GetVerticalCell(transform.position.y);
			horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		}
		else
		{
			horizontalCell = originalCellX;
			verticalCell = originalCellY;
		}
		
		// exceed the map
		if(verticalCell >= ZooMap.NumberofCols - 1 || isStillMoving)
			return;
		
		bool isObstacleAhead = ZooMap.IsObstacle((int) horizontalCell, (int) verticalCell + 1);
		
		if(isObstacleAhead)
		{
			Debug.Log ("Obstacle!!! is in front");
			return;
		}
		
		float nextPosY = ZooMap.GetVerticalPos(verticalCell + 1);
		
		Vector3 endPoint = new Vector3(transform.position.x, nextPosY, transform.position.z);
		
		float time = ZooMap.cellHeight / speed;		// time = distance over speed
		
		// client moves
		if(sendToServer)
		{
			clientSocketScript.SendMovementMessage(horizontalCell, verticalCell, "UP", speed);
			SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
			
			if(soundManager != null)
				soundManager.PlayMoveSound(transform.position);		
		}
		
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Front));
	}
	
	public void MoveRight(float originalCellX, float originalCellY, bool sendToServer)
	{
		if(previousDirection != DirectionType.Right)
		{
			UpdateRayCasting(DirectionType.Right);
			previousDirection = DirectionType.Right;
		}
		
		Vector3 startPoint = transform.position;
		
		float verticalCell = 0; 
		float horizontalCell = 0; 
		
		if(sendToServer)
		{
			verticalCell = ZooMap.GetVerticalCell(transform.position.y);
			horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		}
		else
		{
			horizontalCell = originalCellX;
			verticalCell = originalCellY;
		}
		
		// exceed the map
		if( horizontalCell >= ZooMap.NumberofRows - 1 || isStillMoving)
			return;
		
		bool isObstacleAhead = ZooMap.IsObstacle((int) horizontalCell + 1, (int) verticalCell);
		
		if(isObstacleAhead)
		{
			Debug.Log ("Obstacle!!! is on right");
			return;
		}
		
		float nextPosX = ZooMap.GetHorizontalPos(horizontalCell + 1);
		
		Vector3 endPoint = new Vector3(nextPosX, transform.position.y, transform.position.z);

		//transform.position = Vector3.Lerp(startPoint, endPoint, (speed * Time.deltaTime));
		
		float time = ZooMap.cellWidth / speed;		// time = distance over speed
		
		// client moves
		if(sendToServer)
		{
			clientSocketScript.SendMovementMessage(horizontalCell, verticalCell, "RIGHT", speed);
			SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
			
			if(soundManager != null)
				soundManager.PlayMoveSound(transform.position);				
		}
		
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Right));
	}
	
	public void MoveDown(float originalCellX, float originalCellY, bool sendToServer)
	{
		if(previousDirection != DirectionType.Down)
		{
			UpdateRayCasting(DirectionType.Down);
			previousDirection = DirectionType.Down;
		}

		Vector3 startPoint = transform.position;

		float verticalCell = 0; 
		float horizontalCell = 0; 
		
		if(sendToServer)
		{
			verticalCell = ZooMap.GetVerticalCell(transform.position.y);
			horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		}
		else
		{
			horizontalCell = originalCellX;
			verticalCell = originalCellY;
		}
		
		// exceed the map
		if(verticalCell <= 0 || isStillMoving)
			return;
		
		bool isObstacleAhead = ZooMap.IsObstacle((int) horizontalCell, (int) verticalCell - 1);
		
		if(isObstacleAhead)
		{
			Debug.Log ("Obstacle!!! is on down");
			return;
		}
		
		float nextPosY = ZooMap.GetVerticalPos(verticalCell - 1);
		
		Vector3 endPoint = new Vector3(transform.position.x, nextPosY, transform.position.z);

		// The step size is equal to speed times frame time.
		//var step = speed * Time.deltaTime;

		float time = ZooMap.cellHeight / speed;		// time = distance over speed
		
		// client moves
		if(sendToServer)
		{
			clientSocketScript.SendMovementMessage(horizontalCell, verticalCell, "DOWN", speed);
			SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
			
			if(soundManager != null)
				soundManager.PlayMoveSound(transform.position);				
		}
		
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Down));
	}
	
	public void MoveLeft(float originalCellX, float originalCellY, bool sendToServer)
	{
		if(previousDirection != DirectionType.Left)
		{
			UpdateRayCasting(DirectionType.Left);
			previousDirection = DirectionType.Left;
		}

		Vector3 startPoint = transform.position;

		float verticalCell = 0; 
		float horizontalCell = 0; 
		
		if(sendToServer)
		{
			verticalCell = ZooMap.GetVerticalCell(transform.position.y);
			horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		}
		else
		{
			horizontalCell = originalCellX;
			verticalCell = originalCellY;
		}

		// exceed the map or if the character still moving
		if(horizontalCell <= 0 || isStillMoving)
			return;
		
		bool isObstacleAhead = ZooMap.IsObstacle((int) horizontalCell - 1, (int) verticalCell);
		
		if(isObstacleAhead)
		{
			Debug.Log ("Obstacle!!! is on left");
			return;
		}
		
		float nextPosX = ZooMap.GetHorizontalPos(horizontalCell - 1);
		
		Vector3 endPoint = new Vector3(nextPosX, transform.position.y, transform.position.z);
		
		float time = ZooMap.cellWidth / speed;		// time = distance over speed
		
		// client moves
		if(sendToServer)
		{
			clientSocketScript.SendMovementMessage(horizontalCell, verticalCell, "LEFT", speed);
			SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
			
			if(soundManager != null)
				soundManager.PlayMoveSound(transform.position);		
		}
		
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Left));
	}
	
	
	// Move object within a set of time. The time here is distance over speed
	IEnumerator MoveObject (Transform thisTransform, Vector3 startPos, Vector3 endPos, float time, DirectionType direction) {
	    float i = 0.0f;
	    float rate = 1.0f / time;
		
	    while (i < 1.0f) {
			isStillMoving = true;
	        i += Time.deltaTime * rate;
	        thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			
			if(direction == DirectionType.Left)
				playAnimation(left_anim);
			else if(direction == DirectionType.Right)
				playAnimation(right_anim);
			else if(direction == DirectionType.Front)
				playAnimation(front_anim);
			else if(direction == DirectionType.Down)
				playAnimation(back_anim);
			
	        yield return null; 
	    }
		
		isStillMoving = false;
		
		float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		//Debug.Log ("Cell X: "+horizontalCell);
		//Debug.Log ("Cell Y: "+verticalCell);
	}

	void UpdateRayCasting(DirectionType direction)
	{
		gameObject.transform.Find("RayCastingFront").gameObject.SetActive(false);
		gameObject.transform.Find("RayCastingBack").gameObject.SetActive(false);
		gameObject.transform.Find("RayCastingLeft").gameObject.SetActive(false);
		gameObject.transform.Find("RayCastingRight").gameObject.SetActive(false);
		
		if(direction == DirectionType.Front)
		{
			gameObject.GetComponent<DirectionRaycasting>().currentRay = "RayCastingFront";
			gameObject.transform.Find("RayCastingFront").gameObject.SetActive(true);
		}
		else if(direction == DirectionType.Down)
		{
			gameObject.GetComponent<DirectionRaycasting>().currentRay = "RayCastingBack";
			gameObject.transform.Find("RayCastingBack").gameObject.SetActive(true);
		}
		else if(direction == DirectionType.Left)
		{
			gameObject.GetComponent<DirectionRaycasting>().currentRay = "RayCastingLeft";
			gameObject.transform.Find("RayCastingLeft").gameObject.SetActive(true);
		}
		else if(direction == DirectionType.Right)
		{
			gameObject.GetComponent<DirectionRaycasting>().currentRay = "RayCastingRight";
			gameObject.transform.Find("RayCastingRight").gameObject.SetActive(true);
		}
	}
	
	// This method will be invoked by the "ExplosionDamage.cs"
	public void KillPlayer ()
    {
		Destroy(gameObject);
    } 
	
}