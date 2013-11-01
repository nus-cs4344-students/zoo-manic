using UnityEngine;
using System;
using System.Collections;

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
	
	public bool EnemyPlayer 
	{
        get { return isEnemy; }
        set { isEnemy = value; }
    }

	public int BombLimit 
	{
        get { return m_bombLimit; }
        set { m_bombLimit = value; }
    }
	
    // Use this for initialization
    void Start () {
        // This script must be attached to the sprite to work.
        anim = GetComponent<tk2dSpriteAnimator>();
		
		InitAnimation();
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
			// Can play bomb
			if(m_bombLimit > 0)
				plantBomb();
        }
		
		// Input.GetAxis("Vertical") > 0
		if (Input.GetKey(KeyCode.UpArrow) && !checkHitObstacle("up") ) { MoveUp(); return; }
		
		//else if ( Input.GetAxis("Horizontal") > 0
  		if (Input.GetKey(KeyCode.RightArrow) && !checkHitObstacle("right") ) { MoveRight(); return; }
		
		// else Input.GetAxis("Vertical") < 0 
  		if (Input.GetKey(KeyCode.DownArrow) && !checkHitObstacle("down") ) { MoveDown(); return; }
		
		//else if ( Input.GetAxis("Horizontal") < 0
  		if (Input.GetKey(KeyCode.LeftArrow) && !checkHitObstacle("left") ) { MoveLeft(); return; }
    }
	
		
	public void UpdatePosition(float positionX, float positionY)
	{
		gameObject.transform.position = new Vector3(positionX, positionY, 0);
	}
	
	void plantBomb()
	{
		Vector3 bombPos = transform.position + new Vector3(0, 0, 0.01f);
		GameObject bombInstance = Instantiate(bombGO, bombPos, transform.rotation) as GameObject;
		m_bombLimit--;
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
	
	public void MoveUp()
	{
		if(previousDirection != DirectionType.Front)
		{
			UpdateRayCasting(DirectionType.Front);
			previousDirection = DirectionType.Front;
		}
		
		playAnimation(front_anim);
		//transform.Translate( 0, speed * Time.deltaTime, 0);
		//clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	
		Vector3 startPoint = transform.position;
		//Vector3 endPoint = transform.position - new Vector3(ZooMap.cellWidth, 0, 0);
		
		float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		
		// exceed the map
		if(verticalCell >= ZooMap.NumberofCols || isStillMoving)
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
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Front));
	}
	
	public void MoveRight()
	{
		if(previousDirection != DirectionType.Right)
		{
			UpdateRayCasting(DirectionType.Right);
			previousDirection = DirectionType.Right;
		}
		
		//playAnimation(right_anim);
		//transform.Translate( speed * Time.deltaTime, 0, 0);
		//clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	
		Vector3 startPoint = transform.position;
		//Vector3 endPoint = transform.position + new Vector3(ZooMap.cellWidth, 0, 0);
		
		float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		
		// exceed the map
		if( horizontalCell >= ZooMap.NumberofRows || isStillMoving)
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
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Right));
	}
	
	public void MoveDown()
	{
		if(previousDirection != DirectionType.Down)
		{
			UpdateRayCasting(DirectionType.Down);
			previousDirection = DirectionType.Down;
		}
		
		//playAnimation(back_anim);
		//transform.Translate( 0, -speed * Time.deltaTime, 0);
		
		Vector3 startPoint = transform.position;
		//Vector3 endPoint = transform.position - new Vector3(ZooMap.cellWidth, 0, 0);
		
		float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);

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
		
		//transform.Translate( 0, transform.position.y - ZooMap.cellHeight * Time.deltaTime, 0);
		
		// The step size is equal to speed times frame time.
		var step = speed * Time.deltaTime;
		
		// Move our position a step closer to the target.
		//transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
		
		//transform.position = Vector3.Lerp(startPoint, endPoint, (speed * Time.deltaTime));
		
		//clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	
		float time = ZooMap.cellHeight / speed;		// time = distance over speed
		
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Down));
	}
	
	public void MoveLeft()
	{
		if(previousDirection != DirectionType.Left)
		{
			UpdateRayCasting(DirectionType.Left);
			previousDirection = DirectionType.Left;
		}
		
		//playAnimation(left_anim);
		//transform.Translate( -speed * Time.deltaTime, 0, 0);
		//clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );

		Vector3 startPoint = transform.position;
		//Vector3 endPoint = transform.position - new Vector3(ZooMap.cellWidth, 0, 0);
		
		float verticalCell = ZooMap.GetVerticalCell(transform.position.y);
		float horizontalCell = ZooMap.GetHorizontalCell(transform.position.x);
		
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
		StartCoroutine(MoveObject(transform, startPoint, endPoint, time, DirectionType.Left));

		//transform.position = Vector3.Lerp(startPoint, endPoint, (speed * Time.deltaTime));
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
		Debug.Log ("Cell X: "+horizontalCell);
		Debug.Log ("Cell Y: "+verticalCell);
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