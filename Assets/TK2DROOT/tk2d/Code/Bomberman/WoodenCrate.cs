using UnityEngine;
using System.Collections;

public class WoodenCrate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// This method will be invoked by the "Bomb.cs"
	public void KillObject()
    {
		Destroy(gameObject);
    }
}
