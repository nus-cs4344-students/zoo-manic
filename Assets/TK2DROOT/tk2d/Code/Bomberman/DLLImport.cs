using UnityEngine;
using System.Collections;
using SocketIOClient;
using WebSocket4Net;
using SuperSocket;
using SimpleJson;
using System;

public class DLLImport : MonoBehaviour {
	private Client socket;

	// Use this for initialization
	void Start () {
		//connectToNodeJS();
	}
	
	/*private void connectToNodeJS()
	{
		//string socketUrl = "http://ec2-54-225-24-113.compute-1.amazonaws.com:5000/zoo";
		//string socketUrl = "http://ec2-54-225-24-113.compute-1.amazonaws.com/zoo";
		//string socketUrl = "https://localhost:4344/pong";
		string socketUrl = "http://localhost:4344/pong/";
		//http://127.0.0.1:50122
    	Debug.Log("socket url: " + socketUrl);
		
		socket = new Client(socketUrl);
		

		this.socket.Opened += this.SocketOpened;
        this.socket.Message += this.SocketMessage;
        this.socket.SocketConnectionClosed += this.SocketConnectionClosed;
        this.socket.Error += this.SocketError;

		this.socket.On("message", (data) => {
        	Debug.Log("message: " + data);
    	});
				
		socket.Connect();
		
		Debug.Log ("Client is connected?: "+socket.IsConnected);
	}*/
	
	private void SocketOpened(object sender, EventArgs  e) {
    	 Debug.Log(e);
   		 Debug.Log("socket opened");
   		 //this.socket.Emit("hello", "");
	}
	
	private void SocketError(object sender, ErrorEventArgs e) {
    	//invoke when socket encounters error
		Debug.Log ("Socket ERROR! " + e.Message);
	}
	
	private void SocketConnectionClosed(object sender, EventArgs e) {
    	//invoke when socket encounters error
		Debug.Log ("Socket Closed!");
	}
	
	private void SocketMessage (object sender, MessageEventArgs e) {
    	if ( e!= null && e.Message.Event == "message") {
       		string msg = e.Message.MessageText;
       		//process(msg);
			Debug.Log ("MESSAGE IS : "+msg);
			Debug.Log ("e.message IS : "+e.Message);
    	}
		Debug.Log ("Socket Message!");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
