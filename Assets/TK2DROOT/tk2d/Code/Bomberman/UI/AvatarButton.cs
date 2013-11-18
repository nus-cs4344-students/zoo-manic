using UnityEngine;
using System.Collections;

public enum AvatarIcon {Zebra, Rhino, Tiger, Cassowary, NotSelected}


// Zebra 0, Rhino 1, Tiger 2, Cassowary 3, 
public class AvatarButton : MonoBehaviour {
	
	
	[SerializeField] tk2dUIItem uiItem;
	[SerializeField] GameObject clickedObject;
	[SerializeField] AvatarIcon avatarIcon;
	
	private bool isAvailable = true;

	// Button class calls scene manager, and scene manager calls serverconnection/gamemanager
	private SceneManager sceneManager;
	
	// Use this for initialization
	void Start () 
	{
		sceneManager = GameObject.Find ("SceneObject").GetComponent<SceneManager>();
		
		// If button is initialised
		if(avatarIcon == AvatarIcon.Zebra)
		{
			/*int selectedNumber = 1;
			clickedObject.transform.Find ("ListSelected").gameObject.SetActive(true);
			clickedObject.GetComponent<tk2dUIHoverItem>().enabled = false;
			sceneManager.UpdateSelectedRoom(selectedNumber);*/
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	// The availability of the avatar
	public bool IsAvailable
	{
        get { return isAvailable; }
        set { isAvailable = value; }
    }
	
	void OnEnable()
	{
	    uiItem.OnDown += AvatarDown;
	    uiItem.OnClickUIItem += Clicked;
	}
	
	void AvatarDown()
	{
		int selectedAvatar = 0;
		switch(avatarIcon)
		{
			case AvatarIcon.Zebra:
			selectedAvatar = 0;
			break;
			
			case AvatarIcon.Rhino:
			selectedAvatar = 1;
			break;
			
			case AvatarIcon.Tiger:
			selectedAvatar = 2;
			break;
			
			case AvatarIcon.Cassowary:
			selectedAvatar = 3;
			break;
		}
		
		if(sceneManager.isPlayerReady == false)
		{
			sceneManager.ClearAvatarSelection();
			gameObject.transform.Find ("SelectedBG").gameObject.SetActive(true);
			sceneManager.UpdateSelectedAvatar(selectedAvatar);
		}
	}

	
	void Clicked(tk2dUIItem clickedUIItem)
	{
	    //Debug.Log("Clicked:" + clickedUIItem);
	}
	
	//Also remember if you are adding event listeners to events you need to also remove them:
	void OnDisable()
	{
	    uiItem.OnDown -= AvatarDown;
	    uiItem.OnClickUIItem -= Clicked;
	}
}
