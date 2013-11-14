using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	// Player Information
	public static string PlayerName;
	public static long AvatarID = -1;
	public static int SessionID;		// roomID
	
	public static long PlayerID = -1;
	
	private List<Player> playerList = new List<Player>();
	
	public static SceneType CurrentScene;
	
	[SerializeField] Player playerScript;	
	[SerializeField] GameObject enemyGameObject;	
	[SerializeField] ZooMap zooMapScript;

	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	public void DisplayKillMessage(string message)
	{
		GameObject hud = GameObject.Find ("UnityHUDPrefab");
		if(hud)
		{
			HUD hudScript = hud.GetComponent<HUD>();
			hudScript.DisplayKillMessage(message);
		}
	}
	
	public void DisplayChatMessage(long serverPlayerID, string message)
	{
		GameObject hud = GameObject.Find ("UnityHUDPrefab");
		
		GameObject characterObject = GameObject.Find(""+serverPlayerID);
		if(characterObject != null && hud != null)
		{
			HUD hudScript = hud.GetComponent<HUD>();
			hudScript.HudToogleChatDisplay(characterObject, message);
		}
	}
	
	public void DisplayGameEndMessage(long serverPlayerID, string winnerName)
	{
		GameObject hud = GameObject.Find ("UnityHUDPrefab");
		
		SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
		if(soundManager != null)
			soundManager.PlayGameOverSound();
		
		if(hud)
		{
			HUD hudScript = hud.GetComponent<HUD>();
			if(serverPlayerID == GameManager.PlayerID)
				hudScript.DisplayVictoryPanel(winnerName);
			else
				hudScript.DisplayDefeatPanel(winnerName);	
		}
	
	}
	
	public void PlantBomb(long serverPlayerID, float cellX, float cellY, long bombLeft)
	{
		GameObject characterObject = GameObject.Find(""+serverPlayerID);
		// If the player ID is me, then i update my own prefab bombleft
		
		if(characterObject)
		{
			CharacterAnimController playerController = characterObject.GetComponent<CharacterAnimController>();
			playerController.PlantBomb(cellX, cellY);	
			
			if(serverPlayerID == PlayerID)
			{
				GameObject hud = GameObject.Find ("UnityHUDPrefab");
				if(hud)
				{
					HUD hudScript = hud.GetComponent<HUD>();
					hudScript.SetBombLeft(playerController.BombLimit);
				}
				
				// Play Sound
				SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
				soundManager.PlayPlantBombSound(new Vector3(ZooMap.GetHorizontalPos(cellX), ZooMap.GetVerticalPos(cellY), 0));
			}
		}
	}
	
	public void ExplodeBomb(long serverPlayerID, float cellX, float cellY, long explodeRange)
	{
		GameObject characterObject = GameObject.Find(""+serverPlayerID);
		if(characterObject)
		{
			CharacterAnimController playerController = characterObject.GetComponent<CharacterAnimController>();
			playerController.ExplodeBomb(cellX, cellY, explodeRange);	
			
			// Update HUD
			if(serverPlayerID == PlayerID)
			{
				Debug.Log ("EXPLODE RANGE IS: "+explodeRange);
				playerController.UpdateBombLeft();
			}
		}
	}
	
	public void RespawnPlayer(long serverPlayerID, float cellX, float cellY)
	{
		GameObject characterObject = GameObject.Find(""+serverPlayerID);
		if(characterObject)
		{
			CharacterAnimController playerController = characterObject.GetComponent<CharacterAnimController>();
			
			SoundManager soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
			soundManager.PlayDeathSound(playerController.CharacterIcon, characterObject.transform.position);
			
			playerController.RespawnPlayer(cellX, cellY);
		}
	}
	
	public void UpdatePlayerStatus(string serverPlayerID, long bombLeft, bool isAlive, 
								   List<object> powerupList, long playerLives)
	{
		GameObject characterObject = GameObject.Find(serverPlayerID);
		GameObject hud = GameObject.Find ("UnityHUDPrefab");	
		HUD hudScript = null;
		
		
		// If game object can be found
		if(characterObject)
		{
			// If server says player is dead
			if(isAlive == false)
			{
				// Make camera movable (Spectator Mode)
				if(serverPlayerID == PlayerID.ToString())
				{
					GameObject cameraObject = GameObject.Find("tk2dCamera");
					if(cameraObject)
						cameraObject.GetComponent<SmoothCamera2D>().SetTargetAlive = false;
				}
				// Play Sound
				CharacterAnimController characterController = characterObject.GetComponent<CharacterAnimController>();
				Destroy(characterObject);
			}
			
			// If the character is myself. I Toggle the powerup
			if(serverPlayerID == PlayerID.ToString())
			{
				if(hud)
				{
					hudScript = hud.GetComponent<HUD>();
				}
				
				// Update Lives
				hudScript.UpdateHealthStatus( (int) playerLives);
				
				// no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
				for(int powerupIndex = 0; powerupIndex<powerupList.Count; powerupIndex++)
				{
					long powerupID = (long) powerupList[powerupIndex];
					
					//Debug.Log ("Powerup Index IS: "+powerupIndex    +"    ID is: "+powerupID);
					
					if(powerupIndex == 1)
					{
						hudScript.ToggleRange(powerupID != 0);
					}
					else if(powerupIndex == 2)
						hudScript.ToggleHaste(powerupID != 0);
					else if(powerupIndex == 3)
						hudScript.ToggleInvulnerable(powerupID != 0);
					else if(powerupIndex == 4)
						hudScript.ToggleTrick(powerupID != 0);
					else if(powerupIndex == 5)
						hudScript.ToggleShake(powerupID != 0);					
				}
			}
		}
	}
	
	public void UpdatePosition(long serverPlayerID, float cellX, float cellY, string direction, float moveSpeed, float timeDifference, float serverDelay)
	{	
		GameObject characterObject = GameObject.Find(""+serverPlayerID);
		// If player already been destroyed (e.g. dead)
		if(characterObject == null)
		{
			return;
		}
		
		float bufferTime = 0.05f;
		// If it is the enemy, add a local buffer time to handle jitter
		/*if(serverPlayerID != PlayerID)
		{
			timeDifference += bufferTime;
		}*/
		
		
		// If it is a player, add a local lag
		if(serverPlayerID == PlayerID)
		{
			timeDifference = 2 * serverDelay/1000.0f;
		}
		
		timeDifference += bufferTime;

		CharacterAnimController movementController = characterObject.GetComponent<CharacterAnimController>();
		float defaultSpeed = moveSpeed;		// movement speed from the server
		
		if(timeDifference < 0)
			timeDifference *= -1;
		
		// Delayed time here can be faster or slower
		float delayedTime = (ZooMap.cellWidth / defaultSpeed) + timeDifference;
		
		Debug.Log ("DELAYED TIME!!!: "+delayedTime);
		Debug.Log ("SERVER SEND SPEED!!!: "+moveSpeed);
		
		movementController.PlayerSpeed = ZooMap.cellWidth / delayedTime;
		
		Debug.Log("Time Difference: " + timeDifference);
		Debug.Log ("LPF Speed is : " + movementController.PlayerSpeed);

		// If it is the player
		/*if(serverPlayerID == PlayerID)
		{
			// set the local lag delay and player will move by it's own
			movementController.LOCAL_LAG_DELAY = (float) 2 * serverDelay/1000.0f;
			movementController.PlayerSpeed = moveSpeed;
			
			//Debug.Log ("PLAYER LOCAL LAG DELAY IS: "+movementController.LOCAL_LAG_DELAY );
		}
		*/
		
		
		// If enemy, do LocalPerceptionFilter
		if(serverPlayerID != PlayerID)
		{
			switch(direction)
			{
				case "UP":
				movementController.MoveUp(cellX, cellY, false);
				break;
				
				case "DOWN":
				movementController.MoveDown(cellX, cellY, false);
				break;
				
				case "LEFT":
				movementController.MoveLeft(cellX, cellY, false);
				break;
				
				case "RIGHT":
				movementController.MoveRight(cellX, cellY, false);
				break;
			}
		}
	}
	
	public void InitAvatarHUD(AvatarIcon avatarType, long playerID)
	{
		GameObject hud = GameObject.Find ("UnityHUDPrefab");
		if(hud)
		{
			HUD hudScript = hud.GetComponent<HUD>();
			hudScript.SetPlayerAvatarHUD(avatarType);
			
			hudScript.SetPlayerID(playerID);
		}
	}
	
	public void InitCharacter(long playerID, int avatarID, int cellX, int cellY, long serverDelay, string playerName)
	{
		AvatarIcon avatarType = Avatar.GetAvatarIcon(avatarID);
		
		float localLagDelay = (float) 2* serverDelay / 1000;
		
		// If it is the player
		if(playerID == GameManager.PlayerID)
		{
			switch(avatarType)
			{
				case AvatarIcon.Rhino:
				playerScript.InitRhinoCharacter(playerID, playerName, cellX, cellY);
				break;
				

				case AvatarIcon.Zebra:
				playerScript.InitZebraCharacter(playerID, playerName, cellX, cellY);
				break;
				
				case AvatarIcon.Tiger:
				playerScript.InitTigerCharacter(playerID, playerName, cellX, cellY);
				break;
				
				case AvatarIcon.Cassowary:
				playerScript.InitBirdCharacter(playerID, playerName, cellX, cellY);
				break;
			}
			
			InitAvatarHUD(avatarType, playerID);
		}
		// It is the enemy
		else
		{
			var enemyGameObj = Instantiate(enemyGameObject, transform.position, transform.rotation) as GameObject;
			var enemyScript = enemyGameObj.GetComponent<Player>();
			switch(avatarType)
			{
				// rhino
				case AvatarIcon.Rhino:
				enemyScript.InitRhinoCharacter(playerID, playerName, cellX, cellY);
				break;
			
				// zebra
				case AvatarIcon.Zebra:
				enemyScript.InitZebraCharacter(playerID, playerName, cellX, cellY);
				break;
				
				// tiger
				case AvatarIcon.Tiger:
				enemyScript.InitTigerCharacter(playerID, playerName, cellX, cellY);
				break;
				
				// bird
				case AvatarIcon.Cassowary:
				enemyScript.InitBirdCharacter(playerID, playerName, cellX, cellY);
				break;
			}
		}
		
		playerList.Add (new Player(playerID, avatarID) );
	}
	
	public List<Player> GetPlayerList()
	{
		return playerList;
	}
	
	public void UpdateMap(long cellType, long cellItem, long horizontalCellNum, long verticalCellNum, string cellID)
	{
		zooMapScript.UpdateZooMap(cellType, cellItem, horizontalCellNum, verticalCellNum, cellID);
	}
	
	public static void UpdatePlayerID(long id)
	{
		PlayerID = id;
	}
	
	public static void UpdatePlayerName(string name)
	{
		PlayerName = name;
	}
	
	public static void UpdateAvatarID(int avatarID)
	{
		AvatarID = avatarID;
	}
	
	public static void UpdateSessionID(int roomID)
	{
		SessionID = roomID;
	}
	
	public static void LoadMainScene()
	{
		CurrentScene = SceneType.MainMenu;
		Application.LoadLevel("MainMenu");
	}
	
	public static void LoadLobbyScene()
	{
		CurrentScene = SceneType.Lobby;
		Application.LoadLevel("Lobby");
	}
	
	public static void LoadStartScene()
	{
		CurrentScene = SceneType.Start;
		Application.LoadLevel("Start");
	}
	
	public static void LoadGameRoomScene()
	{
		CurrentScene = SceneType.GameRoom;
		Application.LoadLevel ("GameRoom");
	}
	
	public static void LoadGameScene()
	{
		CurrentScene = SceneType.Game;
		//Application.LoadLevel("BombermanScene");
		AsyncOperation async = Application.LoadLevelAsync("BombermanScene");
		
		//yield return async;
		Debug.Log ("Loading complete");
	}
}
