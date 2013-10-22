using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class ClientSocket : MonoBehaviour {

    public SockjsClient sockjs;

    public void Start()
    {
        sockjs.OnMessage += OnMessage;
        sockjs.OnConnect += OnConnect;

        // connect to wherever your server is running
		
		// string url = "http://localhost:9999/echo/";
		
		string url = "http://localhost:5000/zoo/";
		
        sockjs.Connect(url);
    }

    private void OnMessage(string serverMsg)
    {
        // Receive the JSON message from server
        Debug.Log("Original Unseralized Msg: "+serverMsg);
		
		var dict = Json.Deserialize(serverMsg) as Dictionary<string,object>;
		string messageType = (string) dict["type"];
		string messageContent = (string) dict["content"];
		
		
		Debug.Log ("Message Type is: "+messageType);
		Debug.Log ("Message Content is: "+messageContent);
    }

    private void OnConnect()
    {
		Debug.Log("connected");
		
		Dictionary<string,object> connectionDict = new Dictionary<string,object>();
		connectionDict.Add("type", "hello world");		// message format is always type: start
		
		//var jsonString = "{ \"type\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \"}";
		
		// Make it to a String, Object class
		//var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		//var str = Json.Serialize(dict);
		
		
		var connectStr = Json.Serialize(connectionDict);
		sockjs.SendData(connectStr);
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