using UnityEngine;
using System.Collections;

public enum LobbyNumber {Num_1, Num_2, Num_3, Num_4, Num_5, Num_6, Num_7, Num_8, Num_9, Num_10}

public class ListButton : MonoBehaviour {
	
	
	[SerializeField] tk2dUIItem uiItem;
	[SerializeField] GameObject clickedObject;
	[SerializeField] LobbyNumber lobbyNum;

	// Button class calls scene manager, and scene manager calls serverconnection/gamemanager
	private SceneManager sceneManager;
	
	// Use this for initialization
	void Start () 
	{
		sceneManager = GameObject.Find ("SceneObject").GetComponent<SceneManager>();
		
		// If button is initialised
		if(lobbyNum == LobbyNumber.Num_1)
		{
			int selectedNumber = 1;
			clickedObject.transform.Find ("ListSelected").gameObject.SetActive(true);
			clickedObject.GetComponent<tk2dUIHoverItem>().enabled = false;
			sceneManager.UpdateSelectedRoom(selectedNumber);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void OnEnable()
	{
	    uiItem.OnDown += ListDown;
	    uiItem.OnClickUIItem += Clicked;
	}
	
	void ListDown()
	{
		int selectedNumber = 1;
		switch(lobbyNum)
		{
			case LobbyNumber.Num_1:
			selectedNumber = 1;
			break;

			case LobbyNumber.Num_2:
			selectedNumber = 2;
			break;

			case LobbyNumber.Num_3:
			selectedNumber = 3;
			break;
			
			case LobbyNumber.Num_4:
			selectedNumber = 4;
			break;

			case LobbyNumber.Num_5:
			selectedNumber = 5;
			break;

			case LobbyNumber.Num_6:
			selectedNumber = 6;
			break;

			case LobbyNumber.Num_7:
			selectedNumber = 7;
			break;

			case LobbyNumber.Num_8:
			selectedNumber = 8;
			break;
			
			case LobbyNumber.Num_9:
			selectedNumber = 9;
			break;
			
			case LobbyNumber.Num_10:
			selectedNumber = 10;
			break;
		}
		
		sceneManager.ClearLobbySelection();
		
		//clickedObject.GetChildren
		clickedObject.transform.Find ("ListSelected").gameObject.SetActive(true);
		clickedObject.GetComponent<tk2dUIHoverItem>().enabled = false;

		sceneManager.UpdateSelectedRoom(selectedNumber);
	}

	
	void Clicked(tk2dUIItem clickedUIItem)
	{
	    //Debug.Log("Clicked:" + clickedUIItem);
	}
	
	//Also remember if you are adding event listeners to events you need to also remove them:
	void OnDisable()
	{
	    uiItem.OnDown -= ListDown;
	    uiItem.OnClickUIItem -= Clicked;
	}
}
