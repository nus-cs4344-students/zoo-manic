using UnityEngine;
using System.Collections;

// This script destroys the explosion prefab after a certain time
public class DestroyAfterTime : MonoBehaviour {
	
	[SerializeField] float destroyTime;
	
	/*var destroyTime : Int; // This is the time in seconds
function Start(){
 
yield WaitForSeconds(destroyTime);
Destroy(gameObject);
}ameObject instance = Instantiate(thePrefab, transform.position, transform.rotation); */
	
	// Use this for initialization
	void Start () {
		StartCoroutine(DestroyAfter());
	}
	
	IEnumerator DestroyAfter()
    {
    	yield return new WaitForSeconds(destroyTime);   //Wait
		Destroy (gameObject);
    }
}
