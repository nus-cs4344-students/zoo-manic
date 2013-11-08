using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	
	// Draws 2 buttons, one with an image, and other with a text
	// And print a message when they got clicked.
	//[SerializeField] Texture btnTexture;
	[SerializeField] GUIStyle hudboxStyle;
	[SerializeField] GUIStyle directionalPadStyle;
	
	[SerializeField] GUIStyle button_direction_up;
	[SerializeField] GUIStyle button_direction_down;
	[SerializeField] GUIStyle button_direction_left;
	[SerializeField] GUIStyle button_direction_right;
	
	private int width_offset = 30;		// width of hud box
	private int height_offset = 180;		// height of hud box
	private int space = 30;
	
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
		
		GUI.Box(new Rect (25 + space * 1, (Screen.height - height_offset + 47/2), 131, 133), "", directionalPadStyle);
		
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
