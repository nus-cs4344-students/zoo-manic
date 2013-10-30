using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	// Player Information
	public static string PlayerName;
	public static long AvatarID;
	public static int SessionID;		// roomID
	
	public static long PlayerID;
	
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
	
	public void UpdatePosition(long serverPlayerID, float cellX, float cellY)
	{
		// If it is other player, then update the position
		if(serverPlayerID != PlayerID)
		{
			GameObject characterObject = GameObject.Find(""+serverPlayerID);
			float positionX = ZooMap.GetHorizontalPos(cellX);
			float positionY = ZooMap.GetVerticalPos(cellY);
			
			//Debug.LogError("Updating Position: CELL X: "+cellX + " CELL Y: "+cellY);
			
			characterObject.GetComponent<CharacterAnimController>().UpdatePosition(positionX, positionY);
		}
	}
	
	public void InitCharacter(long playerID, int avatarID, int cellX, int cellY)
	{
		// If it is the player
		if(playerID == GameManager.PlayerID)
		{
			switch(avatarID)
			{
				// rhino
				case 0:
				playerScript.InitRhinoCharacter(playerID, cellX, cellY);
				break;
				
				// zebra
				case 1:
				playerScript.InitZebraCharacter(playerID, cellX, cellY);
				break;
			}
		}
		// It is the enemy
		else
		{
			var enemyGameObj = Instantiate(enemyGameObject, transform.position, transform.rotation) as GameObject;
			var enemyScript = enemyGameObj.GetComponent<Player>();
			switch(avatarID)
			{
				// rhino
				case 0:
				enemyScript.InitRhinoCharacter(playerID, cellX, cellY);
				break;
			
				// zebra
				case 1:
				enemyScript.InitZebraCharacter(playerID, cellX, cellY);
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
