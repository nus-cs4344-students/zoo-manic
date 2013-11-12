using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;

public class ClientSocket : MonoBehaviour {

    public SockjsClient m_sockjs;// = new SockjsClient();

	//[SerializeField] Player playerScript;	
	//[SerializeField] CharacterType characterType;	
	//[SerializeField] ZooMap zooMapScript;
	
	//[SerializeField] GameObject enemyPlayer;
	
	[SerializeField] GameManager gameManager;
	
	//static int sessionId = 100000;
	//static long playerId;
	
	// status 0 - success, > 1 means exception
	private long status;

	ArrayList messageTypeList = new ArrayList();
	ArrayList contentList = new ArrayList();
	
	private List<Player> playerList;
	private List<Lobby> lobbyList = new List<Lobby>();
	private List<Avatar> roomAvatarList = new List<Avatar>();
	bool hasGameStarted = false;
	
	// time in milliseconds
	private long localTimeStamp;
	
	private static long LOCAL_GAME_CLOCK;
	private static long SERVER_GAME_CLOCK;
	private static long LOCAL_START_TIME;
	private static long SERVER_START_TIME;
	
	private long SERVER_DELAY;	
	private long CURRENT_DELAY;

	AsyncOperation BombermanGame;
	
	// Make this game object and all its transform children
	// survive when loading a new scene.
	void Awake () 
	{
		DontDestroyOnLoad (gameObject);
	}
			// Use this for initialization
	public void Start()
	{
		InitSockJS();
	}
	
	void InitSockJS()
	{
		//m_sockjs.AutoPingRefreshMs = 2000;
		m_sockjs.OnMessage += OnMessage;
		m_sockjs.OnConnect += OnConnect;
		m_sockjs.OnDisconnect += OnDisconnect;
		
		//m_sockjs.Connect("http://localhost:5000/");
		//m_sockjs.Connect("http://ec2-54-225-24-113.compute-1.amazonaws.com:5000/");
	}
	
	public void ConnectToIP(string ipAddress)
	{
		string http = "http://";
		string port = ":5000/";
		
		string host = http + ipAddress + port;
		m_sockjs.Connect(host);
	}

    private void OnMessage(string serverMsg)
    {
		var dict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		
		//Debug.Log ("SERVER MESSAGE: "+serverMsg);
		
		if(dict == null || dict.ContainsKey("type") == false)
		{
			Debug.Log ("No such key for type");
			return;
		}
		
		string messageType = (string) dict["type"];

		switch(messageType)
		{
			case "message":
			string messageContent = (string) dict["content"];
			status = (long) dict["status"];
			//HandleMessage(messageContent, (int) status);
			break;
			
			case "newPlayerReply":
			HandleNewPlayer(serverMsg);
			break;
			
			case "session":
			HandleGetSession(serverMsg);
			break;
			
			case "roomSession":
			HandleGetRoomSession(serverMsg);
			break;
			
			case "setPropertyReply":
			// If status is 0 means it is OK
			status = (long) dict["status"];
			HandleSetProperty( (int) status);
			break;
			
			case "readyReply":
			// If status is 0 means it is OK
			status = (long) dict["status"];
			if(status == 0)
			{
				HandleReadyReply( (int) status, (Dictionary<string, object>) dict["content"]);
			}
			else
				Debug.Log ("ERROR WITH STATUS REPLY: "+status);
			
			break;
			
			case "plantBombReply":
			HandlePlantBombMessage(serverMsg);
			break;
			
			//case "pingRefresh":
			//Debug.Log("Ping refresh from server");
			//break;
			
			case "ping":
			HandlePingMessage(serverMsg);
			break;
			
			case "start":
			SERVER_START_TIME = (long) dict["startTime"];
			LOCAL_START_TIME = GetLocalTimeStamp();
			
			StartCoroutine(LoadGameWorld(serverMsg));
			break;
			
			case "update":
			if(hasGameStarted)
				HandleUpdate(serverMsg);
			break;
			
			case "move":
			HandleMovement(serverMsg);
			break;
			
			case "pingRefresh":
			break;
			
			case "gameEnd":
			HandleGameEnd(serverMsg);
			break;
		}
    }
	
