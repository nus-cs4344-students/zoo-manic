using UnityEngine;
using System.Collections;

public class ClientSocket : MonoBehaviour {

    public SockjsClient sockjs;

    public void Start()
    {
        sockjs.OnMessage += OnMessage;
        sockjs.OnConnect += OnConnect;

        // connect to wherever your server is running
		
		// string url = "http://localhost:9999/echo/";
		
		string url = "http://localhost:4344/pong/";
		
        sockjs.Connect(url);
    }

    private void OnMessage(string _msg)
    {
        // got message
        Debug.Log(_msg);
    }

    private void OnConnect()
    {
        Debug.Log("connected");

        sockjs.SendData("hello world");
    }
}