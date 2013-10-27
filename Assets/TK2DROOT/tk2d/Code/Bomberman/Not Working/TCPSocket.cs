using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Random = UnityEngine.Random;
 
public class TCPSocket : MonoBehaviour {
	
	/*private TcpClient client;
	
	static void Connect(String server, int port, String message) 
	{
	  try 
	  {
	    // Create a TcpClient. 
	    // Note, for this client to work you need to have a TcpServer  
	    // connected to the same address as specified by the server, port 
	    // combination.
	    //Int32 port = 5000;
	   
	
	    // Translate the passed message into ASCII and store it as a Byte array.
	    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);         
	
	    // Get a client stream for reading and writing. 
	   //  Stream stream = client.GetStream();
	
	    NetworkStream stream = client.GetStream();
	
	    // Send the message to the connected TcpServer. 
	    stream.Write(data, 0, data.Length);
	
	    Debug.Log(String.Format("Sent: {0}", message));         
	
	    // Receive the TcpServer.response. 
	
	    // Buffer to store the response bytes.
	    data = new Byte[256];
	
	    // String to store the response ASCII representation.
	    String responseData = String.Empty;
	
	    // Read the first batch of the TcpServer response bytes.
	    Int32 bytes = stream.Read(data, 0, data.Length);
	    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
	    Debug.Log(String.Format("Received: {0}", responseData));         
	
	    // Close everything.
	    stream.Close();         
	    client.Close();         
	  } 
	  catch (ArgumentNullException e) 
	  {
	    Debug.Log(String.Format ("ArgumentNullException: {0}", e));
	  } 
	  catch (SocketException e) 
	  {
	    Debug.Log(String.Format("SocketException: {0}", e));
	  }
	
	  //Debug.Log("\n Press Enter to continue...");
	  //Console.Read();
	}*/
	
	/*void Start()
	{
		Connect("localhost", "asdf");
	}*/
	
	
    internal Boolean socketReady = false;
 
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    String Host = "localhost";
    Int32 Port = 5000;
	
	    
    public delegate void OnMessageCallback(string _msg);
    public event OnMessageCallback OnMessage;
 
    void Start () 
	{	
    }
	
    public void Update () 
	{	
		/*string message = readSocket();
		//Debug.Log ("MESSAGE: "+ message);
		if(message != null && message.Length > 0)
		{
			Debug.Log ("ON MESSAGE IS: "+OnMessage);
			if(OnMessage != null)
			{
				Debug.Log ("OnMessage Received: "+ message);
				//OnMessage(message);
			}
		}*/
    }
	
	//Here is some code I want to be executed
	//when SomethingHappened fires.
	void HandleSomethingHappened(string foo)
	{
    	//Do some stuff
	}
	
	public void Connect()
	{
		this.Host = Host;
		this.Port = Port;
		
		setupSocket(Host, Port);
		try
		{
			var client = new TcpClient(Host, Port);
			Socket s = client.Client;
			if (!s.Connected)
			{
			   s.SetSocketOption(SocketOptionLevel.Socket,
			   SocketOptionName.ReceiveBuffer, 16384);
			   //MessageBox.Show("disconnected");
				Debug.Log ("disconnected");
			}
			else
			{
			   //MessageBox.Show("connected");
			    //s.Send(Encoding.UTF8.GetBytes("something"));
				Debug.Log ("connected");
			}
		}
		catch(Exception e)
		{
			Debug.Log ("EXCEPTION: "+e.ToString());
		}
	}
	
// **********************************************
    public void setupSocket(string host, Int32 port) {
        try {
            mySocket = new TcpClient(host, port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e) {
            Debug.Log("Socket error: " + e);
        }
    }
    public void SendData(string theLine) {
        if (!socketReady)
            return;
		
		Debug.Log ("Writing to socket stream!!!");
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }
    public String readSocket() {
		string dataRead = "";
        if (!socketReady)
            return dataRead;
        if (theStream.DataAvailable)
		{
			dataRead = theReader.ReadLine();
            return dataRead;
		}
		
		Debug.Log ("DATA READ IS: "+dataRead);
		
        return dataRead;
    }
    public void closeSocket() {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }
} // end class s_TCP