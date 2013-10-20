using UnityEngine;
using System.Collections;

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
	// Powerup Status
	private bool isHasteActivated = false;
	private bool isInvulnerable = false;
	private bool isTrick = false;		
	private bool isSpeed = false;
	private bool isShake = false;
	
	// Alive Status
	private bool isAlive = true;
	
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
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


