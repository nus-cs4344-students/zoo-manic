using UnityEngine;
using System.Collections;

public enum SceneType {Start, MainMenu, Lobby, GameRoom, Game}

public class SceneManager : MonoBehaviour {
	
	// Each scenes have it's own scene manager to do the intialisation of the states
	[SerializeField] SceneType m_currentScene;	
	
	// Initialized all the components needed
	private ClientSocket serverConnection;
	
	// room ID = sessionID in server connection
	private int roomID = 100000;
	
	// Start Scene UI Components
	[SerializeField] tk2dTextMesh m_playerName;
	
	// Lobby
	private bool isRoomFull = false;
	
	// Game Room
	[SerializeField] tk2dTextMesh m_readyButton;
	
	void Awake()
	{
		serverConnection = GameObject.Find ("PlayerConnection").GetComponent<ClientSocket>();
	}

	public bool RoomFull
	{
        get { return isRoomFull; }
        set { isRoomFull = value; }
    }
	
	public string PlayerName
	{
        get { return m_playerName.text; }
        set { m_playerName.text = value; }
    }
	
	// Use this for initialization
	void Start () 
	{
		switch(m_currentScene)
		{
			case SceneType.Start:
			Debug.Log ("Start scene");
			break;
			
			case SceneType.MainMenu:
			Debug.Log ("Main Menu scene");
			break;
			
			case SceneType.Lobby:
			Debug.Log ("Lobby scene");
			break;
		
			case SceneType.GameRoom:
			Debug.Log ("Game Room scene");
			break;
			
			case SceneType.Game:
			Debug.Log ("Game scene");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	// Ready
	public void Ready()
	{
		//serverConnection.EnterGame();
		
		// Player chooses avatar
		string playerName = "ZebraPlayer";
		
		// 0 - Rhino, 1 - Zebra, 2 - Tiger
		int avatarID = 1;
		
		GameManager.UpdateAvatarID(avatarID);
		
		serverConnection.SendSetPropertyMessage(playerName, avatarID);
		
		// Player's is READY
		serverConnection.SendReadyMessage();
		
		m_readyButton.text = "Start Game";
	}
	
	public void StartGame()
	{	
		// Host presses START
		serverConnection.SendStartMessage();
		
		// Get session for a LIST of players
		serverConnection.SendGetSessionMessage();
	}
	
	
	public void ConnectToRoom()
	{
		GameManager.UpdateSessionID(roomID);
		
		// reset the value
		isRoomFull = false;
		serverConnection.SendJoinARoomMessage(roomID);
	}
	
	public void UpdatePlayerName()
	{
		string name = m_playerName.text;
		
		GameManager.UpdatePlayerName(name);
		Debug.Log ("Updating player name: "+name);
	}
}
