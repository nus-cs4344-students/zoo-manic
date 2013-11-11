using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	[SerializeField] GameObject explosionParticle;
	
	bool rightHitObstacle = false;
	bool leftHitObstacle = false;
	bool upHitObstacle = false;
	bool downHitObstacle = false;
	
	[SerializeField] AudioClip explosionClip;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	// if range is 3
	public void InitExplosion(int range)
	{
		//float cellWidth = ZooMap.cellWidth;
		//float cellHeight = ZooMap.cellHeight;
		
		
		AudioSource.PlayClipAtPoint(explosionClip, transform.position);
		
		float cellWidth = 3.0f;
		float cellHeight = 3.0f;
		
		// Center of the explosion
		GameObject explosionInstance = Instantiate(explosionParticle, transform.position, transform.rotation) as GameObject;
		explosionInstance.transform.parent = transform;

		//hits = Physics.RaycastAll (transform.position, transform.forward, 100.0);
		
		//RaycastHit[] leftHits = new RaycastHit[10];
		//Ray leftRay = new Ray(transform.position);
		
		// initially it is -1
		int rightHitCount = -1;
		int upHitCount = -1;
		int downHitCount = -1;
		int leftHitCount = -1;

		for(int index=1; index <= range; index++)
		{
			RaycastHit rightRayHit = new RaycastHit();
			if (Physics.Raycast (transform.position + new Vector3(cellWidth * index-1, 0, 0), Vector3.right, out rightRayHit, 5.0f) || Physics.Raycast (transform.position + new Vector3(cellWidth * index-1, 0, 0), Vector3.right, out rightRayHit, 1.0f)) {
				if(rightRayHit.collider.tag == "Obstacle" || rightRayHit.collider.tag == "Untagged")
					rightHitObstacle = true;
				else if(rightRayHit.collider.tag == "Crate")
					rightHitCount++;
			}
			
			RaycastHit leftRayHit = new RaycastHit();
			if (Physics.Raycast (transform.position - new Vector3(cellWidth * index-1, 0, 0), Vector3.left, out leftRayHit, 5.0f) || Physics.Raycast (transform.position - new Vector3(cellWidth * index-1, 0, 0), Vector3.left, out leftRayHit, 1.0f)) {
				if(leftRayHit.collider.tag == "Obstacle" || leftRayHit.collider.tag == "Untagged")
					leftHitObstacle = true;
				else if(leftRayHit.collider.tag == "Crate")					
					leftHitCount++;
			}
			
			RaycastHit upRayHit = new RaycastHit();
			if (Physics.Raycast (transform.position + new Vector3(0, cellHeight * index-1, 0), Vector3.up, out upRayHit, 5.0f) || Physics.Raycast (transform.position + new Vector3(0, cellHeight * index-1, 0), Vector3.up, out upRayHit, 1.0f)) {
				if(upRayHit.collider.tag == "Obstacle" || upRayHit.collider.tag == "Untagged")
					upHitObstacle = true;					
				else if(upRayHit.collider.tag == "Crate")
					upHitCount++;
			}
			
			RaycastHit downRayHit = new RaycastHit();
			if (Physics.Raycast (transform.position - new Vector3(0, cellHeight * index-1, 0), Vector3.down, out downRayHit, 5.0f) || Physics.Raycast (transform.position - new Vector3(0, cellHeight * index-1, 0), Vector3.down, out downRayHit, 1.0f)) {
				if(downRayHit.collider.tag == "Obstacle" || downRayHit.collider.tag == "Untagged")
					downHitObstacle = true;					
				else if (downRayHit.collider.tag == "Crate")
					downHitCount++;
			}
			
			/*var rightRay = transform.TransformDirection (Vector3.right);
			if (Physics.Raycast (transform.position + new Vector3(cellWidth * index-1, 0, 0), rightRay, 10.0f))
				rightHitObstacle = true;
			
			var leftRay = transform.TransformDirection (Vector3.left);
			if (Physics.Raycast ( transform.position - new Vector3(cellWidth * index-1, 0, 0), leftRay, 10.0f)) 
				leftHitObstacle = true;
			
			
			var forwardRay = transform.TransformDirection (Vector3.up);
			if (Physics.Raycast (transform.position + new Vector3(0, cellHeight * index-1, 0), forwardRay, 10.0f)) 
				upHitObstacle = true;
			
			var backRay = transform.TransformDirection (Vector3.down);
			if (Physics.Raycast (transform.position - new Vector3(0, cellHeight * index-1, 0), backRay, 10.0f)) 
				downHitObstacle = true;*/

			if( !rightHitObstacle && rightHitCount != 1)
			{
				// Right
				GameObject explosionInstanceRight = Instantiate(explosionParticle, transform.position + new Vector3(cellWidth * index, 0, 0), transform.rotation) as GameObject;
				explosionInstanceRight.transform.parent = transform;
				
				if(rightHitCount == 0)
					rightHitCount++;
			}
			
			if( !leftHitObstacle && leftHitCount != 1)
			{
				// Left
				GameObject explosionInstanceLeft = Instantiate(explosionParticle, transform.position - new Vector3(cellWidth * index, 0, 0), transform.rotation) as GameObject;
				explosionInstanceLeft.transform.parent = transform;
				
				// increment the count if it hits the crate
				if(leftHitCount == 0)
					leftHitCount++;
			}
			
		
			if( !upHitObstacle && upHitCount != 1)
			{
				// Up
				GameObject explosionInstanceUp = Instantiate(explosionParticle, transform.position + new Vector3(0, cellHeight * index, 0), transform.rotation) as GameObject;
				explosionInstanceUp.transform.parent = transform;
				
				if(upHitCount == 0)
					upHitCount++;
			}
			
			if( !downHitObstacle && downHitCount != 1)
			{
				// Down
				GameObject explosionInstanceDown = Instantiate(explosionParticle, transform.position - new Vector3(0, cellHeight * index, 0), transform.rotation) as GameObject;
				explosionInstanceDown.transform.parent = transform;
				
				if(downHitCount == 0)
					upHitCount++;
			}
		}
	}
}