	IEnumerator LoadGameWorld(string serverMsg)
	{
		//yield return new WaitForSeconds(0.5f);
		BombermanGame = Application.LoadLevelAsync("BombermanScene");
	    while (!BombermanGame.isDone)
	    { yield return 0; }
		
		GameManager.CurrentScene = SceneType.Game;
		
		HandleGameStart(serverMsg);
		playerList = gameManager.GetPlayerList();
		hasGameStarted = true;
		
		// Handle Update Message from server
		if(hasGameStarted)
			HandleZooMap(serverMsg);
	}
	
	// back to lobby
	IEnumerator GoBacktoLobby()
	{
		yield return new WaitForSeconds(5.0f);
		GameManager.LoadLobbyScene();
	}
	
	private void HandleGameEnd(string serverMsg)
	{
		var dict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		/*long playerId = (long) dict["winnerId"];
		string playerName = (string) dict["winnerName"];
		
		if(playerId == GameManager.PlayerID)
		{
			gameManager.DisplayGameEndMessage("ROUND ENDED. YOU ARE THE WINNER.\nGoing to lobby in 5 seconds");
		}
		else
		{
			gameManager.DisplayGameEndMessage("ROUND ENDED. WINNER IS "+playerName+", You lose.\nGoing to lobby in 5 seconds");
		}*/
		
		gameManager.DisplayGameEndMessage("\nGoing to lobby in 5 seconds");
		
		StartCoroutine(GoBacktoLobby());
	}
	
	private void HandlePingMessage(string serverMsg)
	{
		var pingDict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		long timeStamp = (long) pingDict["timestamp"];
		long serialNo = (long) pingDict["serialNo"];
		SendPingMessage(timeStamp, GameManager.PlayerID, serialNo);
	}
	
	private void HandleKillMessage(string serverMsg)
	{
		Debug.Log("Kill MESSAGE message: "+serverMsg);
		
		var killMsgDict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		string message = (string) killMsgDict["content"];

		gameManager.DisplayKillMessage(message);
	}
	
	private void HandlePlantBombMessage(string serverMsg)
	{
		Debug.Log("Plant bomb message: "+serverMsg);
		
		var bombDict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		long playerId = (long) bombDict["playerId"];
		long bombX = (long) bombDict["bombX"];
		long bombY = (long) bombDict["bombY"];
		long bombLeft = (long) bombDict["bombLeft"];
		
		gameManager.PlantBomb(playerId, (float) bombX, (float) bombY, bombLeft);
	}

	private void HandleReadyReply(int serverReply, Dictionary<string, object> playerDict)
	{
		if( serverReply == 0 )
		{
			// A from the server that who update the thing
			long playerId = (long) playerDict["id"];
			long avatarId = (long) playerDict["avatarId"];
			string playerName = (string) playerDict["name"];

			roomAvatarList.Add (new Avatar(playerId, (int) avatarId, playerName) );
			
			GameObject.Find ("SceneObject").GetComponent<SceneManager>().UpdateAvatarList();
		}
		else
		{
			Debug.Log("SERVER REPLYING 0 --- ERROR");
		}
	}
	
	private void HandleSetProperty(int serverReply)
	{
		if(serverReply == 0)
			Debug.Log ("Set Property OK");
		else
			Debug.Log ("Set Property ERROR");
	}
	
	private void HandleMovement(string serverMsg)
	{
		Debug.Log ("Message Movement received from server: "+serverMsg);
		var movementDict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		long serverPlayerID = (long) movementDict["playerId"];
		long cellX = (long) movementDict["cellX"];		// starting point cell X
		long cellY = (long) movementDict["cellY"];		// starting point cell Y
		//long destX = (long) movementDict["destX"];
		//long destY = (long) movementDict["destY"];
		string direction = (string) movementDict["direction"];
		long movementSpeed = (long) movementDict["speed"];
		long serverTimeStamp = (long) movementDict["timestamp"];  // time stamp server start moving
		SERVER_DELAY = (long) movementDict["serverDelay"];

		float timeDifference = (float) CalculateTimeStampDifference((long) serverTimeStamp);
		gameManager.UpdatePosition(serverPlayerID, cellX, cellY, direction, (float) movementSpeed, timeDifference, SERVER_DELAY);
	}
	
