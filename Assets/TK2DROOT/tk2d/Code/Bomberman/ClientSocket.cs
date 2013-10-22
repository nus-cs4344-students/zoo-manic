using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;

public class ClientSocket : MonoBehaviour {

    public SockjsClient sockjs;

	[SerializeField] Player playerScript;	
	[SerializeField] CharacterType characterType;	
	[SerializeField] ZooMap zooMapScript;
	
	[SerializeField] GameObject enemyPlayer;
	
	static int sessionId = 100000;
	static long playerId;
	
	ArrayList messageTypeList = new ArrayList();
	ArrayList contentList = new ArrayList();

    public void Start()
    {
		if(playerScript.IsMyself)
		{
			sockjs.OnMessage += OnMessage;
       		sockjs.OnConnect += OnConnect;

        	// connect to wherever your server is running
			// string url = "http://localhost:9999/echo/";
		
			string url = "http://localhost:5000/zoo/";
		
        	sockjs.Connect(url);
		}
        
    }

    private void OnMessage(string serverMsg)
    {
        // Receive the JSON message from server
        //Debug.Log("Received Msg: "+serverMsg);
		
		var dict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		string messageType = (string) dict["type"];
		string messageContent = "";
		
		switch(messageType)
		{
			case "message":
			messageContent = (string) dict["content"];
			Debug.Log ("content: "+messageContent);
			break;
			
			case "session":
			Debug.Log("RECEIVE SESSION MSG: "+serverMsg);
			StoreSessionID(serverMsg);
			break;
			
			case "getPlayerId":
			playerId = (long) dict["content"];
			Debug.Log ("Player ID IS: "+playerId);
			break;
			
			case "update":
			//HandleUpdate(serverMsg);
			break;
		}
    }
	
	private void StoreSessionID(string dataFromServer)
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
		
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		
		List<object> contentList = (List<object>) dict["content"];
		
