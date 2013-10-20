using UnityEngine;
using System.Collections;

public class ExplosionDamage : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// When the explosion collider collide with the game object
	void OnCollisionEnter(Collision collider)
    {
		if(collider.gameObject.tag == "Player")
			collider.gameObject.BroadcastMessage("KillPlayer");
		else if(collider.gameObject.tag == "Crate"){
			collider.gameObject.BroadcastMessage("KillObject");
		}
    }
}
