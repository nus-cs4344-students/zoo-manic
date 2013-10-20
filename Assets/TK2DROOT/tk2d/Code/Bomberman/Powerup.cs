using UnityEngine;
using System.Collections;


public enum PowerupType {Invulnerability, Range, Shake, Speed, Trick}

public class Powerup : MonoBehaviour {
	
	
	[SerializeField] PowerupType powerupType;
	
	bool isDestroyable = false;
	float powerUpInvunerableTime = 3.0f;

	// Use this for initialization
	void Start () 
	{
	}
	
	// When the powerup collide with game object
	void OnCollisionEnter(Collision collider)
    {
		// To do collision detection effectively, do Powerup Effect
		if(collider.gameObject.tag == "Player"){
			
			DoPowerUp();
			// get powerup
			Destroy(gameObject);
		}
    }
	
	void DoPowerUp()
	{
		switch (powerupType)
		{
			case PowerupType.Invulnerability:
			Debug.Log("Invunerable");
			break;
			
			case PowerupType.Range:
			Debug.Log("Range");
			break;
			
			case PowerupType.Shake:
			Debug.Log("Shake");
			break;
			
			case PowerupType.Speed:
			Debug.Log("Speed");
			break;
			
			case PowerupType.Trick:
			Debug.Log("Trick");
			break;
		}
	}
	
	// This method will be invoked by the "ExplosionDamage.cs"
	public void KillObject()
    {
		// After 3 seconds after the powerup is spawn, if explosion collides, destroy the powerup
		if(isDestroyable)
			Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () 
	{
		if(isDestroyable == false)
		{
			powerUpInvunerableTime -= Time.deltaTime;
			if(powerUpInvunerableTime <= 0.0f)
				isDestroyable = true;
		}
	}
}
