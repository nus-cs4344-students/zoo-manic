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
	
	public void UpdatePosition(long serverPlayerID, float cellX, float cellY, string direction, float moveSpeed)
	{
		// If it is other player, then update the position
		if(serverPlayerID != PlayerID)
		{
			GameObject characterObject = GameObject.Find(""+serverPlayerID);

			CharacterAnimController movementController = characterObject.GetComponent<CharacterAnimController>();
			movementController.PlayerSpeed = moveSpeed;
			
			Debug.Log ("Updating playerId position: "+serverPlayerID + " Direction: "+direction);
			
			switch(direction)
			{
				case "UP":
				movementController.MoveUp(false);
				break;
				
				case "DOWN":
				movementController.MoveDown(false);
				break;
				
				case "LEFT":
				movementController.MoveLeft(false);
				break;
				
				case "RIGHT":
				movementController.MoveRight(false);
				break;
			}
			
			//Debug.LogError("Updating Position: CELL X: "+cellX + " CELL Y: "+cellY);
			
			//characterObject.GetComponent<CharacterAnimController>().UpdatePosition(positionX, positionY);
			//characterObject.GetComponent<CharacterAnimController>().MoveUp();
		}
	}
	
	public void InitCharacter(long playerID, int avatarID, int cellX, int cellY)
	{
		AvatarIcon avatarType = Avatar.GetAvatarIcon(avatarID);
		// If it is the player
		if(playerID == GameManager.PlayerID)
		{
			switch(avatarType)
			{
				case AvatarIcon.Rhino:
				playerScript.InitRhinoCharacter(playerID, cellX, cellY);
				break;
				

				case AvatarIcon.Zebra:
				playerScript.InitZebraCharacter(playerID, cellX, cellY);
				break;
				
				case AvatarIcon.Tiger:
				playerScript.InitTigerCharacter(playerID, cellX, cellY);
				break;
				
				case AvatarIcon.Cassowary:
				playerScript.InitBirdCharacter(playerID, cellX, cellY);
				break;
			}
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
				enemyScript.InitRhinoCharacter(playerID, cellX, cellY);
				break;
			
				// zebra
				case AvatarIcon.Zebra:
				enemyScript.InitZebraCharacter(playerID, cellX, cellY);
				break;
				
				// tiger
				case AvatarIcon.Tiger:
				enemyScript.InitTigerCharacter(playerID, cellX, cellY);
				break;
				
				// bird
				case AvatarIcon.Cassowary:
				enemyScript.InitBirdCharacter(playerID, cellX, cellY);
				break;
			}
		}
		
		playerList.Add (new Player(playerID, avatarID) );
	}
	
	public List<Player> GetPlayerList()
	{
		return playerList;
	}
	
	/*public void InitPlayerCharacter()
	{
		switch(AvatarID)
		{
			// rhino
			case 0:
			playerScript.InitRhinoCharacter();
			break;
			
			// zebra
			case 1:
			playerScript.InitZebraCharacter();
			break;
		}
	}
	
	public void InitEnemyCharacter(int avatarID, int playerID)
	{
		
		var enemyGameObj = Instantiate(enemyGameObject, transform.position, transform.rotation) as GameObject;
		var enemyScript = enemyGameObj.GetComponent<Player>();
		
		switch(avatarID)
		{
			// rhino
			case 0:
			enemyScript.InitRhinoCharacter();
			break;
			
			// zebra
			case 1:
			enemyScript.InitZebraCharacter();
			break;
		}
	}*/
	
	public void UpdateMap(long cellType, long cellItem, long horizontalCellNum, long verticalCellNum, int cellNum)
	{
		zooMapScript.UpdateZooMap(cellType, cellItem, horizontalCellNum, verticalCellNum, cellNum);
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
