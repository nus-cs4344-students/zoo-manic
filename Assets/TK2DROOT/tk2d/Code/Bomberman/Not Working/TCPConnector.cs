// This is the client DLL class code to use for the sockServer
// include this DLL in your Plugins folder under Assets
// using it is very simple
// Look at LinkSyncSCR.cs


using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using UnityEngine;
using MiniJSON;
using System.Collections.Generic;
using System.Threading;

public class TCPConnector : MonoBehaviour
{
	const int READ_BUFFER_SIZE = 100000;
	const Int32 PORT_NUM = 10000;
	private TcpClient client;
	//var client;
	private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
	public ArrayList lstUsers=new ArrayList();
	public string strMessage=string.Empty;
	public string res=String.Empty;
	private string pUserName;
	
	public delegate string AsyncDelegate();
	
	public List<string> m_messageQueue = new List<string>();
	
    public delegate void OnMessageCallback(string _msg);
    public event OnMessageCallback OnMessage;
	
	AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
	
	private ClientSocket clientConnection;

	public TCPConnector(){}

	public string fnConnectResult(string netIP, Int32 portNum)
	{
		try 
		{
			//setupSocket(sNetIP, iPORT_NUM);
			//pUserName=sUserName;
			// The TcpClient is a subclass of Socket, providing higher level 
			// functionality like streaming.
			client = new TcpClient(netIP, portNum);
			// Start an asynchronous read invoking DoRead to avoid lagging the user
			// interface.
			client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, DoRead, null);
			// Make sure the window is showing before popping up connection dialog.
			
			clientConnection = GameObject.Find ("PlayerConnection").GetComponent<ClientSocket>();
			
			//AttemptLogin(sUserName);
			return "Connection Succeeded";
		} 
		catch(Exception ex)
		{
			return "Server is not active.  Please start server and try again.      " + ex.ToString();
		}
	}
	
	public void AttemptLogin(string user)
	{
		SendData("CONNECT|"+ user);
	}

	public void fnPacketTest(string sInfo)
	{
		SendData("CHAT|" + sInfo);
	}

	public void fnDisconnect()
	{
		SendData("DISCONNECT");
	}

	public void fnListUsers()
	{
		SendData("REQUESTUSERS");
	}
	
	void Update()
	{
	}
	
	/*public void SomeFunction()
    {    
        StopOperation();
        stopWaitHandle.WaitOne(); // wait for callback    
        StartOperation();
    }*/
	
	private void StartOperation(int BytesRead)
    {
		strMessage = Encoding.ASCII.GetString(readBuffer, 0, BytesRead-2);
		//strMessage = this.Encoding.GetString(readBuffer, 0, BytesRead-2);
		ProcessCommands(strMessage);
		
		Thread.Sleep(500);
		
		// do something
		client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, DoRead, null);
		
		//Thread.Sleep(500);
    }
    private int StopOperation(IAsyncResult ar)
    {
        // This task simulates an asynchronous call that will invoke
        // Stop_Callback upon completion. In real code you will probably
        // have something like this instead:
        //
        //     someObject.DoSomethingAsync("input", Stop_Callback);
        //
        
		/*new Task(() =>
            {
                Thread.Sleep(500);
                Stop_Callback(); // invoke the callback
            }).Start();*/
		
		return ReadData(ar);
    }
	
	private int ReadData(IAsyncResult ar)
	{
		int BytesRead = -1;
		try
		{
			// Finish asynchronous read into readBuffer and return number of bytes read.
			BytesRead = client.GetStream().EndRead(ar);
		
			if (BytesRead < 1) 
			{
				// if no bytes were read server has close.  
				res="Disconnected";
				return BytesRead;
			}
			// Convert the byte array the message was saved into, minus two for the
			// Chr(13) and Chr(10)
			
			//client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, DoRead, null);
		} 
		catch
		{
	
			res="Disconnected";
		}
		
		Stop_Callback(); // invoke the callback
		
		return BytesRead;
	}

    private void Stop_Callback()
    {
        // signal the wait handle
        stopWaitHandle.Set();
    }


	private void DoRead(IAsyncResult ar)
	{ 
		/*int BytesRead;
		try
		{
			// Finish asynchronous read into readBuffer and return number of bytes read.
			BytesRead = client.GetStream().EndRead(ar);
		
			if (BytesRead < 1) 
			{
				// if no bytes were read server has close.  
				res="Disconnected";
				return;
			}
			// Convert the byte array the message was saved into, minus two for the
			// Chr(13) and Chr(10)
			strMessage = Encoding.ASCII.GetString(readBuffer, 0, BytesRead-2);
			//strMessage = this.Encoding.GetString(readBuffer, 0, BytesRead-2);
			ProcessCommands(strMessage);
			
			// Start a new asynchronous read into readBuffer.
			client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
			
			//client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, DoRead, null);
		} 
		catch
		{
	
			res="Disconnected";
		}
		
		//Thread.Sleep(2000);*/

		int bytesRead = StopOperation(ar);
        stopWaitHandle.WaitOne(); // wait for callback    
        StartOperation(bytesRead);
	}
	
	/*public static void ExecuteSync(AsyncDelegate func)
    {
        ManualResetEvent reset = new ManualResetEvent(false);
        int threadId;
        func.BeginInvoke((a)=>reset.Set(), null);
        reset.WaitOne();
    }
	
	public static void Execute()
	{
		ManualResetEvent reset = new ManualResetEvent(false);
		this.AsyncActionDone += (sender, args) => reset.Set();
		this.PerformAsyncAction();
		reset.WaitOne();
	}
	

	public object Operation(object arg)
	{
	    var ar = BeginOperation(arg, DoRead, null);
	
	    return EndOperation(ar);
	}
	
	public IAsyncResult BeginOperation(object arg, AsyncCallback asyncCallback, object state)
	{
	    AsyncResult asyncResult = new AsyncResult(asyncCallback, state);
	
	    // Lauch the asynchronous operation
	    return asyncResult;
	}
	
	private void LaunchOperation(AsyncResult asyncResult)
	{
	    // Do something asynchronously and call OnOperationFinished when finished
		int BytesRead;
		try
		{
			// Finish asynchronous read into readBuffer and return number of bytes read.
			BytesRead = client.GetStream().EndRead(ar);
			if (BytesRead < 1) 
			{
				// if no bytes were read server has close.  
				res="Disconnected";
				return;
			}
			// Convert the byte array the message was saved into, minus two for the
			// Chr(13) and Chr(10)
			strMessage = Encoding.ASCII.GetString(readBuffer, 0, BytesRead-2);
			ProcessCommands(strMessage);
		} 
		catch
		{
			res="Disconnected";
		}
	}
	
	private void OnOperationFinished(AsyncResult asyncResult, object result)
	{
	    asyncResult.Complete(result);
	}
	
	
	public object EndOperation(IAsyncResult asyncResult)
	{
	    AsyncResult ar = (AsyncResult)asyncResult;
	
	    return ar.EndInvoke();
	}*/

	// Process the command received from the server, and take appropriate action.
	private void ProcessCommands(string strMessage)
	{
		/*string[] dataArray;

		// Message parts are divided by "|"  Break the string into an array accordingly.
		dataArray = strMessage.Split((char) 124);
		// dataArray(0) is the command.
		switch( dataArray[0])
		{
			case "JOIN":
				// Server acknowledged login.
				res= "You have joined the chat";
				break;
			case "CHAT":
				// Received chat message, display it.
				res=  dataArray[1].ToString();
				break;
			case "REFUSE":
				// Server refused login with this user name, try to log in with another.
				AttemptLogin(pUserName);
				res=  "Attempted Re-Login";
				break;
			case "LISTUSERS":
				// Server sent a list of users.
				ListUsers(dataArray);
				break;
			case "BROAD":
				// Server sent a broadcast message
				res=  "ServerMessage: " + dataArray[1].ToString();
				break;
		}*/
		
		//Thread.Sleep(1000);
		
		//StartCoroutine(SendDataToClient());	
		if(strMessage != null && OnMessage != null)
		{
			lock (ClientSocket.Lock) 
			{
				ClientSocket.threadMsg = strMessage;
				ClientSocket.isMsgUpdated = true;
				
				m_messageQueue.Add (ClientSocket.threadMsg);
			}
			
			//OnMessage(strMessage);
		}
	}
	
	/*IEnumerator SendDataToClient()
	{
     	yield return new WaitForSeconds(0.1f);
		
		if(strMessage != null && OnMessage != null)
		{
			lock (clientConnection.Lock) 
			{
				clientConnection.threadMsg = strMessage;
				clientConnection.isMsgUpdated = true;
				
				m_messageQueue.Add (strMessage);
			}

			//threadMessage = strMessage;
			//OnMessage( strMessage );
			
			//OnMessage(strMessage);
		}
	}*/

	// Use a StreamWriter to send a message to server.
	public void SendData(string data)
	{
		StreamWriter writer = new StreamWriter(client.GetStream());
		writer.Write(data + (char) 13);
		writer.Flush();
	}

	private void ListUsers(string[] users)
	{
		int I;
		lstUsers.Clear();
		for (I = 1; I <= (users.Length - 1); I++)
		{
			lstUsers.Add(users[I]);	
		}
	}
}
