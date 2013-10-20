using UnityEngine;
using System.Collections;

public class WoodenCrate : MonoBehaviour {
	
	[SerializeField] GameObject m_InvulnerablePowerup;
	[SerializeField] GameObject m_RangePowerup;
	[SerializeField] GameObject m_ShakePowerup;
	[SerializeField] GameObject m_SpeedPowerup;
	[SerializeField] GameObject m_TrickPowerup;
	
	int powerupValue = 0;
	PowerupType powerupType;
	GameObject powerUpToCreate;
	
	bool isDestroyed = false;

	// Use this for initialization
	void Start () {
		
		//if(powerupValue = 0)
		//	powerupType = PowerupType.Invulnerability;
		
		powerupType = PowerupType.Invulnerability;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	// This method will be invoked by the "ExplosionDamage.cs"
	public void KillObject()
    {
		// To prevent multiple calls
		if(isDestroyed)
			return;
		
		switch (powerupType)
		{
			case PowerupType.Invulnerability:
				powerUpToCreate = m_InvulnerablePowerup;
				break;
			case PowerupType.Range:
				powerUpToCreate = m_RangePowerup;
				break;
			case PowerupType.Shake:
				powerUpToCreate = m_ShakePowerup;
				break;
			case PowerupType.Speed:
				powerUpToCreate = m_SpeedPowerup;
				break;
			case PowerupType.Trick:
				powerUpToCreate = m_TrickPowerup;
				break;
		}

		// Spawn a powerup then destroy the game object
		Vector3 cratePos = transform.position + new Vector3(0, 0, 1);
		GameObject powerupInstance = Instantiate(powerUpToCreate, cratePos, transform.rotation) as GameObject;
		
		isDestroyed = true;
		Destroy(gameObject);
    }
}
