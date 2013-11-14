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
	[SerializeField] public tk2dTextMesh m_playerName;	
	[SerializeField] public GameObject m_playerTextBox;	
	[SerializeField] public tk2dTextMesh m_startLabel;	
	
	[SerializeField] public Button buttonScript;	
	
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
	[SerializeField] GameObject m_AvatarItem1;
	[SerializeField] GameObject m_AvatarItem2;
	[SerializeField] GameObject m_AvatarItem3;
	[SerializeField] GameObject m_AvatarItem4;
	
	[SerializeField] tk2dTextMesh m_roomTitleText;
	[SerializeField] tk2dTextMesh m_playerStatusText;
	[SerializeField] tk2dTextMesh m_gameText;
	
	public SceneType CurrentScene
    {
        get { return m_currentScene; }
    }

	
	void Awake()
	{
		serverConnection = GameObject.Find ("PlayerConnection").GetComponent<ClientSocket>();
		switch(m_currentScene)
		{
			case SceneType.Start:
			serverConnection.ForceDisconnect();
			Debug.Log ("Start scene");
			break;
			
			case SceneType.MainMenu:
			Debug.Log ("Main Menu scene");
			break;
			
			case SceneType.Lobby:
			Debug.Log ("Lobby scene");
			GameObject zooMap = GameObject.Find("ZooMap");
			if(zooMap != null)
				zooMap.GetComponent<ZooMap>().StartZooMap();
			DisplayLobby();
			break;
		
			case SceneType.GameRoom:
			Debug.Log ("Game Room scene");
			DisplayRoom();
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
	public bool Ready()
	{
		if(GameManager.AvatarID == -1)
		{
			m_gameText.text = "Please select an avatar first!";
			return false;
		}
		
		LockAvatarSelection();
		
		serverConnection.SendSetPropertyMessage(GameManager.PlayerName, (int) GameManager.AvatarID);
		
		serverConnection.SendReadyMessage( (int) GameManager.AvatarID);
		m_readyButton.text = "Start Game";
		
		return true;
	}
	
	public void StartGame()
	{	
		int mapID = 2;
		
		// Host presses START
		serverConnection.SendStartMessage(mapID);
	}
	
	public void CreateNewPlayer()
	{
		serverConnection.SendNewPlayerMessage(GameManager.PlayerName);
	}
	
	public void ConnectToServer()
	{
		// Get the IP address from the textfield
		string ipAddress = m_playerName.text;
		serverConnection.ConnectToIP(ipAddress);
	}
	
	public void ConnectToRoom()
	{
		Lobby lobby = m_serverLobbyData[selectedRoom-1];
		Debug.Log("Connecting to room of ID: "+lobby.SessionID);
		
		GameManager.UpdateSessionID(lobby.SessionID);
		
		// reset the value
		isRoomFull = false;
		serverConnection.SendJoinARoomMessage(lobby.SessionID);

		// Send a get session message
		serverConnection.SendGetRoomSession();
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
	
	public void UpdateSelectedAvatar(int avatarID)
	{
		GameManager.UpdateAvatarID(avatarID);
	}
	
	public void UpdateAvatarList()
	{
		InitAvatarList();
		
		m_roomTitleText.text = "Room " + GameManager.SessionID;
		
		string statusText = "Welcome to Room, " + GameManager.PlayerName;		
		m_gameText.text = "";
		
		List<Avatar> avatarList = serverConnection.GetRoomAvatarList();
		for(int i=0; i<avatarList.Count; i++)
		{
			Avatar avatar = avatarList[i];
			
			if(avatar.AvatarType != AvatarIcon.NotSelected)
				statusText += "\n" + avatar.PlayerName + " selected " + Avatar.ToString(avatar.AvatarID);

			UpdateAvatar(avatar.AvatarID, avatar.SelectedPlayerID, avatar.AvatarType, avatar.PlayerName);
		}
		
		m_playerStatusText.text = statusText;
	}
	
	public void UpdateAvatar(int avatarID, long playerID, AvatarIcon avatarType, string playerName)
	{
		tk2dTextMesh nameScript = null;
		tk2dSprite avatarScript = null;
		
		//int selectedAvatar = Avatar
		switch(avatarType)
		{
			// If server returns avatarID of -1
			case AvatarIcon.NotSelected:
			return;
			break;
			
			case AvatarIcon.Zebra:
			nameScript = m_AvatarItem1.transform.Find ("Name").gameObject.GetComponent<tk2dTextMesh>();
			avatarScript = m_AvatarItem1.transform.Find ("Portrait").gameObject.GetComponent<tk2dSprite>();
			m_AvatarItem1.GetComponent<AvatarButton>().enabled = false;
			break;
			
			case AvatarIcon.Rhino:
			nameScript = m_AvatarItem2.transform.Find ("Name").gameObject.GetComponent<tk2dTextMesh>();
			avatarScript = m_AvatarItem2.transform.Find ("Portrait").gameObject.GetComponent<tk2dSprite>();
			m_AvatarItem2.GetComponent<AvatarButton>().enabled = false;
			break;
			
			case AvatarIcon.Tiger:
			nameScript = m_AvatarItem3.transform.Find ("Name").gameObject.GetComponent<tk2dTextMesh>();
			avatarScript = m_AvatarItem3.transform.Find ("Portrait").gameObject.GetComponent<tk2dSprite>();
			m_AvatarItem3.GetComponent<AvatarButton>().enabled = false;
			break;
			
			case AvatarIcon.Cassowary:
			nameScript = m_AvatarItem4.transform.Find ("Name").gameObject.GetComponent<tk2dTextMesh>();
			avatarScript = m_AvatarItem4.transform.Find ("Portrait").gameObject.GetComponent<tk2dSprite>();
			m_AvatarItem4.GetComponent<AvatarButton>().enabled = false;
			break;
		}
		
		// If they are selected, set the alpha to not so transparent
		if(nameScript)
			nameScript.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			
		if(avatarScript)
			avatarScript.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
	}
	
	public void InitAvatarList()
	{
		/* Set selected BG to false
		m_AvatarItem1.transform.Find ("SelectedBG").gameObject.SetActive(false);
		m_AvatarItem2.transform.Find ("SelectedBG").gameObject.SetActive(false);
		m_AvatarItem3.transform.Find ("SelectedBG").gameObject.SetActive(false);
		m_AvatarItem4.transform.Find ("SelectedBG").gameObject.SetActive(false);*/
		
		m_AvatarItem1.GetComponent<AvatarButton>().enabled = true;
		m_AvatarItem2.GetComponent<AvatarButton>().enabled = true;
		m_AvatarItem3.GetComponent<AvatarButton>().enabled = true;
		m_AvatarItem4.GetComponent<AvatarButton>().enabled = true;
	}
	
	public void DisplayRoom()
	{		
		UpdateAvatarList();
	}
	
	public void DisplayLobby()
	{
		InitLobbyList();
		GetLobbyList();
	}
	
	void InitLobbyList()
	{
		m_lobbyList.Clear();
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
		serverConnection.SendGetAllSessionMessage();
		
		//StartCoroutine( WaitForLobbyList(1.0f) );
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
	
	public void LockAvatarSelection()
	{
		m_AvatarItem1.GetComponent<AvatarButton>().enabled = false;
		m_AvatarItem2.GetComponent<AvatarButton>().enabled = false;
		m_AvatarItem3.GetComponent<AvatarButton>().enabled = false;
		m_AvatarItem4.GetComponent<AvatarButton>().enabled = false;
	}
	
	public void ClearAvatarSelection()
	{
		m_AvatarItem1.transform.Find ("SelectedBG").gameObject.SetActive(false);
		m_AvatarItem2.transform.Find ("SelectedBG").gameObject.SetActive(false);
		m_AvatarItem3.transform.Find ("SelectedBG").gameObject.SetActive(false);
		m_AvatarItem4.transform.Find ("SelectedBG").gameObject.SetActive(false);
	}
	
	public void SendChatToServer(string message)
	{
		if( !string.IsNullOrEmpty(message) )
			serverConnection.SendChatMessage(message);
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
			m_lobbyList[i].transform.Find ("ListHover").gameObject.SetActive(false);
			m_lobbyList[i].GetComponent<tk2dUIHoverItem>().enabled = true;
		}
	}
}