	private void HandleMessage(string content, int status)
	{	
		// Game room is full
		if(content != null && status == 1 )
		{
			SceneManager sceneScript = GameObject.Find ("SceneObject").GetComponent<SceneManager>();
			sceneScript.RoomFull = true;
			Debug.Log ("ROOM IS FULL");
		}
	}
	
	private void HandleNewPlayer(string serverData)
	{
		//HandleNewPlayer(serverMsg);
		var dict = Json.Deserialize(serverData) as Dictionary<string,object>;
		long statusReply = (long) dict["status"];
		if(statusReply != 0)
		{
			Debug.LogError("Something went wrong in creating new player!");
			return;
		}
		
		long playerID = (long) dict["playerId"];
		GameManager.UpdatePlayerID(playerID);
		
		GameManager.LoadLobbyScene();
	}
	
	private void HandleGetRoomSession(string sessionData)
	{
		Debug.Log ("Server reply of session data is: "+sessionData);

		var dict = Json.Deserialize(sessionData) as Dictionary<string,object>;
		List<object> contentList = (List<object>) dict["content"];
		
		for(int i=0; i<contentList.Count; i++)
		{
			Dictionary<string, object> sessionDict = (Dictionary<string, object>) contentList[i];

			string serverSessionID =  (string) sessionDict["id"];
			long numOfPlayers = (long) sessionDict["size"];
			List<object> playerList = (List<object>) sessionDict["players"];

			// if session ID is equals to game manager one
			if(Convert.ToInt32(serverSessionID) == GameManager.SessionID)
			{
				PopulateRoomList(playerList);
				break;
			}
		}
		
		// After finish retrieving data from server, load the game room
		GameManager.LoadGameRoomScene();
	}
	
	private void PopulateRoomList(List<object> playerList)
	{
		if(playerList == null)
		{
			Debug.LogError("Playerlist is null");
			return;
		}
		
		for(int playerIndex=0; playerIndex<playerList.Count; playerIndex++)
		{
			Dictionary<string, object> playerDict = (Dictionary<string, object>) playerList[playerIndex];
			long playerId = (long) playerDict["id"];
			string playerName = (string) playerDict["name"];
			long avatarId = (long) playerDict["avatarId"];
			roomAvatarList.Add (new Avatar(playerId, (int) avatarId, playerName) );
		}
	}
	
	public List<Avatar> GetRoomAvatarList()
	{
		return roomAvatarList;
	}
	
	private void HandleGetSession(string sessionData)
	{
		/*
		 *   "type": "session",
    "content": [
        {
            "id": "100000",
            "size": 1,
            "players": [
                {
                    "id": 1382425642568,
                    "name": "a",
                    "avatarId": 0
                }
            ]
        },
		*/
		
		Debug.Log ("Session Data: "+sessionData);
		
		var dict = Json.Deserialize(sessionData) as Dictionary<string,object>;
		List<object> contentList = (List<object>) dict["content"];
		
		for(int i=0; i<contentList.Count; i++)
		{
			Dictionary<string, object> sessionDict = (Dictionary<string, object>) contentList[i];

			string sessionID =  (string) sessionDict["id"];
			long numOfPlayers = (long) sessionDict["size"];
	
			lobbyList.Add (new Lobby( Convert.ToInt32(sessionID), (int) numOfPlayers, false));
		}
		
		GameObject.Find ("SceneObject").GetComponent<SceneManager>().UpdateLobbyList();
	}
	
