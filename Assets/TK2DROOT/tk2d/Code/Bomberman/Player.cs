using UnityEngine;
using System.Collections;

public enum CharacterType {Zebra, Rhino, Tiger, Bird}

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
	[SerializeField] GameObject m_birdCharacter;	
	
	[SerializeField] int avatarId;			// 0 - Zebra, 1 - Rhino, 2 - Tiger, 3 - Bird
	[SerializeField] string playerName;		// Playername
	
	private GameObject characterInstance;
	
	// Powerup Status
	private bool isHasteActivated = false;
	private bool isInvulnerable = false;
	private bool isTrick = false;		
	private bool isSpeed = false;
	private bool isShake = false;
	
	// Speed and Avatar
	private int speed = 15;
	
	private long playerID;
	
	
	// Alive Status
	private bool isAlive = true;
	
	// If it is player himself or it is other player (opponent)
	[SerializeField] bool isSelf = false;
	
	public Player(long playerID, int avatarID)
	{
		this.avatarId = avatarID;
		this.playerID = playerID;
	}
	
	public string PlayerName
	{
        get { return playerName; }
        set { playerName = value; }
    }
	
	public long PlayerID
	{
        get { return playerID; }
        set { playerID = value; }
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
	
	void Update()
	{
		GameObject playerCharacter = GameObject.FindGameObjectWithTag("Player");
		// Update player position so the camera will follow
		if(playerCharacter)
		{
			gameObject.transform.localPosition = playerCharacter.transform.position;
		}
	}
	
	void UpdateSpeed(int speedVal)
	{
		
	}

	public void InitZebraCharacter(long playerID, string playerName, int cellX, int cellY)
	{
		InitGeneralCharacter(m_zebraCharacter, playerID, playerName, cellX, cellY);
	}
	
	public void InitRhinoCharacter(long playerID, string playerName, int cellX, int cellY)
	{
		InitGeneralCharacter(m_rhinoCharacter, playerID, playerName, cellX, cellY);
	}
	
	public void InitTigerCharacter(long playerID, string playerName, int cellX, int cellY)
	{
		InitGeneralCharacter(m_tigerCharacter, playerID, playerName, cellX, cellY);
	}
	
	public void InitBirdCharacter(long playerID, string playerName, int cellX, int cellY)
	{
		InitGeneralCharacter(m_birdCharacter, playerID, playerName, cellX, cellY);
	}
	
	public void InitGeneralCharacter(GameObject animalCharacter, long playerID, string playerName, int cellX, int cellY)
	{
		characterInstance = Instantiate(animalCharacter, transform.position, transform.rotation) as GameObject;
		var animationScript = characterInstance.GetComponent<CharacterAnimController>();
		animationScript.EnemyPlayer = !isSelf;
		
		float spawnX = ZooMap.GetHorizontalPos(cellX);
		float spawnY = ZooMap.GetVerticalPos(cellY);
		
		animationScript.SpawnCharacter(spawnX, spawnY);
		
		if(isSelf)
		{
			characterInstance.gameObject.tag = "Player";		
		}
		
		characterInstance.gameObject.name = ""+playerID;
		
		// Instantiate name
		tk2dTextMesh nameScript = characterInstance.transform.Find("HUD_Name").GetComponent<tk2dTextMesh>();
		
		if(nameScript != null)
		{
			nameScript.text = playerName;
		}
	}
	
}


