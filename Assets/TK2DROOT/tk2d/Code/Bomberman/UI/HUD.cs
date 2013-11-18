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
	
	
	[SerializeField] GameObject chatTextboxObject;
	[SerializeField] GameObject serverChatObject;
	
	[SerializeField] GUIStyle heartFilled;
	[SerializeField] GUIStyle heartEmpty;
	
	[SerializeField] GameObject victoryPanel;
	[SerializeField] GameObject losePanel;	
	
	[SerializeField] GUIStyle chatButtonStyle;
	
	private CharacterAnimController playerController;
	
	private GUIStyle bombLeftStyle;
	private GUIStyle avatarIconStyle;
	
	private int width_offset = 30;		// width of hud box
	private int height_offset = 180;		// height of hud box
	private int space = 30;
	
	private long playerID;
	private long hud_bombLeft = 3;
	
	float MessageDuration = 5.0f;
	float invulnerableDuration = 5.0f;
	float trickDuration = 10.0f;
	float rangeDuration = 10.0f;
	float shakeDuration = 10.0f;
	float hasteDuration = 10.0f;
	
	float chatBoxDuration = 5.0f;
	
	bool isChatMode = false;
	
	private SmoothCamera2D cameraScript;
	
	public bool IsChatEnabled
    {
        get { return isChatMode; }
    }
	
	int setVisibleOnce = 1;
	string chatText = "";
	
	SoundManager soundManager;
	SceneManager sceneManager;
	
	private GameObject characterChatBox;
	
	public bool IsRangeActivated()
	{
		return rangeDuration > 0.0f;
	}
	
	private int playerLives = 3;
	
	void Start()
	{
		ClearPowerup();
		soundManager = GameObject.Find ("SoundManager").GetComponent<SoundManager>();
		sceneManager = GameObject.Find ("SceneObject").GetComponent<SceneManager>();
		cameraScript = GameObject.Find ("tk2dCamera").GetComponent<SmoothCamera2D>();
		
		InitServerDisplay();
	}
	
		// Update is called once per frame
	void Update () 
	{
		MessageDuration -= Time.deltaTime;
		
		if(MessageDuration <= 0)
		{
		}
		
		invulnerableDuration -= Time.deltaTime;
		trickDuration -= Time.deltaTime;
		rangeDuration -= Time.deltaTime;
		shakeDuration -= Time.deltaTime;
		hasteDuration -= Time.deltaTime;
		
		chatBoxDuration -= Time.deltaTime;

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
		
		if(chatBoxDuration <= 0)
			ToggleChatDisplay(false);
	}
	
	public void HudToogleChatDisplay(GameObject characterInstance, string chatMsg)
	{
		characterChatBox = characterInstance.transform.Find("Chat").gameObject;	
		characterChatBox.SetActive(true);
		tk2dTextMesh chatScript = characterInstance.transform.Find("Chat/ChatText").GetComponent<tk2dTextMesh>();
			
		if(chatScript != null)
		{
			Debug.Log ("Server gives chat messageis: "+chatMsg);
			chatBoxDuration = 5.0f;
			chatScript.text = chatMsg;
		}
	}
	
	void ToggleChatDisplay(bool isVisible)
	{
		if(characterChatBox != null)
		{
			characterChatBox.SetActive(isVisible);
		}
	}
	
	void InitServerDisplay()
	{
		if(serverChatObject)
		{
			tk2dUITextInput serverDisplayScript = serverChatObject.GetComponent<tk2dUITextInput>();
			serverDisplayScript.Text = "Server message: Game Started.\nEnjoy!";
		}
	}
	
	public void UpdateHealthStatus(int playerLives)
	{
		this.playerLives = playerLives;
	}
	
	void InitSoundButton()
	{
		if(chatButtonStyle != null)
		{
			if(GUI.Button(new Rect (0.514210525f * Screen.width, 0.821978022f * Screen.height, 0.067368421f * Screen.width, 0.14065934f * Screen.height), "", chatButtonStyle))
				ToogleChatDisplay();
		}
	}
	
	void ToogleChatDisplay()
	{
		if(isChatMode)
		{
			tk2dUITextInput textboxScript = chatTextboxObject.GetComponent<tk2dUITextInput>();
			string userInput = textboxScript.Text;
			
			// Send chat to server
			sceneManager.SendChatToServer(userInput);
		
			textboxScript.Text = "";
		
			chatTextboxObject.SetActive(false);
			setVisibleOnce = 0;
			isChatMode = false;
		}
		else
		{
			chatTextboxObject.SetActive(true);
			tk2dUITextInput textboxScript = chatTextboxObject.GetComponent<tk2dUITextInput>();
			textboxScript.SetFocus();
			isChatMode = true;
		}
	}
	
	void InitChatText()
	{
		// When user presses enter, clear the chat box
     	if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return) 
		{
			ToogleChatDisplay();
		}
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

	void OnGUI() {
		//Rect params left, top, width, height
		GUI.Box(new Rect (0.013157894f * Screen.width, 0.802197802f * Screen.height, 0.95f * Screen.width, 0.197802197f * Screen.height), "", hudboxStyle);
		
		
		InitDirectionalButtons();
		
		InitAvatarPanel();
		
		InitBombLeft();
		
		InitHUDIcon();
		
		InitPlantBombButton();
		
		InitHealthDisplay();
		
		InitSoundButton();
		
		InitChatText();
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
				playerController.BombLimit = 6;
			}
			
			trickDuration = 10.0f;
			trickPowerup.normal.background = textureTrick;
		}
		else
			trickPowerup.normal.background = alphaTrick;
	}
	
	void InitHealthDisplay()
	{
		if(playerLives == 3)
		{
			GUI.Box(new Rect (0.315789473f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartFilled);
			GUI.Box(new Rect (0.357894736f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartFilled);
			GUI.Box(new Rect (0.4f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartFilled);
		}
		else if(playerLives == 2)
		{
			GUI.Box(new Rect (0.315789473f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartFilled);
			GUI.Box(new Rect (0.357894736f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartFilled);
			GUI.Box(new Rect (0.4f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartEmpty);
		}
		else if(playerLives == 1)
		{
			GUI.Box(new Rect (0.315789473f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartFilled);
			GUI.Box(new Rect (0.357894736f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartEmpty);
			GUI.Box(new Rect (0.4f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartEmpty);
		}
		else if(playerLives < 1)
		{
			GUI.Box(new Rect (0.315789473f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartEmpty);
			GUI.Box(new Rect (0.357894736f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartEmpty);
			GUI.Box(new Rect (0.4f * Screen.width, 0.835164835f * Screen.height, 0.029473684f * Screen.width, 0.054945054f * Screen.height), "", heartEmpty);
		}
	}
	
	void InitPlantBombButton()
	{
		if(GUI.Button(new Rect (0.87f * Screen.width, 0.824175824f * Screen.height, 0.071578947f * Screen.width, 0.153846153f * Screen.height), "", plantBombStyle))
		{
			if(playerController)
				playerController.SendPlantBombMessage();
		}
	}
	
	public void DisplayKillMessage(string killMessage)
	{
		Debug.Log ("KILL MESSAGE IS: "+killMessage);
		if(serverChatObject)
		{
			tk2dUITextInput serverDisplayScript = serverChatObject.GetComponent<tk2dUITextInput>();
			serverDisplayScript.Text = killMessage;
		}
	}

	void InitHUDIcon()
	{
		// 577 823
		GUI.Box(new Rect (0.30368421f * Screen.width, 0.904395604f * Screen.height, 0.021052631f * Screen.width, 0.065934065f * Screen.height), "", invulnerablePowerup);
		GUI.Box(new Rect (0.33f * Screen.width, 0.904395604f * Screen.height, 0.021052631f * Screen.width, 0.065934065f * Screen.height), "", rangePowerup);
		GUI.Box(new Rect (0.361578947f * Screen.width, 0.904395604f * Screen.height, 0.021052631f * Screen.width, 0.065934065f * Screen.height), "", shakePowerup);
		GUI.Box(new Rect (0.393157894f * Screen.width, 0.904395604f * Screen.height, 0.021052631f * Screen.width, 0.065934065f * Screen.height), "", speedPowerup);
		GUI.Box(new Rect (0.419473684f * Screen.width, 0.904395604f * Screen.height, 0.021052631f * Screen.width, 0.065934065f * Screen.height), "", trickPowerup);
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
			
			GUI.Box(new Rect (0.67f * Screen.width, 0.821978022f * Screen.height, 0.054736842f * Screen.width, 0.12857142f * Screen.height), "", bombLeftStyle);
			GUI.Label(new Rect (0.72f * Screen.width, 0.887912087f * Screen.height, 0.054736842f * Screen.width, 0.142857142f * Screen.height), ""+hud_bombLeft, labelStyle);
		}
		//GUI.Label(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2), 104, 130), "3", rhinoBomb);
		//GUI.Label(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2), 104, 130), "3", tigerBomb);
		//GUI.Label(new Rect (25 + space*3 + 131 + 131, (Screen.height - height_offset + 47/2), 104, 130), "3", birdBomb);
	}
	
	void InitAvatarPanel()
	{
		GUI.Box(new Rect (0.157894736f * Screen.width, 0.827472527f * Screen.height, 0.078947368f * Screen.width, 0.146153846f * Screen.height), "", avatarBox);
		
		if(avatarIconStyle != null)
			GUI.Box(new Rect (0.164210526f * Screen.width, 0.85f * Screen.height, 0.064210526f * Screen.width, 0.095604395f * Screen.height), "", avatarIconStyle);
	}
	
	public void DisplayVictoryPanel(string winnerName)
	{
		//GUI.Box(new Rect (0.5f * Screen.width, 0.5f * Screen.height, 0.318947368f * Screen.width, 0.219780219f * Screen.height), "", victoryPanelStyle);
	
		victoryPanel.SetActive(true);
		
		// Instantiate name
		tk2dTextMesh lobbyScript = victoryPanel.transform.Find("LobbyText").GetComponent<tk2dTextMesh>();
		tk2dTextMesh winnerTextScript = victoryPanel.transform.Find("NameText").GetComponent<tk2dTextMesh>();
		
		winnerTextScript.text = "Congratulations, You WIN!";
		
		StartCoroutine( GoBacktoLobby(lobbyScript) );
	}
	
	public void DisplayDefeatPanel(string winnerName)
	{
		//GUI.Box(new Rect (0.5f * Screen.width, 0.5f * Screen.height, 0.318947368f * Screen.width, 0.219780219f * Screen.height), "", losePanelStyle);
		losePanel.SetActive(true);
		
		tk2dTextMesh lobbyScript = losePanel.transform.Find("LobbyText").GetComponent<tk2dTextMesh>();
		tk2dTextMesh winnerTextScript = losePanel.transform.Find("NameText").GetComponent<tk2dTextMesh>();
		
		winnerTextScript.text = "Winner is: "+winnerName;
		
		StartCoroutine( GoBacktoLobby(lobbyScript) );
	}
	
	void InitDirectionalButtons()
	{
		GUI.Box(new Rect (0.028947368f * Screen.width, 0.78f * Screen.height, 0.068947368f * 1.5f * Screen.width, 0.146153846f * 1.5f * Screen.height), "", directionalPadStyle);
		
		// Button Up
		//if (GUI.Button( new Rect(25 + space + 45, (Screen.height - height_offset + 47/2) + 10, 38, 25), "", button_direction_up))
		if (GUI.RepeatButton( new Rect(0.062f * Screen.width, 0.78f * Screen.height, 0.02f * 2f * Screen.width, 0.027472527f * 2f * Screen.height), "", button_direction_up))
		{
			/*Debug.Log ("SCREEN.WIDTH 1900"+ Screen.width );
			Debug.Log ("SCREEN.HEIGHT 910"+ Screen.height );
			Debug.Log ("X: "+ (25 + space*3 + 131 + 131 + 200) );
			Debug.Log ("Y: "+ ( (Screen.height - height_offset) + 20) );*/
			
			soundManager.PlayHUDClickSound(transform.position);			
			if(playerController)
				playerController.MoveUp(0.0f, 0.0f, true);
			else if(cameraScript)
				cameraScript.MoveUp();
		}
		
		//Button Left
		//if (GUI.Button( new Rect(25 + space + 12, (Screen.height - height_offset + 47/2) + 45, 26, 40), "", button_direction_left))
		if (GUI.RepeatButton( new Rect(0.035f * Screen.width, 0.845f * Screen.height, 0.01368421f * 2f * Screen.width, 0.043956043f * 2f * Screen.height), "", button_direction_left))
		{
			soundManager.PlayHUDClickSound(transform.position);
			if(playerController)
				playerController.MoveLeft(0.0f, 0.0f, true);
			else if(cameraScript)
				cameraScript.MoveLeft();
		}
		
		//Button Down
		//if (GUI.Button( new Rect(25 + space + 45, (Screen.height - height_offset + 47/2) + 100, 38, 25), "", button_direction_down))
		if (GUI.RepeatButton( new Rect(0.062f * Screen.width, 0.937362637f * Screen.height, 0.02f * 2f * Screen.width, 0.027472527f * 2f * Screen.height), "", button_direction_down))
		{			
			soundManager.PlayHUDClickSound(transform.position);			
			if(playerController)
				playerController.MoveDown(0.0f, 0.0f, true);
			else if(cameraScript)
				cameraScript.MoveDown();
		}
		
		//Button Right
		//if (GUI.Button( new Rect(25 + space + 100, (Screen.height - height_offset + 47/2) + 45, 24, 40), "", button_direction_right))
		if (GUI.RepeatButton( new Rect(0.105f * Screen.width, 0.845f * Screen.height, 0.012631578f * 2f * Screen.width, 0.043956043f * 2f * Screen.height), "", button_direction_right))
		{			
			soundManager.PlayHUDClickSound(transform.position);			
			if(playerController)
				playerController.MoveRight(0.0f, 0.0f, true);
			else if(cameraScript)
				cameraScript.MoveRight();
		
		}
	}
	
	
		// back to lobby
	IEnumerator GoBacktoLobby(tk2dTextMesh textScript)
	{
		textScript.text = "Returning back to lobby in 5 seconds";
		yield return new WaitForSeconds(1.0f);
		
		
		textScript.text = "Returning back to lobby in 4 seconds";
		yield return new WaitForSeconds(1.0f);
		
		textScript.text = "Returning back to lobby in 3 seconds";
		yield return new WaitForSeconds(1.0f);
		
		textScript.text = "Returning back to lobby in 2 seconds";
		yield return new WaitForSeconds(1.0f);
		
		textScript.text = "Returning back to lobby in 1 seconds";
		yield return new WaitForSeconds(1.0f);
		
		GameManager.LoadLobbyScene();
	}
}
