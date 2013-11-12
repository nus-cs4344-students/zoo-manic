using UnityEngine;
using System.Collections;

public enum ButtonType 
{
	Connect, Start_Tap, Main_Play, Main_Settings, Main_HowtoPlay, 
	Lobby_HostGame, Lobby_JoinGame, Lobby_BacktoMain, Lobby_Refresh,
	GameRoom_Ready, GameRoom_Start
}

public class Button : MonoBehaviour {
	
	
	[SerializeField] tk2dUIItem uiItem;
	[SerializeField] public tk2dTextMesh buttonText;
	[SerializeField] public ButtonType buttonType;
	
	// Button class calls scene manager, and scene manager calls serverconnection/gamemanager
	private SceneManager sceneManager;
	
	// Use this for initialization
	void Start () 
	{
		sceneManager = GameObject.Find ("SceneObject").GetComponent<SceneManager>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void OnEnable()
	{
	    uiItem.OnDown += ButtonDown;
	    uiItem.OnClickUIItem += Clicked;
	}
	
	void ButtonDown()
	{
		switch(buttonType)
		{
			case ButtonType.Connect:
			ConnectToServer();
			break;
			
			// go to main menu
			case ButtonType.Start_Tap:
			CreateNewPlayer();
			//GameManager.LoadMainScene();
			//GameManager.LoadLobbyScene();
			break;
			
			// Main Menu
			case ButtonType.Main_Play:
			GameManager.LoadLobbyScene();
			break;
			
			case ButtonType.Main_Settings:
			break;
			
			case ButtonType.Main_HowtoPlay:
			break;
			
			// Lobby
			case ButtonType.Lobby_BacktoMain:
			GameManager.LoadStartScene();
			//GameManager.LoadMainScene();
			break;
			
			case ButtonType.Lobby_JoinGame:
			ConnectToRoom();
			break;
			
			case ButtonType.Lobby_Refresh:
			DisplayLobby();
			break;
			
			// Entering GameRoom
			case ButtonType.GameRoom_Ready:
			Ready();
			break;
			
			case ButtonType.GameRoom_Start:
			StartGame();
			break;
		}
	}
	
	void UpdatePlayerName()
	{
		sceneManager.UpdatePlayerName();
	}
	
	void Ready()
	{
		bool isAvatarSelected = sceneManager.Ready();
		if(isAvatarSelected)
			buttonType = ButtonType.GameRoom_Start;
	}
	
	void StartGame()
	{
		sceneManager.StartGame();
		// Wait for game to start, the server replies a "start" message, then everybody change the scene
		//StartCoroutine( WaitForGame(3.0f) );
	}
	
	void ConnectToServer()
	{
		sceneManager.ConnectToServer();
	}
	
	void CreateNewPlayer()
	{
		UpdatePlayerName();
		sceneManager.CreateNewPlayer();
	}
	
	void DisplayLobby()
	{
		sceneManager.DisplayLobby();
	}
	
	void ConnectToRoom()
	{
		sceneManager.ConnectToRoom();
		
		//StartCoroutine( WaitForRoom(1.0f) );
	}
	
	// Check room is not full then enter game
	/*void EnterRoom()
	{
		if( !sceneManager.RoomFull )
		{
			Debug.Log ("Room is NOT full");
			GameManager.LoadGameRoomScene();
		}
		else
		{
			Debug.Log ("Room is FULL!!!");
		}
	}
	
	IEnumerator WaitForRoom(float duration)
    {
		Debug.Log ("Waiting for 3 Seconds!!");
		yield return new WaitForSeconds(duration);   //Wait duration sec for server to reply
		
		//EnterRoom();
    }*/
	
	void Clicked(tk2dUIItem clickedUIItem)
	{
	    //Debug.Log("Clicked:" + clickedUIItem);
	}
	
	//Also remember if you are adding event listeners to events you need to also remove them:
	void OnDisable()
	{
	    uiItem.OnDown -= ButtonDown;
	    uiItem.OnClickUIItem -= Clicked;
	}
}
