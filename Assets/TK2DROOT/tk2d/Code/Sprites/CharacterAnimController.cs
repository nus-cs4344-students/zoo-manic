using UnityEngine;
using System;
using System.Collections;

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
		if (Input.GetKey(KeyCode.UpArrow) && !checkHitObstacle("up") ) { moveUP(); return; }
		
		//else if ( Input.GetAxis("Horizontal") > 0
  		if (Input.GetKey(KeyCode.RightArrow) && !checkHitObstacle("right") ) { moveRight(); return; }
		
		// else Input.GetAxis("Vertical") < 0 
  		if (Input.GetKey(KeyCode.DownArrow) && !checkHitObstacle("down") ) { moveDown(); return; }
		
		//else if ( Input.GetAxis("Horizontal") < 0
  		if (Input.GetKey(KeyCode.LeftArrow) && !checkHitObstacle("left") ) { moveLeft(); return; }
    }
	
		
	public void UpdatePosition(float positionX, float positionY)
	{
		gameObject.transform.position = new Vector3(positionX, positionY, 0);
	}
	
	void plantBomb()
	{
		Vector3 bombPos = transform.position + new Vector3(0, 0, 1);
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
	
	void moveUP()
	{
		playAnimation(front_anim);
		transform.Translate( 0, speed * Time.deltaTime, 0);
		clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	}
	
	void moveRight()
	{
		playAnimation(right_anim);
		transform.Translate( speed * Time.deltaTime, 0, 0);
		clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	}
	
	void moveDown()
	{
		playAnimation(back_anim);
		transform.Translate( 0, -speed * Time.deltaTime, 0);
		clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	}
	
	void moveLeft()
	{
		playAnimation(left_anim);
		transform.Translate( -speed * Time.deltaTime, 0, 0);
		clientSocketScript.SendMovementMessage( ZooMap.GetHorizontalCell(transform.position.x) , ZooMap.GetVerticalCell(transform.position.y) );
	}
	
	// This method will be invoked by the "ExplosionDamage.cs"
	public void KillPlayer ()
    {
		Destroy(gameObject);
    } 
	
}