using UnityEngine;
using System.Collections;
using SocketIOClient;
using WebSocket4Net;
using SuperSocket;
using SimpleJson;
using System;
using pomeloUnityClient;

public class DLLImport : MonoBehaviour {
	private Client socket;
	public static PomeloClient pclient = null;

	// Use this for initialization
	void Start () {
		//connectToNodeJS();
		
		//LocalHost local = new LocalHost("http://localhost:5000/");
		//local.init();
		//local.sendMessage("
		
		
		//Note: your data can only be numbers and strings.  This is not a solution for object serialization or anything like that.
		/*JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		//number
		j.AddField("field1", 0.5);
		//string
		j.AddField("field2", "sampletext");
		
		local.sendMessage(1, "hello", j);*/

		//pclient.disconnect();
	}
	
	private void ConnectChat()
	{
		string url = "http://localhost:5000";
		pclient = new PomeloClient(url);
		pclient.init();
		JsonObject userMessage = new JsonObject();
		userMessage.Add("uid", "abc");
	}
	
	private void connectToNodeJS()
	{
		//string socketUrl = "http://ec2-54-225-24-113.compute-1.amazonaws.com:5000/zoo";
		//string socketUrl = "http://ec2-54-225-24-113.compute-1.amazonaws.com/zoo";
		//string socketUrl = "https://localhost:4344/pong";
		//string url = "http://localhost:5000/";
		
		//string host = "ec2-54-225-24-113.compute-1.amazonaws.com";
		string host = "localhost";
		int port = 5000;
		string url = "http://"+host+":"+port+"/";
    	Debug.Log("socket url: " + url);
		
		socket = new Client(url);
		this.socket.Opened += this.SocketOpened;
        this.socket.Message += this.SocketMessage;
        this.socket.SocketConnectionClosed += this.SocketConnectionClosed;
        this.socket.Error += this.SocketError;

		this.socket.On("message", (data) => {
        	Debug.Log("message: " + data);
    	});
				
		socket.Connect();
		
		//this.socket.Send("my other event");
		
		 //this.socket.Emit("welcome", { message: "Welcome!" });
		//this.socket.Emit("welcome", "hello world");
		
		StartCoroutine( WaitFor(2.0f) );
	}
	
	
	// Wait for server to reply with the list of lobby
	IEnumerator WaitFor(float duration)
    {
		Debug.Log ("Waiting for "+duration +" sec");
		yield return new WaitForSeconds(duration);   //Wait duration sec for server to reply
		
		Debug.Log ("Client is connected?: "+socket.IsConnected);
		
		socket.Send("{type: \"getAllSession\"}");
    }
	
	
	private void SocketOpened(object sender, EventArgs  e) {
    	 Debug.Log(e);
   		 Debug.Log("socket opened");
   		 this.socket.Emit("connection", "hello world");
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
