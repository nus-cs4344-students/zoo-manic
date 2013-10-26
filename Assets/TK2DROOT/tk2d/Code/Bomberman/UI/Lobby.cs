using UnityEngine;
using System.Collections;

public class Lobby  
{
	// ID of the room
	private int sessionID;
	
	private int numOfPlayers;
	
	private bool isPlaying;
	
	
	public int SessionID
	{
        get { return sessionID; }
        set { sessionID = value; }
    }
	
	// size of the room
	public int NumofPlayers
	{
        get { return numOfPlayers; }
        set { numOfPlayers = value; }
    }
	
	public bool HasStarted
	{
        get { return isPlaying; }
        set { isPlaying = value; }
    }
	
	public Lobby(int sessionID, int numPlayers, bool isPlaying)
	{
		this.sessionID = sessionID;
		this.numOfPlayers = numPlayers;
		this.isPlaying = false;
	}
}
