using UnityEngine;
using System.Collections;
 
public class SmoothCamera2D : MonoBehaviour {
 
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
	
	private float left_clip_x = 32.0f;
	private float bottom_clip_y = 23.0f;
	private float top_clip_y = 78.0f;
	private float right_clip_x = 104.0f;

    // Update is called once per frame
    void Update () 
    {
       if (target)
       {
			// get position but must first convert camera to world coordinates
        	Vector3 point = camera.WorldToViewportPoint(target.position);
        	Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        	Vector3 destination = transform.position + delta;
			//destination.y = 0;
			
			// set the clipping range of camera
			if(destination.x < left_clip_x)
				destination.x = left_clip_x;
			else if(destination.x > right_clip_x)
				destination.x = right_clip_x;
			
			if(destination.y < bottom_clip_y)
				destination.y = bottom_clip_y;
			else if(destination.y > top_clip_y)
				destination.y = top_clip_y;
			
        	transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
       }
 
    }
}