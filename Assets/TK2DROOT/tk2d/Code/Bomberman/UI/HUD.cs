using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	
	// Draws 2 buttons, one with an image, and other with a text
	// And print a message when they got clicked.
	//[SerializeField] Texture btnTexture;
	[SerializeField] GUIStyle hudboxStyle;
	
	// Keypads
	[SerializeField] GUIStyle directionalPadStyle;
	[SerializeField] GUIStyle button_direction_up;
	[SerializeField] GUIStyle button_direction_down;
	[SerializeField] GUIStyle button_direction_left;
	[SerializeField] GUIStyle button_direction_right;
	
	// Avatars
	[SerializeField] GUIStyle avatarBox;
	[SerializeField] GUIStyle avatarZebra;
	[SerializeField] GUIStyle avatarRhino;
	[SerializeField] GUIStyle avatarBird;
	[SerializeField] GUIStyle avatarTiger;
	
	// Bomb Left Panel
	[SerializeField] GUIStyle zebraBomb;
	[SerializeField] GUIStyle tigerBomb;
	[SerializeField] GUIStyle rhinoBomb;
	[SerializeField] GUIStyle birdBomb;
	
	// Plant bomb label style
	[SerializeField] GUIStyle labelStyle;
	
	// Kill and Game End Message
	[SerializeField] GUIStyle killMsgLabelStyle;
	[SerializeField] GUIStyle gameEndLabelStyle;
	
	// Powerup Panel
	[SerializeField] GUIStyle invulnerablePowerup;
	[SerializeField] GUIStyle rangePowerup;
	[SerializeField] GUIStyle shakePowerup;
	[SerializeField] GUIStyle speedPowerup;
	[SerializeField] GUIStyle trickPowerup;
	
	[SerializeField] Texture2D textureInvulnerable;
	[SerializeField] Texture2D textureRange;
	[SerializeField] Texture2D textureShake;
	[SerializeField] Texture2D textureSpeed;
	[SerializeField] Texture2D textureTrick;
	
	[SerializeField] Texture2D alphaInvulnerable;
	[SerializeField] Texture2D alphaRange;
	[SerializeField] Texture2D alphaShake;
	[SerializeField] Texture2D alphaSpeed;
	[SerializeField] Texture2D alphaTrick;
	
	// Button Plant Bomb
	[SerializeField] GUIStyle plantBombStyle;
	[SerializeField] Texture2D plantZebra;
	[SerializeField] Texture2D plantRhino;
	[SerializeField] Texture2D plantBird;
	[SerializeField] Texture2D plantTiger;
	
	private CharacterAnimController playerController;
	
	private GUIStyle bombLeftStyle;
	private GUIStyle avatarIconStyle;
	
	private int width_offset = 30;		// width of hud box
	private int height_offset = 180;		// height of hud box
	private int space = 30;
	
	private long playerID;
	private long hud_bombLeft = 3;
	
	private string currentKillMessage = "";
	private string gameEndMessage = "";
	
	float MessageDuration = 3.0f;
	float invulnerableDuration = 10.0f;
	float trickDuration = 10.0f;
	float rangeDuration = 10.0f;
	float shakeDuration = 10.0f;
	float hasteDuration = 10.0f;
	
	SoundManager soundManager;
	
	public bool IsRangeActivated()
	{
		return rangeDuration > 0.0f;
	}
	
	void Start()
	{
		/*SetPlayerAvatarHUD(AvatarIcon.Rhino);
		
		ToggleInvulnerable(false);
		ToggleTrick(false);
		ToggleRange(false);
		ToggleShake(false);
		ToggleSpeed(false);*/
		
		ClearPowerup();
		soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
	}
	
		// Update is called once per frame
	void Update () 
	{
		MessageDuration -= Time.deltaTime;
		
		if(MessageDuration <= 0)
		{
			MessageDuration = 3.0f;
			currentKillMessage = "";
		}
		
		invulnerableDuration -= Time.deltaTime;
		trickDuration -= Time.deltaTime;
		rangeDuration -= Time.deltaTime;
		shakeDuration -= Time.deltaTime;
		hasteDuration -= Time.deltaTime;

		if(invulnerableDuration <= 0)
			ToggleInvulnerable(false);
		
		if(trickDuration <= 0)
			ToggleTrick(false);
		
		if(rangeDuration <= 0)
			ToggleRange(false);
		
		if(shakeDuration <= 0)
			ToggleShake(false);
		
		if(hasteDuration <= 0)
			ToggleHaste(false);	
	}
	
	public void ClearPowerup()
	{
		ToggleInvulnerable(false);
		ToggleTrick(false);
		ToggleRange(false);
		ToggleShake(false);
		ToggleHaste(false);
	}
	
	public void SetPlayerID(long serverPlayerID)
	{
		playerID = serverPlayerID;
		
		GameObject playerObj = GameObject.Find(""+playerID);
		if(playerObj)
		{
			playerController = playerObj.GetComponent<CharacterAnimController>();
		}
	}
	
	public void ShowKillMessage(string message)
	{
		currentKillMessage = message;
	}
	
	public void ShowGameEndMessage(string message)
	{
		gameEndMessage = message;
	}
	
	
	void OnGUI() {
		/*if (!btnTexture) {
			Debug.LogError("Please assign a texture on the inspector");
			return;
		}
		if (GUI.Button(new Rect(0,0,50,50),btnTexture))
			Debug.Log("Clicked the button with an image");
		if (GUI.Button(new Rect(10,70,50,30),"Click"))
			Debug.Log("Clicked the button with text");*/
		
		// Provide the name of the Style as the final argument to use it
		//GUILayout.Button ("HUDDock", customGuiStyle);
		
		//Rect params left, top, width, height
		GUI.Box(new Rect (25, (Screen.height - height_offset), 1400, 180), "", hudboxStyle);
		
		
		InitDirectionalButtons();
		
		InitAvatarPanel();
		
		InitBombLeft();
		
		InitHUDIcon();
		
		InitPlantBombButton();
		
		InitHUDMessage();
		
		InitGameEndMessage();
	}
	
	public void SetPlayerAvatarHUD(AvatarIcon type)
	{
		switch(type)
		{
			case AvatarIcon.Rhino:
			bombLeftStyle = rhinoBomb;
			avatarIconStyle = avatarRhino;
			plantBombStyle.normal.background = plantRhino;
			break;
			
			case AvatarIcon.Zebra:
			bombLeftStyle = zebraBomb;
			avatarIconStyle = avatarZebra;
			plantBombStyle.normal.background = plantZebra;
			break;
			
			case AvatarIcon.Tiger:
			bombLeftStyle = tigerBomb;
			avatarIconStyle = avatarTiger;
			plantBombStyle.normal.background = plantTiger;
			break;
			
			case AvatarIcon.Cassowary:
			bombLeftStyle = birdBomb;
			avatarIconStyle = avatarBird;
			plantBombStyle.normal.background = plantBird;
			break;
		}
	}
	
	public void ToggleInvulnerable(bool isShow)
	{
		if(isShow)
		{
			invulnerableDuration = 10.0f;
			invulnerablePowerup.normal.background = textureInvulnerable;
		}
		else
			invulnerablePowerup.normal.background = alphaInvulnerable;
	}
	
	public void ToggleHaste(bool isShow)
	{
		if(isShow)
		{
			hasteDuration = 10.0f;
			speedPowerup.normal.background = textureSpeed;
		}
		else
			speedPowerup.normal.background = alphaSpeed;
	}
	
	public void ToggleShake(bool isShow)
	{
		if(isShow)
		{
			shakeDuration = 10.0f;
			shakePowerup.normal.background = textureShake;
		}
		else
			shakePowerup.normal.background = alphaShake;
	}
	
	public void ToggleRange(bool isShow)
	{
		if(isShow)
		{
			rangeDuration = 10.0f;
			rangePowerup.normal.background = textureRange;
		}
		else
			rangePowerup.normal.background = alphaRange;
	}
	
	public void ToggleTrick(bool isShow)
	{
		if(isShow)
		{
			hud_bombLeft = 6;
			GameObject playerGO = GameObject.FindWithTag("Player");
			// e.g. player still alive
			if(playerGO != null)
			{
				var playerController = playerGO.GetComponent<CharacterAnimController>();
				Debug.Log ("UPDATING BOMB LIMIT TO 6");
				playerController.BombLimit = 6;
			}
			
			trickDuration = 10.0f;
			trickPowerup.normal.background = textureTrick;
		}
		else
			trickPowerup.normal.background = alphaTrick;
	}
	
	void InitPlantBombButton()
	{
		if(GUI.Button(new Rect (Screen.width - 60 - 140, (Screen.height - height_offset) + 20, 136, 140), "", plantBombStyle))
		{
			if(playerController)
				playerController.SendPlantBombMessage();
		}
	}
	
	void InitGameEndMessage()
	{
		GUI.Label(new Rect (Screen.width/2, Screen.height / 2, 200, 60), gameEndMessage, gameEndLabelStyle);
	}
	
	void InitHUDMessage()
	{
		GUI.Label(new Rect (25 + space*3 + 131 + 131 + 200, (Screen.height - height_offset + 47/2) + 100, 40, 60), currentKillMessage, killMsgLabelStyle);
	}

	void InitHUDIcon()
	{
		GUI.Box(new Rect (25 + space*3 + 131 + 131 + 200, (Screen.height - height_offset + 47/2) + 10, 40, 60), "", invulnerablePowerup);
		GUI.Box(new Rect (25 + space*3 + 131 + 131 + 200 + 50, (Screen.height - height_offset + 47/2) + 10, 40, 60), "", rangePowerup);
		GUI.Box(new Rect (25 + space*3 + 131 + 131 + 200 + 110, (Screen.height - height_offset + 47/2) + 10, 40, 60), "", shakePowerup);
		GUI.Box(new Rect (25 + space*3 + 131 + 131 + 200 + 170, (Screen.height - height_offset + 47/2) + 10, 40, 60), "", speedPowerup);
		GUI.Box(new Rect (25 + space*3 + 131 + 131 + 200 + 220, (Screen.height - height_offset + 47/2) + 10, 40, 60), "", trickPowerup);
	}
	
	// Invoked by the bomb class when it explode
	public void SetBombLeft(int bombLeft)
	{
		hud_bombLeft = bombLeft;
	}
	
	void InitBombLeft()
	{
		//GUI.Label (new Rect (0,0,100,50), "bombleft", labelStyle);
		// Zebra
		if(bombLeftStyle != null)
		{
			//GameObject playerObj = GameObject.Find(GameManager.PlayerID+"");
			//if(playerObj)
			//	hud_bombLeft = playerObj.GetComponent<CharacterAnimController>();
			
			GUI.Box(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2) - 5, 104, 130), "", bombLeftStyle);
			GUI.Label(new Rect (25 + space*3 + 131 + 131 + 104, (Screen.height - height_offset + 47/2) + 55, 104, 130), ""+hud_bombLeft, labelStyle);
		}
		//GUI.Label(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2), 104, 130), "3", rhinoBomb);
		//GUI.Label(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2), 104, 130), "3", tigerBomb);
		//GUI.Label(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2), 104, 130), "3", birdBomb);
	}
	
	void InitAvatarPanel()
	{
		GUI.Box(new Rect (25 + space*2 + 131, (Screen.height - height_offset + 47/2), 131, 133), "", avatarBox);
		
		if(avatarIconStyle != null)
			GUI.Box(new Rect (25 + space*2 + 145, (Screen.height - height_offset + 47/2) + 7, 67, 108), "", avatarIconStyle);
		
		//GUI.Box(new Rect (25 + space*2 + 145, (Screen.height - height_offset + 47/2) + 7, 67, 108), "", avatarZebra);
		//GUI.Box(new Rect (25 + space*2 + 145, (Screen.height - height_offset + 47/2) + 7, 67, 108), "", avatarRhino);
		//GUI.Box(new Rect (25 + space*2 + 145, (Screen.height - height_offset + 47/2) + 7, 67, 108), "", avatarBird);
		//GUI.Box(new Rect (25 + space*2 + 140, (Screen.height - height_offset + 47/2) + 5, 67, 108), "", avatarTiger);
	}
	
	void InitDirectionalButtons()
	{
		GUI.Box(new Rect (25 + space * 1, (Screen.height - height_offset + 47/2), 131, 133), "", directionalPadStyle);
		
		// Button Up
		if (GUI.Button( new Rect(25 + space + 45, (Screen.height - height_offset + 47/2) + 10, 38, 25), "", button_direction_up))
		{
			soundManager.PlayHUDClickSound(transform.position);			
			if(playerController)
				playerController.MoveUp(true);
		}
		
		//Button Left
		if (GUI.Button( new Rect(25 + space + 12, (Screen.height - height_offset + 47/2) + 45, 26, 40), "", button_direction_left))
		{
			soundManager.PlayHUDClickSound(transform.position);
			if(playerController)
				playerController.MoveLeft(true);
		}
		
		//Button Down
		if (GUI.Button( new Rect(25 + space + 45, (Screen.height - height_offset + 47/2) + 100, 38, 25), "", button_direction_down))
		{
			soundManager.PlayHUDClickSound(transform.position);			
			if(playerController)
				playerController.MoveDown(true);
		}
		
		//Button Right
		if (GUI.Button( new Rect(25 + space + 100, (Screen.height - height_offset + 47/2) + 45, 24, 40), "", button_direction_right))
		{
			soundManager.PlayHUDClickSound(transform.position);			
			if(playerController)
				playerController.MoveRight(true);
		}
	}
	

}
