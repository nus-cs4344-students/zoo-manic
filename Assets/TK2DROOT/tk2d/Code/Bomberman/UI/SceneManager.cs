using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SceneType {Start, MainMenu, Lobby, GameRoom, Game}

public class SceneManager : MonoBehaviour {
	
	// Each scenes have it's own scene manager to do the intialisation of the states
	[SerializeField] SceneType m_currentScene;	
	
	// Initialized all the components needed
	private ClientSocket serverConnection;
	
	// room ID = sessionID in server connection
	//private int roomID = 100000;
	
	// Start Scene UI Components
	[SerializeField] tk2dTextMesh m_playerName;
	
	// Lobby
	private bool isRoomFull = false;
	[SerializeField] GameObject m_LobbyItem1;
	[SerializeField] GameObject m_LobbyItem2;
	[SerializeField] GameObject m_LobbyItem3;
	[SerializeField] GameObject m_LobbyItem4;
	[SerializeField] GameObject m_LobbyItem5;
	[SerializeField] GameObject m_LobbyItem6;
	[SerializeField] GameObject m_LobbyItem7;
	[SerializeField] GameObject m_LobbyItem8;
	[SerializeField] GameObject m_LobbyItem9;
	[SerializeField] GameObject m_LobbyItem10;
	
	private List<GameObject> m_lobbyList = new List<GameObject>();
	private List<Lobby> m_serverLobbyData = new List<Lobby>();
	
	private int selectedRoom;
	
	// Game Room
	[SerializeField] tk2dTextMesh m_readyButton;
	
	void Awake()
	{
		serverConnection = GameObject.Find ("PlayerConnection").GetComponent<ClientSocket>();
		switch(m_currentScene)
		{
			case SceneType.Start:
			Debug.Log ("Start scene");
			break;
			
			case SceneType.MainMenu:
			Debug.Log ("Main Menu scene");
			break;
			
			case SceneType.Lobby:
			InitLobbyList();		// init the data structure
			Debug.Log ("Lobby scene");
			GetLobbyList();
			break;
		
			case SceneType.GameRoom:
			Debug.Log ("Game Room scene");
			break;
			
			case SceneType.Game:
			Debug.Log ("Game scene");
			break;
		}
	}
	
