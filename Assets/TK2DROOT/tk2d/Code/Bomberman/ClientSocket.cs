﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;

public class ClientSocket : MonoBehaviour {

    public SockjsClient m_sockjs;// = new SockjsClient();
	
	//public TCPSocket m_sockjs;// = new SockjsClient();
	
	//public TCPConnector m_sockjs;

	//[SerializeField] Player playerScript;	
	//[SerializeField] CharacterType characterType;	
	//[SerializeField] ZooMap zooMapScript;
	
	//[SerializeField] GameObject enemyPlayer;
	
	[SerializeField] GameManager gameManager;
	
	//static int sessionId = 100000;
	//static long playerId;
	
	// status 0 - success, > 1 means exception
	private long status;
	
	public static string threadMsg;
	public static bool isMsgUpdated;
	public static object Lock = new object();
	
	ArrayList messageTypeList = new ArrayList();
	ArrayList contentList = new ArrayList();
	
	private List<Player> playerList;
	private List<Lobby> lobbyList = new List<Lobby>();
	private bool hasLobbyInfo = false;
	
	bool hasGameStarted = false;
	
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
		//m_sockjs.AutoPingRefreshMs = 2000;		// don't do the auto ping refresh
		//m_sockjs.OnMessage += OnMessage;
		//m_sockjs.OnConnect += OnConnect;
		
		//string url = "http://localhost:5000/";
		//string host = "localhost";
		//Int32 portNum = 5000;
		
		//JsonSample();
		
		//InitTCPConnector();
		
