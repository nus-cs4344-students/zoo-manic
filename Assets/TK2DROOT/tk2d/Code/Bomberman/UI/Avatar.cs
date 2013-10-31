﻿using UnityEngine;
using System.Collections;

public class Avatar  
{
	private long playerID;		// the player that selected this avatar
	
	private string playerName;
	
	private int avatarID;
	
	private AvatarIcon avatarType;
	
	public int AvatarID
	{
        get { return avatarID; }
        set { avatarID = value; }
    }
	
	public string PlayerName
	{
        get { return playerName; }
        set { playerName = value; }
    }
	
	public static string ToString(int avatarID)
	{
		string result = "";
		if(avatarID == -1)
			result = "Not selected";
		else if(avatarID == 0)
			result = "Zebra";
		else if(avatarID == 1)
			result = "Rhino";
		else if(avatarID == 2)
			result = "Tiger";
		else if(avatarID == 3)
			result = "Cassowary";
		
		return result;
	}
	
	public static AvatarIcon GetAvatarIcon(int avatarID)
	{
		AvatarIcon myAvatar = AvatarIcon.NotSelected;
		if(avatarID == -1)
			myAvatar = AvatarIcon.NotSelected;
		else if(avatarID == 0)
			myAvatar = AvatarIcon.Zebra;
		else if(avatarID == 1)
			myAvatar = AvatarIcon.Rhino;
		else if(avatarID == 2)
			myAvatar = AvatarIcon.Tiger;
		else if(avatarID == 3)
			myAvatar = AvatarIcon.Cassowary;
		
		return myAvatar;
	}
	
	public AvatarIcon AvatarType
	{
        get { return avatarType; }
        set { avatarType = value; }
    }
	
	// size of the room
	public long SelectedPlayerID
	{
        get { return playerID; }
        set { playerID = value; }
    }
	
	public Avatar(long playerID, int avatarID, string playerName)
	{
		this.playerID = playerID;
		this.avatarID = avatarID;
		this.playerName = playerName;
		
		this.avatarType = GetAvatarIcon(avatarID);
	}
}