	public int SelectedRoom
	{
        get { return selectedRoom; }
        set { selectedRoom = value; }
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
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void InitPlayer1()
	{
		// Player chooses avatar
		string playerName = GameManager.PlayerName;
		
		// 0 - Rhino, 1 - Zebra, 2 - Tiger
		int avatarID = 1;
		
		GameManager.UpdateAvatarID(avatarID);
		
		serverConnection.SendSetPropertyMessage(playerName, avatarID);

	}
	
	void InitPlayer2()
	{
		// Player chooses avatar
		string playerName = GameManager.PlayerName;
		
		// 0 - Rhino, 1 - Zebra, 2 - Tiger
		int avatarID = 0;
		
		GameManager.UpdateAvatarID(avatarID);
		
		serverConnection.SendSetPropertyMessage(playerName, avatarID);
	}
	
	// Ready
	public void Ready()
	{
		//serverConnection.EnterGame();
	
		//InitPlayer1();
		
		InitPlayer2();
		
		Debug.Log ("AVATAR ID IS: "+(int) GameManager.AvatarID);
		
		// Player's is READY
		serverConnection.SendReadyMessage( (int) GameManager.AvatarID);
		
		m_readyButton.text = "Start Game";
	}
	
	public void StartGame()
	{	
		// Host presses START
		serverConnection.SendStartMessage();
		
		// SERVER WILL SEND A SESSION after it is being called
		// Get session for a LIST of players
		//serverConnection.SendGetSessionMessage();
	}
	
	
	public void ConnectToRoom()
	{
		Lobby lobby = m_serverLobbyData[selectedRoom-1];
		Debug.Log("Connecting to room of ID: "+lobby.SessionID);
		GameManager.UpdateSessionID(lobby.SessionID);
		
		// reset the value
		isRoomFull = false;
		serverConnection.SendJoinARoomMessage(lobby.SessionID);
	}
	
	public void UpdatePlayerName()
	{
		string name = m_playerName.text;
		
		GameManager.UpdatePlayerName(name);
		Debug.Log ("Updating player name: "+name);
	}
	
	// Selected room is the num. e.g. 1st row = 1
	public void UpdateSelectedRoom(int roomSelected)
	{
		selectedRoom = roomSelected;
	}
	
	// Wait for server to reply with the list of lobby
	IEnumerator WaitForLobbyList(float duration)
    {
		Debug.Log ("Waiting for "+duration +" sec");
		yield return new WaitForSeconds(duration);   //Wait duration sec for server to reply
		
		UpdateLobbyList();
    }
	
	void InitLobbyList()
	{
		m_lobbyList.Add(m_LobbyItem1);
		m_lobbyList.Add(m_LobbyItem2);
		m_lobbyList.Add(m_LobbyItem3);
		m_lobbyList.Add(m_LobbyItem4);
		m_lobbyList.Add(m_LobbyItem5);
		m_lobbyList.Add(m_LobbyItem6);
		m_lobbyList.Add(m_LobbyItem7);
		m_lobbyList.Add(m_LobbyItem8);
		m_lobbyList.Add(m_LobbyItem9);
		m_lobbyList.Add(m_LobbyItem10);
		
		for(int i=0; i<m_lobbyList.Count; i++)
		{
			m_lobbyList[i].SetActive(false);
		}
	}
	
	void GetLobbyList()
	{
		// wait for awhile and get the updated lobby list from the server
		serverConnection.SendGetSessionMessage();
		
		StartCoroutine( WaitForLobbyList(1.0f) );
	}
	
	public void UpdateLobbyList()
	{
		List<Lobby> serverLobby = serverConnection.GetLobbyList();
		if(serverLobby.Count > 0)
			UpdateLobbyUI(serverLobby);
		else
			Debug.Log ("No lobby retrieved!");
	}
	
	void UpdateLobbyUI(List<Lobby> list)
	{
		m_serverLobbyData = list;
		for(int i=0; i<list.Count; i++)
		{
			Lobby lobby = list[i];
			//Debug.Log("I IS: "+i +  "    lobby is: "+lobby.SessionID);
			
			if(m_lobbyList[i] != null)
			{
				m_lobbyList[i].SetActive(true);
				
				tk2dTextMesh lobbyInfo = m_lobbyList[i].GetComponentInChildren<tk2dTextMesh>();
				string info = lobby.SessionID + 
				"                                        " +
				lobby.NumofPlayers + "/4" + 
				"                                      " + 
				( (lobby.HasStarted) ? "Playing" : "Waiting");
				
				lobbyInfo.text = info;
			}
			else{
				Debug.LogError("Lobby list of index: "+i+" is NULL");
			}
		}
	}
	
	public void ClearLobbySelection()
	{
		/*m_LobbyItem1.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem2.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem3.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem4.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem5.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem6.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem7.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem8.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem9.transform.Find ("ListSelected").gameObject.SetActive(false);
		m_LobbyItem10.transform.Find ("ListSelected").gameObject.SetActive(false);
		
		m_LobbyItem1.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem2.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem3.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem4.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem5.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem6.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem7.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem8.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem9.GetComponent<tk2dUIHoverItem>().enabled = true;
		m_LobbyItem10.GetComponent<tk2dUIHoverItem>().enabled = true;*/
		
		for(int i=0; i<m_lobbyList.Count; i++)
		{
			m_lobbyList[i].transform.Find ("ListSelected").gameObject.SetActive(false);
			m_lobbyList[i].GetComponent<tk2dUIHoverItem>().enabled = true;
		}
	}
}
