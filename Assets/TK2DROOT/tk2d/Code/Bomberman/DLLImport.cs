using UnityEngine;
using System.Collections;
using SocketIOClient;
using WebSocket4Net;
using SuperSocket;
using SimpleJson;
using System;

public class DLLImport : MonoBehaviour {
	private Client client;

	// Use this for initialization
	void Start () {
		//connectToNodeJS();
	}
	
	private void connectToNodeJS()
	{
		string socketUrl = "http://ec2-54-225-24-113.compute-1.amazonaws.com:5000";
    	Debug.Log("socket url: " + socketUrl);
		
		client = new Client(socketUrl);
		client.Message += SocketMessage;
		client.SocketConnectionClosed += SocketConnectionClosed;
		client.Message +=SocketError;
		
		this.client.On("message", (data) => {
        	Debug.Log("message: " + data);
    	});
		
		client.Connect();
	}
	
	private void SocketOpened(object sender, MessageEventArgs e) {
    	//invoke when socket opened
	}
	
	private void SocketError(object sender, MessageEventArgs e) {
    	//invoke when socket encounters error
	}
	
	private void SocketConnectionClosed(object sender, EventArgs e) {
    	//invoke when socket encounters error
	}
	
	private void SocketMessage (object sender, MessageEventArgs e) {
    	if ( e!= null && e.Message.Event == "message") {
       		string msg = e.Message.MessageText;
       		//process(msg);
    	}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
