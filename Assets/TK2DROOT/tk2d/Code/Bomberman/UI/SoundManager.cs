using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	[SerializeField] AudioClip plantBombClip;
	
	[SerializeField] AudioClip rhinoDeathSound;
	[SerializeField] AudioClip zebraDeathSound;
	[SerializeField] AudioClip birdDeathSound;
	[SerializeField] AudioClip tigerDeathSound;
	
	[SerializeField] AudioClip pickupSound;
	[SerializeField] AudioClip playerWalkSound;
	[SerializeField] AudioClip gameOverSound;
	
	[SerializeField] AudioClip hudMoveSound;
	
	public void PlayPlantBombSound(Vector3 position)
	{
		//AudioSource.PlayClipAtPoint(plantBombClip, position);
		
		audio.PlayOneShot(plantBombClip);
	}
	
	public void PlayMoveSound(Vector3 position)
	{
		AudioSource.PlayClipAtPoint(playerWalkSound, position);
	}
	
	public void PlayHUDClickSound(Vector3 position)
	{
		AudioSource.PlayClipAtPoint(hudMoveSound, position);
	}
	
	public void PlayGameOverSound()
	{
		AudioSource.PlayClipAtPoint(gameOverSound, new Vector3(0,0,0));
	}
	
	
	public void PlayItemPickupSound(Vector3 position)
	{
		AudioSource.PlayClipAtPoint(pickupSound, position);
	}
	
	
	public void PlayDeathSound(CharacterType icon, Vector3 position)
	{
		if(icon == CharacterType.Rhino)
			AudioSource.PlayClipAtPoint(rhinoDeathSound, position);
		else if(icon == CharacterType.Zebra)
			AudioSource.PlayClipAtPoint(zebraDeathSound, position);
		else if(icon == CharacterType.Bird)
			AudioSource.PlayClipAtPoint(birdDeathSound, position);	
		else if(icon == CharacterType.Tiger)
			AudioSource.PlayClipAtPoint(tigerDeathSound, position);		
	}
	
	public void PlayItemSound(Vector3 position)
	{
		AudioSource.PlayClipAtPoint(pickupSound, position);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
