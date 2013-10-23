﻿using UnityEngine;
using System.Collections;

public enum CharacterType {Zebra, Rhino, Tiger}

public class Player : MonoBehaviour {
	
	/*
	 * sid	- 	socket id
pid		-	id of the player
position (x,y)		- 	the x and y tiles in the map
avatarId	-	id of the avatar
isAlive			-	server will update the isAlive status and send back to the client
haste_powerup	-	the powerup status
invulnerable_powerup	-	the powerup status
disguise_powerup		- 	the powerup status
bomb_left		-	how many bomb left to plant
*/
	
	// Animal Characters
	[SerializeField] GameObject m_zebraCharacter;	
	[SerializeField] GameObject m_tigerCharacter;	
	[SerializeField] GameObject m_rhinoCharacter;	
	
	[SerializeField] int avatarId;			// 1 - Zebra, 2 - Rhino, 3 - Tiger
	[SerializeField] string playerName;		// Playername
	
	private GameObject character;
	
	// Powerup Status
	private bool isHasteActivated = false;
	private bool isInvulnerable = false;
	private bool isTrick = false;		
	private bool isSpeed = false;
	private bool isShake = false;
	
	// Speed and Avatar
	private int speed = 15;
	
	
	// Alive Status
	private bool isAlive = true;
	
	// If it is player himself or it is other player (opponent)
	[SerializeField] bool isSelf = false;
	
	public string PlayerName
	{
        get { return playerName; }
        set { playerName = value; }
    }
	
	public int AvatarId
	{
        get { return avatarId; }
        set { avatarId = value; }
    }
	
	public int Speed 
	{
        get { return speed; }
        set { speed = value; }
    }
	
	public bool IsMyself 
	{
        get { return isSelf; }
        set { isSelf = value; }
    }
	
	public bool HastePowerup 
	{
        get { return isHasteActivated; }
        set { isHasteActivated = value; }
    }
	
	public bool InvulnerablePowerup 
	{
        get { return isInvulnerable; }
        set { isInvulnerable = value; }
    }
	
	public bool TrickPowerup 
	{
        get { return isTrick; }
        set { isTrick = value; }
    }
	
	public bool SpeedPowerup 
	{
        get { return isSpeed; }
        set { isSpeed = value; }
    }
	
	public bool ShakePowerup 
	{
        get { return isShake; }
        set { isShake = value; }
    }
	
	public bool AliveStatus 
	{
        get { return isAlive; }
        set { isAlive = value; }
    }
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	public void UpdatePosition(float cellX, float cellY)
	{
		// Update position of other player but not yourself
		if(character)
		{
			gameObject.transform.position = character.gameObject.transform.position;
			//playerCharacter.transform.position = new Vector3(ZooMap.GetHorizontalPos(cellX),ZooMap.GetVerticalPos(cellY), 0);
			//Debug.Log ("Updating position to x: "+ZooMap.GetHorizontalPos(cellX)+" y: "+ZooMap.GetVerticalPos(cellY));
		}
	}
	
	void UpdateSpeed(int speedVal)
	{
		
	}
	
	public void InitZebraCharacter()
	{
		character = Instantiate(m_zebraCharacter, transform.position, transform.rotation) as GameObject;
		var animationScript = character.GetComponent<CharacterAnimController>();
		animationScript.EnemyPlayer = !isSelf;
		
		// If is player himself
		if(isSelf)
		{
			// Set the Zebra to tagged as Player
			character.gameObject.tag = "Player";
		}
		
		Debug.Log ("Zebra is created");
	}
	
	public void InitRhinoCharacter()
	{
		character = Instantiate(m_rhinoCharacter, transform.position, transform.rotation) as GameObject;
		var animationScript = character.GetComponent<CharacterAnimController>();
		animationScript.EnemyPlayer = !isSelf;
		
		// If is player himself
		if(isSelf)
		{
			// Set the rhino to tagged as Player
			character.gameObject.tag = "Player";
		}
		
		Debug.Log ("Rhino is created");
	}

	// Update is called once per frame
	void Update () 
	{
		GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
		// Update player position so the camera will follow
		if(playerCharacter)
		{
			gameObject.transform.localPosition = playerCharacter.transform.position;
		}
	}
}


