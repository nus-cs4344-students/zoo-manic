using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
 
public class TCPSocket : MonoBehaviour {
    internal Boolean socketReady = false;
 
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    String Host = "localhost";
    Int32 Port = 5000;
 
    void Start () 
	{
		//setupSocket();
		
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
		    s.Send(Encoding.UTF8.GetBytes("something"));
			Debug.Log ("connected");
		}
		
    }
	
    void Update () 
	{	
    }
	
// **********************************************
    public void setupSocket() {
        try {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
			
			Debug.Log ("UNITY SOCKET CONNECTED!!");
			
			writeSocket("HELLO WORLD!!!");
        }
        catch (Exception e) {
            Debug.Log("Socket error: " + e);
        }
    }
    public void writeSocket(string theLine) {
        if (!socketReady)
            return;
		
		Debug.Log ("Writing to socket stream!!!");
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }
    public String readSocket() {
        if (!socketReady)
            return "";
        if (theStream.DataAvailable)
            return theReader.ReadLine();
        return "";
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