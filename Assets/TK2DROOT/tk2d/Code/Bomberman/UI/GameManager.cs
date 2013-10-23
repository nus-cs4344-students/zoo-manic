using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	// Player Information
	public static string PlayerName;
	public static int AvatarID;
	public static int SessionID;		// roomID
	
	public static long PlayerID;
	
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
	
	public void InitPlayerCharacter()
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
	
	public void InitEnemyCharacter(int avatarID)
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
	}
	
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
		Application.LoadLevel("BombermanScene");
	}
}
