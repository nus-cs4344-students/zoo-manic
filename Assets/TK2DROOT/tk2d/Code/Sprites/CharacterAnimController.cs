using UnityEngine;
using System;
using System.Collections;

public enum CharacterType {Zebra, Rhino, Tiger}

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
	
	private string left_anim;
	private string right_anim;
	private string back_anim;
	private string front_anim;

	public int BombLimit {
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
		
        if (Input.GetKeyDown(KeyCode.Space)) 
		{
            // Only play the clip if it is not already playing.
            // Calling play will restart the clip if it is already playing.
            /*if (!anim.IsPlaying("hit")) {
                anim.Play("hit");

                // The delegate is used here to return to the previously
                // playing clip after the "hit" animation is done playing.
                anim.AnimationCompleted = HitCompleteDelegate;
            }*/
			
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

        /*if (Input.GetKey(KeyCode.RightArrow)) {
            if (!anim.IsPlaying("walk")) {

                // Walk is a looping animation
                // A looping animation never completes...
                anim.Play("walk");

                // We dont have any reason for detecting when it completes
                anim.AnimationCompleted = null;
                walking = true;
            }
        }

        if (Input.GetKey(KeyCode.W)) {
            if (!anim.IsPlaying("idle")) {
                anim.Play("idle");
                // We dont have any reason for detecting when it completes
                anim.AnimationCompleted = null;
                walking = false;
            }
        }*/
    }
	
	void plantBomb()
	{
		Vector3 bombPos = transform.position + new Vector3(0, 0, 1);
		GameObject bombInstance = Instantiate(bombGO, bombPos, transform.rotation) as GameObject;
		m_bombLimit--;
	}
	
	void playAnimation(string animation)
	{
		/*if (!anim.IsPlaying("walk")) {

            // Walk is a looping animation
            // A looping animation never completes...
            anim.Play("walk");

            // We dont have any reason for detecting when it completes
            anim.AnimationCompleted = null;
            walking = true;
        }*/
		
        /*if (!anim.IsPlaying("idle")) {
            anim.Play("idle");
            // We dont have any reason for detecting when it completes
            anim.AnimationCompleted = null;
            walking = false;
        }*/
		
		anim.Play(animation);
	}
	
	// Raycast to check whether hit any obstacle
	// Return true if hits, otherwise returns false
	bool checkHitObstacle(string direction)
	{
		/*var fwd = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		
		if (Physics.Raycast (transform.position, fwd, 10)) {
			if(hit.collider.tag == "Obstacle")
				Debug.Log("There is something in front of the object!");
			else
				Debug.Log("Not an Obstacle!!");
		}*/
		
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
	}
	
	void moveRight()
	{
		playAnimation(right_anim);
		transform.Translate( speed * Time.deltaTime, 0, 0);
	}
	
	void moveDown()
	{
		playAnimation(back_anim);
		transform.Translate( 0, -speed * Time.deltaTime, 0);
	}
	
	void moveLeft()
	{
		playAnimation(left_anim);
		transform.Translate( -speed * Time.deltaTime, 0, 0);
	}
	
	// This method will be invoked by the "ExplosionDamage.cs"
	public void KillPlayer ()
    {
		Destroy(gameObject);
    } 
	
}