	private float CalculateTimeStampDifference(long serverCurrentTime)
	{
		//localTimeStamp = GetLocalTimeStamp();
		//return serverTimeStamp - localTimeStamp;
		
		LOCAL_GAME_CLOCK = GetLocalTimeStamp() - LOCAL_START_TIME;
		SERVER_GAME_CLOCK = serverCurrentTime - SERVER_START_TIME;
		
		Debug.Log ("Local Game Clock: " + LOCAL_GAME_CLOCK);
		Debug.Log ("Server Game Clock: " + SERVER_GAME_CLOCK);
		
		Debug.Log ("Local Start Time: " + LOCAL_START_TIME);
		Debug.Log ("Server Start Time: " + SERVER_START_TIME);
		
		CURRENT_DELAY = LOCAL_GAME_CLOCK - SERVER_GAME_CLOCK - SERVER_DELAY;
		
		// time stamp difference, convert windows ticks to seconds
		//return (float) (LOCAL_GAME_CLOCK - SERVER_GAME_CLOCK - SERVER_DELAY) / 1000;
		return (float) (LOCAL_GAME_CLOCK - SERVER_GAME_CLOCK) / 1000;
	}
	
	private long GetLocalTimeStamp()
	{
		long localTimeStamp = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
		localTimeStamp /= 10000; //Convert windows ticks to seconds*/
		return localTimeStamp;
	}
	
	private void HandleGameStart(string dataFromServer)
	{
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		List<object> contentList = (List<object>) dict["content"];
		//SERVER_DELAY = (long) dict["serverDelay"];
		
		//Debug.Log ("Message from server: "+ dataFromServer);
		
		//{"type":"start","content":[{"id":1383373806006,"name":"456","delay":339,"isAlive":false,"avatarId":2,"sessionId":"100004"}],"timestamp":1383373811664}
		
		for(int i=0; i<contentList.Count; i++)
		{
			Dictionary<string, object> gameDict = (Dictionary<string, object>) contentList[i];
			//List<object> listOfPlayers = (List<object>) gameDict["content"];
			
			//IteratePlayerList(listOfPlayers);
			
			long serverPlayerId = (long) gameDict["id"];
			string playerName = (string) gameDict["name"];
			long avatarId = (long) gameDict["avatarId"];
			long spawnX = (long) gameDict["spawnX"];
			long spawnY = (long) gameDict["spawnY"];
			gameManager.InitCharacter(serverPlayerId, (int) avatarId, (int) spawnX, (int) spawnY, SERVER_DELAY);
		}
	}
	
	public void SendPingMessage(long time, long playerId, long serialNum)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("ping");
		
		messageTypeList.Add("timestamp");
		contentList.Add(time);
		
		messageTypeList.Add("playerId");
		contentList.Add(playerId);
		
		messageTypeList.Add("serialNo");
		contentList.Add(serialNum);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendNewPlayerMessage(string playerName)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("newPlayer");
		
		messageTypeList.Add("playerName");
		contentList.Add(playerName);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendSetPropertyMessage(string playerName, int avatarID)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("setProperty");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		messageTypeList.Add("properties");
		Dictionary<string, object> propertiesList = new Dictionary<string, object>();
		//BY RIGHT
		//propertiesList.Add("name", playerScript.PlayerName);	
		//propertiesList.Add("avatarId", playerScript.AvatarId);
		
		propertiesList.Add("name", playerName);	
		//propertiesList.Add("avatarId", avatarID);

		contentList.Add(propertiesList);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	IEnumerator WaitForGameToLoad(float duration, string serverMsg)
    {
		Debug.Log ("Waiting for 3 Seconds for server to reply");
		yield return new WaitForSeconds(duration);   //Wait duration sec for server to reply
    }
	
