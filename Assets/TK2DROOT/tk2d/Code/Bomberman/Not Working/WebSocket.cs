using UnityEngine;
using System.Collections;
//using SuperWebSocket.Client;
using SuperSocket;
using System;
 
public class WebSocket : MonoBehaviour {
	/*private string lastMessage = string.Empty;
	//public static string serverURI = "ws://192.168.100.196:12345/channels/0?userId=1";
	public static string serverURI = "ws://localhost:5000";
	public static WebSocket webSocket = new WebSocket(serverURI, "basic");
	
	
	void Awake () {
		Debug.Log("Awoke!");
        webSocket.OnClose += new EventHandler(webSocketClient_OnClose);
        webSocket.OnOpen += new EventHandler(webSocketClient_OnOpen);
        webSocket.OnMessage += new EventHandler<MessageEventArgs>(webSocketClient_OnMessage);
        webSocket.Connect();
	}
 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		Debug.Log("Destroy!");
		webSocket.Close();
	}
 
    void webSocketClient_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("OnOpen!");
		webSocket.Send("say:Hello WebSocket from Unity!");
    }
 
    void webSocketClient_OnMessage(object sender, MessageEventArgs e)
    {
        lastMessage = e.Message;
        Debug.Log("lastMessage = " + lastMessage);
    }
 
    void webSocketClient_OnClose(object sender, EventArgs e)
    {
        Debug.Log("OnClose!");
    }*/
}