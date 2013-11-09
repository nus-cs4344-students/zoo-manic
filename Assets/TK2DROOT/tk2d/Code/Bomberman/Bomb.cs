using UnityEngine;
using System.Collections;


public class Bomb : MonoBehaviour {
	
	
	[SerializeField] string bombType;
	[SerializeField] int	playerId;
	[SerializeField] bool isShakeable;
	[SerializeField] float timeLeftToExplode;
	[SerializeField] GameObject explosionPrefab;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeLeftToExplode -= Time.deltaTime;
    	/*if (timeLeftToExplode <= 0.0f)
    	{
			Instantiate(explosionPrefab, transform.position, transform.rotation);
			
			// Update the bomb limit of player
			GameObject playerGO = GameObject.FindWithTag("Player");
			
			// e.g. player still alive
			if(playerGO != null)
			{
				var characterScript = playerGO.GetComponent<CharacterAnimController>();
				
				// make sure does not exceed 3
				if(characterScript.BombLimit < 3)
					characterScript.BombLimit++;
			}

			// See whether it exceeds the countdown
			Destroy(gameObject);
    	}*/
	}
	
	public void Explode()
	{
		Instantiate(explosionPrefab, transform.position, transform.rotation);
			
		// Update the bomb limit of player
		GameObject playerGO = GameObject.FindWithTag("Player");
		
		// e.g. player still alive
		if(playerGO != null)
		{
			var characterScript = playerGO.GetComponent<CharacterAnimController>();
			
			// make sure does not exceed 3
			if(characterScript.BombLimit < 3)
				characterScript.BombLimit++;
		}

		// See whether it exceeds the countdown
		Destroy(gameObject);
	}
}