	private void HandleZooMap(string dataFromServer)
	{
		// Server should not send update even if i have not start the game
		// Game is not started (ZooMap instance is null)
		if(GameManager.CurrentScene != SceneType.Game)
		{
			return;
		}
		
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		Dictionary<string, object> zooMapDict = (Dictionary<string, object>) dict["zooMap"];
		
		/*for(int index=0; index < (int) ZooMap.NumberofRows * ZooMap.NumberofCols; index++)
		{
			Dictionary<string, object> zooMapInfoDict = (Dictionary<string, object>) zooMapDict[""+index];
			//long cellType = (long) zooMapInfoDict["type"];
			long cellType = (long) zooMapInfoDict["tile_type"];
			long cellItem = (long) zooMapInfoDict["item"];
			long horizontalCellNum = (long) zooMapInfoDict["x"];
			long verticalCellNum = (long) zooMapInfoDict["y"];
			
			//zooMapScript.UpdateZooMap(cellType, cellItem, horizontalCellNum, verticalCellNum, index);
			gameManager.UpdateMap(cellType, cellItem, horizontalCellNum, verticalCellNum, index);
		}*/
		
		int keyIndex = 0;
		
		for(int row=0; row < (int) ZooMap.NumberofRows ; row++)
		{
			for(int column=0; column < (int) ZooMap.NumberofCols ; column++)
			{
				Dictionary<string, object> zooMapInfoDict = (Dictionary<string, object>) zooMapDict[""+keyIndex];
				//long cellType = (long) zooMapInfoDict["type"];
				long cellType = (long) zooMapInfoDict["tile_type"];
				long cellItem = (long) zooMapInfoDict["item"];
				long horizontalCellNum = (long) zooMapInfoDict["x"];
				long verticalCellNum = (long) zooMapInfoDict["y"];
				string cellID = row+":"+column;
				//Debug.Log ("SERVER IS : " + horizontalCellNum+":"+verticalCellNum);
				//Debug.Log ("CLIENT IS : " + cellID);
				
				keyIndex++;
				gameManager.UpdateMap(cellType, cellItem, horizontalCellNum, verticalCellNum, cellID);
			}
		}
	}
	
	private void HandlePlayerUpdate(string dataFromServer)
	{
		// Server should not send update even if i have not start the game
		// Game is not started (ZooMap instance is null)
		if(GameManager.CurrentScene != SceneType.Game)
		{
			return;
		}
		
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		Dictionary<string, object> playersDict = (Dictionary<string, object>) dict["players"];
		
		/*for(int i=0; i<playersDict.Count; i++)
		{
			Dictionary<string, object> playerInfo = (Dictionary<string, object>) playersDict[i+""];
			long serverPlayerID = (long) playerInfo["playerId"];
			//long playerCellX = (long) playerInfo["x"];
			//long playerCellY = (long) playerInfo["y"];
			//long playerSpeed = (long) playerInfo["speed"];
			long bombLeft = (long) playerInfo["bombLeft"];
			bool isAlive = (bool) playerInfo["isAlive"];
			
			gameManager.UpdatePlayerStatus(serverPlayerID, bombLeft, isAlive);
		}*/
		
		// dictKeys here is all the playerID
		foreach(string serverPlayerID in playersDict.Keys)
		{
			Dictionary<string, object> playerInfo = (Dictionary<string, object>) playersDict[serverPlayerID];
			long bombLeft = (long) playerInfo["bombLeft"];
			bool isAlive = (bool) playerInfo["isAlive"];
			List<object> powerupList = (List<object>) playerInfo["items"];
			
			gameManager.UpdatePlayerStatus(serverPlayerID, bombLeft, isAlive, powerupList);
		}
	}
	