		for(int i=0; i<contentList.Count; i++)
		{
			Dictionary<string, object> sessionDict = (Dictionary<string, object>) contentList[i];
			int sessionId = Convert.ToInt32(sessionDict["id"]);
			long size = (long) sessionDict["size"];
			List<object> playerList = (List<object>) sessionDict["players"];
			
			IteratePlayerList(playerList);
		}
		
	}
	
	private void SendSetPropertyMessage()
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("setProperty");
		
		messageTypeList.Add("properties");
		Dictionary<string, object> propertiesList = new Dictionary<string, object>();
		//BY RIGHT
		propertiesList.Add("name", playerScript.PlayerName);	
		propertiesList.Add("avatarId", playerScript.AvatarId);

		contentList.Add(propertiesList);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	private void IteratePlayerList(List<object> playerList)
	{
		Debug.Log ("LIST COUNT IS: "+playerList.Count);
		
		for(int i=0; i<playerList.Count; i++)
		{
			Dictionary<string, object> playerDict = (Dictionary<string, object>) playerList[i];
			long serverPlayerId = (long) playerDict["id"];
			string playerName = (string) playerDict["name"];
			long avatarId = (long) playerDict["avatarId"];
			
			// I am this player
			if(playerId == serverPlayerId)
			{
				InitPlayerCharacter();
				Debug.Log ("Create my own Character!!");
			}
			else
			{
				InitEnemyCharacter((int) avatarId);
				Debug.Log ("Creating Enemy Character!!");
			}
			
			Debug.Log ("Playerid: "+playerId);
			Debug.Log ("Playername: "+playerName);
			Debug.Log ("AvatarId: "+avatarId);
		}
	}
	
	private void HandleUpdate(string dataFromServer)
	{
		var dict = Json.Deserialize(dataFromServer) as Dictionary<string,object>;
		
		Dictionary<string, object> bombDict = (Dictionary<string, object>) dict["bombs"];
		Dictionary<string, object> playersDict = (Dictionary<string, object>) dict["players"];
		Dictionary<string, object> zooMapDict = (Dictionary<string, object>) dict["zooMap"];
		
		/*
		 * {"type":"update","bombs":{"exploded":[],"active":[]},"players":{"1382421891543":{}},"zooMap":{"0":{"type":0,"item":0},"1":{"type":0,"item":0}
		 * */
		List<object> bombList = (List<object>) bombDict["exploded"];
		List<object> activeList = (List<object>) bombDict["active"];
		
		//"players":{"1382421891543":{}}
		Dictionary<string, object> playersInfoDict = (Dictionary<string, object>) playersDict[""+playerId];
		
		//long bombLeft = (long) playersInfoDict["bombLeft"];
		//Debug.Log ("bombleft: "+bombLeft);
		//long x = (long) playersInfoDict["x"];
		//long y = (long) playersInfoDict["y"];
		
		// iterate through each cells, and get the cell status
		for(int index=0; index< (int) ZooMap.horizontalCell * ZooMap.verticalCell; index++)
		{
			Dictionary<string, object> zooMapInfoDict = (Dictionary<string, object>) zooMapDict[""+index];
			long cellType = (long) zooMapInfoDict["type"];
			long cellItem = (long) zooMapInfoDict["item"];
			long horizontalCellNum = (long) zooMapInfoDict["x"];
			long verticalCellNum = (long) zooMapInfoDict["y"];
			
			zooMapScript.UpdateZooMap(cellType, cellItem, horizontalCellNum, verticalCellNum, index);
			//Debug.Log ("Cell num is: "+horizontalCellNum+""+verticalCellNum);
		}
		
		//playerScript.UpdatePosition(x, y);
		
		//Debug.Log ("KEY IS: "+zooMapDict.);
		
		//{"0":{"type":0,"item":0}
		
		/*Dictionary<string, object> zooMapInfoDict = (Dictionary<string, object>) zooMapDict["0"];
		long cellType = (long) zooMapInfoDict["type"];
		long cellItem = (long) zooMapInfoDict["item"];*/

		//Dictionary<string, object> zooMapInfoDict = Dictionary<string, object>) zooMapDict["0"];


		//Debug.Log ("x: "+x + "   y " + y + "   bomb left "+bombLeft);
		/*long x = (long) playersInfoList[0];
		long y = (long) playersInfoList[1];
		long bombLeft = (long) playersInfoList[2];
		Debug.Log ("X: "+ x);
		Debug.Log ("Y: "+ y);*/
		
		
		//Debug.Log ("Data is: "+bombDict);
		//string data = bombData.ToString();
		//Debug.Log ("BOMB DATA IS : "+data);
		//var bombDict = Json.Deserialize(bombData) as Dictionary<string,object>;
		//
		//Debug.Log ("Bomb List: "+ bombList);
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
	
	private void SendReadyMessage()
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("ready");
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	private void SendStartMessage()
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("start");
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	private void ConnectToLobby()
	{
		// Connect to lobby
		ClearList();
		
		// Player 1 joins a room
		SendJoinARoomMessage();
		// After selecting avatar, send player's your avatar
		SendSetPropertyMessage();
		
		// Get the current player ID
		SendReadyMessage();
		SendStartMessage();
		
		// get a list of player in that room, must call after join a room message
		SendGetSessionMessage();
		
		//InitCharacter();
		
		//StartCoroutine (WaitOneSecond ());
	}
	
	private void SendGetSessionMessage()
	{
		ClearList(); 
		
		messageTypeList.Add("type");
		contentList.Add("getSession");
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	private void SendJoinARoomMessage()
	{
		ClearList();
		
		messageTypeList.Add("type");
		contentList.Add("selectSession");
		
		messageTypeList.Add("sessionId");
		contentList.Add (sessionId);
		
		SendMessageToServer(messageTypeList, contentList);
	}
	
	// Wait one second
	IEnumerator WaitOneSecond()
    {
   		yield return new WaitForSeconds(3.0f);
    }

	
	private void InitPlayerCharacter()
	{
		switch(characterType)
		{
			case CharacterType.Rhino:
			playerScript.InitRhinoCharacter();
			break;
			
			case CharacterType.Zebra:
			playerScript.InitZebraCharacter();
			break;
		}
	}
	
	private void InitEnemyCharacter(int avatarID)
	{
		// rhino
		if(avatarID == 0)
		{
			//playerScript.InitRhinoCharacter();
			var enemyGameObj = Instantiate(enemyPlayer, transform.position, transform.rotation) as GameObject;
			var enemyScript = enemyGameObj.GetComponent<Player>();
			enemyScript.InitRhinoCharacter();
		}
		// zebra
		else if(avatarID == 1)
		{
			//playerScript.InitZebraCharacter();
			var enemyGameObj = Instantiate(enemyPlayer, transform.position, transform.rotation) as GameObject;
			var enemyScript = enemyGameObj.GetComponent<Player>();
			enemyScript.InitZebraCharacter();
		}
		
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
		Debug.Log ("Message to send: "+connectStr);
		sockjs.SendData(connectStr);
	}

    private void OnConnect()
    {
		Debug.Log("connected");
		
		ConnectToLobby();
    }
	
	private void JsonSample()
	{
		var jsonString = "{ \"array\": [1.44,2,3], " +
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

    	var str = Json.Serialize(dict);

    	Debug.Log("serialized: " + str);		
	}
}