		InitSockJS();
	}
	
	void InitSockJS()
	{
		m_sockjs.AutoPingRefreshMs = 2000;
		m_sockjs.OnMessage += OnMessage;
		m_sockjs.OnConnect += OnConnect;
		m_sockjs.OnDisconnect += OnDisconnect;
		
		m_sockjs.Connect("http://localhost:5000/");
		//m_sockjs.Connect("http://ec2-54-225-24-113.compute-1.amazonaws.com:5000/");
	}
	
	
	
	void InitTCPConnector()
	{
		//m_sockjs = new TCPConnector();
		//m_sockjs.OnMessage += OnMessage;
		//string message = m_sockjs.fnConnectResult("ec2-54-225-24-113.compute-1.amazonaws.com", 5000);
		//string message = m_sockjs.fnConnectResult("localhost", 5000);
		//Debug.Log ("TCP Connection Result: "+message);
	}
	
	void Update()
	{
		/*string msg = null;
		lock (Lock) 
		{
			//if(isMsgUpdated && threadMsg != null)
			//{	
			//	OnMessage(threadMsg);
			//}
			
			//msg = m_sockjs.m_messageQueue.RemoveAt(0);
			
			if(m_sockjs.m_messageQueue.Count > 0 && isMsgUpdated){
				msg = m_sockjs.m_messageQueue[0];
				
				//m_sockjs.m_messageQueue.Clear();				
				m_sockjs.m_messageQueue.RemoveAt(0);
			}
		}
		
		if (msg != null)
			OnMessage(msg);
		*/
	}

    private void OnMessage(string serverMsg)
    {
        // Receive the JSON message from server
		isMsgUpdated = false;

		var dict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		
		//Debug.Log ("SERVER MESSAGE: "+serverMsg);
		
		if(dict == null || dict.ContainsKey("type") == false)
		{
			Debug.Log ("No such key for type");
			return;
		}
		
		string messageType = (string) dict["type"];
		string messageContent = "";

		switch(messageType)
		{
			case "message":
			messageContent = (string) dict["content"];
			HandleMessage(messageContent);
			break;
			
			case "session":
			HandleGetSession(serverMsg);
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
			
			case "pingRefresh":
			Debug.Log("Ping refresh from server");
			break;
			
			case "start":
			StartCoroutine(LoadGameWorld(serverMsg));
			break;
			
			case "update":
			if(hasGameStarted)
				HandleUpdate(serverMsg);
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
	}
	
	private void HandleReadyReply(int serverReply, Dictionary<string, object> playerDict)
	{
		if( serverReply == 0 )
		{
			// A reply
			GameManager.PlayerID = (long) playerDict["id"];
			//GameManager.AvatarID = (long) playerDict["avatarId"];
			
			Debug.Log ("PLAYER ID IS: "+GameManager.PlayerID);
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
	
	private void HandleMessage(string content)
	{
		Debug.Log ("Server message content is: "+content);
		
		// Game room is full
		if(content != null && content.ToLower ().Equals ("the room is full.") )
		{
			SceneManager sceneScript = GameObject.Find ("SceneObject").GetComponent<SceneManager>();
			sceneScript.RoomFull = true;
			Debug.Log ("ROOM IS FULL");
		}
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
		
		hasLobbyInfo = true;
		
		//GameObject.Find ("SceneObject").GetComponent<SceneManager>().UpdatedLobbyList();
	}
	
	private void HandleGameStart(string dataFromServer)
	{
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		List<object> contentList = (List<object>) dict["content"];
		
		for(int i=0; i<contentList.Count; i++)
		{
			Dictionary<string, object> gameDict = (Dictionary<string, object>) contentList[i];
			//List<object> listOfPlayers = (List<object>) gameDict["content"];
			
			//IteratePlayerList(listOfPlayers);
			
			long serverPlayerId = (long) gameDict["id"];
			string playerName = (string) gameDict["name"];
			long avatarId = (long) gameDict["avatarId"];
			long spawnX = (long) gameDict["x"];
			long spawnY = (long) gameDict["y"];
			gameManager.InitCharacter(serverPlayerId, (int) avatarId, (int) spawnX, (int) spawnY);
			
			Debug.Log ("CREATED CHARACTER: "+serverPlayerId);
		}
	}
	
	public void SendSetPropertyMessage(string playerName, int avatarID)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("setProperty");
		
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
	
	/*private void IteratePlayerList(List<object> listOfPlayers)
	{
		for(int i=0; i<listOfPlayers.Count; i++)
		{
			Dictionary<string, object> playerDict = (Dictionary<string, object>) listOfPlayers[i];
			long serverPlayerId = (long) playerDict["id"];
			string playerName = (string) playerDict["name"];
			long avatarId = (long) playerDict["avatarId"];
			
			
			gameManager.InitCharacter(serverPlayerId, (int) avatarId);
			
			Debug.Log ("Playerid: "+serverPlayerId);
			Debug.Log ("Playername: "+playerName);
			Debug.Log ("AvatarId: "+avatarId);
		}
	}*/
	
	IEnumerator WaitForGameToLoad(float duration, string serverMsg)
    {
		Debug.Log ("Waiting for 3 Seconds for server to reply");
		yield return new WaitForSeconds(duration);   //Wait duration sec for server to reply
    }
	
	private void HandleUpdate(string dataFromServer)
	{
		// Server should not send update even if i have not start the game
		// Game is not started (ZooMap instance is null)
		if(GameManager.CurrentScene != SceneType.Game)
		{
			return;
		}
		
		if(playerList == null)
		{
			//Debug.Log ("PLAYER LIST IS NULL");
			return;
		}
		
		//Debug.Log ("Received: data " + dataFromServer);
		
		
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		
		Dictionary<string, object> bombDict = (Dictionary<string, object>) dict["bombs"];
		Dictionary<string, object> playersDict = (Dictionary<string, object>) dict["players"];
		Dictionary<string, object> zooMapDict = (Dictionary<string, object>) dict["zooMap"];
		
		/*
		 * {"type":"update","bombs":{"exploded":[],"active":[]},"players":{"1382421891543":{}},"zooMap":{"0":{"type":0,"item":0},"1":{"type":0,"item":0}
		 * */
		List<object> bombList = (List<object>) bombDict["exploded"];
		List<object> activeList = (List<object>) bombDict["active"];
		
		// Updating the list of player's 
		for(int index=0; index<playerList.Count; index++)
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
		}
		
		
		//"players":{"1382421891543":{}}
		// iterate through each cells, and get the cell status
		for(int index=0; index < (int) ZooMap.horizontalCell * ZooMap.verticalCell; index++)
		{
			Dictionary<string, object> zooMapInfoDict = (Dictionary<string, object>) zooMapDict[""+index];
			//long cellType = (long) zooMapInfoDict["type"];
			long cellType = (long) zooMapInfoDict["tile_type"];
			long cellItem = (long) zooMapInfoDict["item"];
			long horizontalCellNum = (long) zooMapInfoDict["x"];
			long verticalCellNum = (long) zooMapInfoDict["y"];
			
			//zooMapScript.UpdateZooMap(cellType, cellItem, horizontalCellNum, verticalCellNum, index);
			gameManager.UpdateMap(cellType, cellItem, horizontalCellNum, verticalCellNum, index);
		}
		
	}
	
	private void ClearList()
	{
		messageTypeList.Clear();
		contentList.Clear();
	}
	
	// Send player's movement to server
	public void SendMovementMessage(float playerX, float playerY)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("move");
		
		messageTypeList.Add("x");
		contentList.Add(playerX);
		
		messageTypeList.Add("y");
		contentList.Add(playerY);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendReadyMessage(int avatarID)
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("ready");
		
		messageTypeList.Add("avatarId");
		contentList.Add(avatarID);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendStartMessage()
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("start");
		
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
	public void SendGetSessionMessage()
	{
		ClearList(); 
		
		// clear the previous list
		lobbyList.Clear();
		hasLobbyInfo = false;
		
		messageTypeList.Add("type");
		contentList.Add("getAllSession");
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	public void SendJoinARoomMessage(int sessionID)
	{
		ClearList();

		messageTypeList.Add("type");
		contentList.Add("setSession");
		
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
		//Debug.Log ("Message to send: "+connectStr);
		m_sockjs.SendData(connectStr);
	}

    private void OnConnect()
    {
		Debug.Log("Connected to SockJS server");
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