	private void HandleUpdate(string dataFromServer)
	{
		Debug.Log ("Update: "+dataFromServer);
		
		// Server should not send update even if i have not start the game
		// Game is not started (ZooMap instance is null)
		if(GameManager.CurrentScene != SceneType.Game)
		{
			return;
		}
		
		if(playerList == null)
		{
			return;
		}
		

		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		
		Dictionary<string, object> bombDict = (Dictionary<string, object>) dict["bombs"];
		//Dictionary<string, object> zooMapDict = (Dictionary<string, object>) dict["zooMap"];
		
		/*
		 * {"type":"update","bombs":{"exploded":[],"active":[]},"players":{"1382421891543":{}},"zooMap":{"0":{"type":0,"item":0},"1":{"type":0,"item":0}
		 * */
		List<object> explosionList = (List<object>) bombDict["exploded"];
		List<object> activeList = (List<object>) bombDict["active"];
		
		// Handle bomb explosion updates
		for(int i=0; i<explosionList.Count; i++)
		{
			Dictionary<string, object> explodeInfo = (Dictionary<string, object>) explosionList[i];
			long serverPlayerID = (long) explodeInfo["playerId"];
			long cellX = (long) explodeInfo["x"];
			long cellY = (long) explodeInfo["y"];
			long explodeRange = (long) explodeInfo["range"];
			
			gameManager.ExplodeBomb(serverPlayerID, cellX, cellY, explodeRange);
		}
		
		// If there is zoomap key, update the cells
		if( dict.ContainsKey ("zooMap") )
			HandleZooMap(dataFromServer);
		
		if( dict.ContainsKey ("players") )
			HandlePlayerUpdate(dataFromServer);
		
		if( dict.ContainsKey ("killMessage"))
			HandleKillMessage(dataFromServer);
		
		
		// Updating the ZOOMap if there is update
		
		// Updating the list of player's 
		/*for(int index=0; index<playerList.Count; index++)
		{
			Player player = playerList[index];
			long serverPlayerID = player.PlayerID;
			
			// If does not contains the key, break out of the loop e.g. Connection is lost
			if( !playersDict.ContainsKey(""+serverPlayerID) )
				break;
			
			Dictionary<string, object> playersInfoDict = (Dictionary<string, object>) playersDict[""+serverPlayerID];
			
			// Updating player's position
			if(playersInfoDict != null)
			{
				long cellX = (long) playersInfoDict["x"];
				long cellY = (long) playersInfoDict["y"];

				gameManager.UpdatePosition(serverPlayerID, cellX, cellY);
			}
		}*/
		
		
		//"players":{"1382421891543":{}}
		// iterate through each cells, and get the cell status
	}
	
	private void ClearList()
	{
		messageTypeList.Clear();
		contentList.Clear();
	}
	
	// Send bomb's position to server (current cell position)
	public void SendPlantBombMessage(float cellX, float cellY)
	{
		ClearList();
		
		int bombCellX = (int) cellX;
		int bombCellY = (int) cellY;
		
		messageTypeList.Add("type");
		contentList.Add("plantBomb");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);		
		
		messageTypeList.Add("x");
		contentList.Add(bombCellX);
		
		messageTypeList.Add("y");
		contentList.Add(bombCellY);		
		
		if(CURRENT_DELAY < 0)
			CURRENT_DELAY = 0;
		
		messageTypeList.Add("delay");
		contentList.Add(CURRENT_DELAY);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	// Send player's movement to server (current cell position)
	public void SendMovementMessage(float cellX, float cellY, string direction, float moveSpeed)
	{
		ClearList();
		
		int newCellX = (int) cellX;
		int newCellY = (int) cellY;
		int newMoveSpeed = (int) moveSpeed;
		
		messageTypeList.Add("type");
		contentList.Add("move");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		messageTypeList.Add("cellX");
		contentList.Add(newCellX);
		
		messageTypeList.Add("cellY");
		contentList.Add(newCellY);
		
		messageTypeList.Add("direction");
		contentList.Add(direction);
		
		messageTypeList.Add("speed");
		contentList.Add(newMoveSpeed);
		
		// Send to server in milliseconds
		//long millisecondsDelay = CalculateTimeStampDifference
		//CalculateTimeStampDifference
		//messageTypeList.Add("delay");
		
		if(CURRENT_DELAY < 0)
			CURRENT_DELAY = 0;
		
		messageTypeList.Add("delay");
		contentList.Add(CURRENT_DELAY);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendReadyMessage(int avatarID)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("playerReady");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		messageTypeList.Add("avatarId");
		contentList.Add(avatarID);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendStartMessage()
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("start");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void ConnectToLobby()
	{
		//type: newPlayer content {playerName: "ABC"}
		// after message send playerId
		
		// Connect to lobby
		//ClearList();
		
		//
		//Call before JoinRoom message to see who is inside the room
		//SendGetSessionMessage();
		
		// Player 1 joins a room
		//SendJoinARoomMessage();
		// After selecting avatar, send player's your avatar
		//SendSetPropertyMessage();
		
		// Get the current player ID
		//SendReadyMessage();
		//SendStartMessage();
		
		// get a list of player in that room, must call after join a room message
		//SendGetSessionMessage();
		
		//InitCharacter();
		
		//StartCoroutine (WaitOneSecond ());
	}
	
	public List<Lobby> GetLobbyList()
	{
		return lobbyList;
	}
	
	// Get a List of LOBBY
	public void SendGetAllSessionMessage()
	{
		ClearList(); 
		
		// clear the previous list
		lobbyList.Clear();
		
		messageTypeList.Add("type");
		contentList.Add("getAllSession");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendGetRoomSession()
	{
		ClearList(); 
		
		roomAvatarList.Clear();
		
		messageTypeList.Add("type");
		contentList.Add("getRoomSession");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendJoinARoomMessage(int sessionID)
	{
		ClearList();

		messageTypeList.Add("type");
		contentList.Add("setSession");
		
		messageTypeList.Add("playerId");
		contentList.Add(GameManager.PlayerID);
		
		messageTypeList.Add("sessionId");
		contentList.Add (sessionID);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	// Wait one second
	IEnumerator WaitOneSecond()
    {
   		yield return new WaitForSeconds(3.0f);
    }
	
	private void SendMessageToServer(ArrayList typeList, ArrayList dataList)
	{
		if(Utility.IsEmpty(typeList) || Utility.IsEmpty(dataList))
		{
			Debug.Log ("Err: TYPE OR DATA IS NULL");
			return;
		}
		
		Dictionary<string,object> connectionDict = new Dictionary<string,object>();
		
		// Iterate a list of message and add them
		for(int index=0; index<typeList.Count; index++)
		{
			string type = typeList[index] as string;
			object data = dataList[index];
			
			connectionDict.Add(type, data);		// message format is always type: start
		}
		
		var connectStr = Json.Serialize(connectionDict);
		m_sockjs.SendData(connectStr);
	}

    private void OnConnect()
    {
		//Debug.Log("Connected to SockJS server");
		
		GameObject sceneManager = GameObject.Find ("SceneObject");
		if(sceneManager != null)
		{
			if(sceneManager.GetComponent<SceneManager>().CurrentScene == SceneType.Start)
			{
				Debug.Log ("Connected to SockJS Server");
				sceneManager.GetComponent<SceneManager>().buttonScript.buttonType = ButtonType.Start_Tap;
				sceneManager.GetComponent<SceneManager>().buttonScript.buttonText.text = "Tap to Start";
				sceneManager.GetComponent<SceneManager>().m_playerTextBox.SetActive(true);
				sceneManager.GetComponent<SceneManager>().m_ipTextBox.SetActive(true);
				//sceneManager.GetComponent<SceneManager>() = "Enter your name";
			}
		}
    }
	
  	private void OnDisconnect(int _code, string _msg)
    {
		Debug.LogError("Disconnected: "+ _msg + " Error code: " + _code);
    }
	
	private void JsonSample()
	{
		/*var jsonString = "{ \"array\": [1.44,2,3], " +
						"\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
						"\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
						"\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
						"\"int\": 65536, " +
						 "\"float\": 3.1415926, " +
						"\"bool\": true, " +
						"\"null\": null }";
		
		
    	var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;

   		Debug.Log("deserialized: " + dict.GetType());
    	Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    	Debug.Log("dict['string']: " + (string) dict["string"]);
    	Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
    	Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
   		Debug.Log("dict['unicode']: " + (string) dict["unicode"]);

    	var str = Json.Serialize(dict);*/
		
		//var json2 = "{\"type\":\"session\",\"content\": \"\"}";
		var json2 = "{\"type\":\"session\",\"content\":[{\"id\":\"100003\",\"size\":0,\"players\":[]}]}";
		var dict = Json.Deserialize(json2) as Dictionary<string,object>;
		Debug.Log("dict['type']: " + (string) dict["type"]);

    	//Debug.Log("serialized: " + str);		
	}
}