using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is a script used in the Youtube video "RayCasting Script to detect sides of 2D collision in Unity. C#"
// Youtube Link - http://www.youtube.com/watch?v=glMs6qZOOV8

//calculate which side of an object was hit (up, down, right, left)
public class DirectionRaycasting : MonoBehaviour
{

    //-------------------------------
    //          fields
    //-------------------------------
    public bool collisionUp;
    public bool collisionDown;
    public bool collisionLeft;
    public bool collisionRight;

    //show rays in debug
    public bool showRays = false;

    //ray cast fields
    public float rayDistance;

    //the ray that hit something
    public RaycastHit TileHit;

    //raycast related
    public List<GameObject> rayPoints;
    public List<Ray> rays;

    public List<Ray> raysUp;
    public List<Ray> raysDown;
    public List<Ray> raysLeft;
    public List<Ray> raysRight;

    //-------------------------------
    //          Unity
    //-------------------------------
    void Start()
    {
        //acquire the ray point origins
        rayPoints = new List<GameObject>();
        getRays();
    }


    void Update()
    {
        //check collision on all sides
        checkCollision();

        //debug
        if (showRays)
            drawRaycast();
    }

    //-------------------------------
    //          Functions
    //-------------------------------

    void getRays()
    {
        //get the object named Raycasting
        List<GameObject> children = gameObject.GetChildren();

        //get the children inside Raycasting
        List<GameObject> children2 = new List<GameObject>();

        //check inside raycasting object for the children (children are inside the raycasting folder)
        for (int i = 0; i < children.Count; i++)
        {
            if(children[i].name == "RayCasting")
                children2 = children[i].GetChildren();
        }

        for (int i = 0; i < children2.Count; i++)
        {
            //Debug.Log(i + " " + children2[i].gameObject.name);
            rayPoints.Add(children2[i]);
        }
    }

    void checkCollision()
    {
        //-------------------------------
        //          init rays list
        //-------------------------------
        List<Ray> raysUp = new List<Ray>();
        List<Ray> raysDown = new List<Ray>();
        List<Ray> raysLeft = new List<Ray>();
        List<Ray> raysRight = new List<Ray>();
		List<Ray> raysDiagonalRight = new List<Ray>();

        TileHit = new RaycastHit();

        //assign rays to list
        for (int i = 0; i < rayPoints.Count; i++)
        {

            //up
            if (rayPoints[i].gameObject.name == "up")
            {
                raysUp.Add(new Ray(new Vector3(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y, this.gameObject.transform.position.z), Vector3.up));
            }

            //down
            if (rayPoints[i].gameObject.name == "down")
            {
                raysDown.Add(new Ray(new Vector3(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y, this.gameObject.transform.position.z), Vector3.down));
            }


            //left
            if (rayPoints[i].gameObject.name == "left")
            {
                raysLeft.Add(new Ray(new Vector3(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y, this.gameObject.transform.position.z), Vector3.left));
            }

            //right
            if (rayPoints[i].gameObject.name == "right")
            {
                raysRight.Add(new Ray(new Vector3(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y, this.gameObject.transform.position.z), Vector3.right));
            }
        }

        //-------------------------------
        //          check up
        //-------------------------------
        //check normal collision
        for (int i = 0; i < raysUp.Count; i++)
        {
            //check all rays
            if (Physics.Raycast(raysUp[i], out TileHit, rayDistance + .001f))
            {
                collisionUp = true;
            }
            else
                collisionUp = false;

            ////breakout of loop the moment 1 ray has a collision
            if (collisionUp)
                break;
        }

        //-------------------------------
        //          check down
        //-------------------------------
        for (int i = 0; i < raysDown.Count; i++)
        {
            //check all rays
            if (Physics.Raycast(raysDown[i], out TileHit, rayDistance + .001f))
            {
                collisionDown = true;
            }
            else
                collisionDown = false;

            //breakout of loop the moment 1 ray has a collision
            if (collisionDown)
                break;
        }

        //-------------------------------
        //          check left
        //-------------------------------
        for (int i = 0; i < raysLeft.Count; i++)
        {
            //check all rays
            if (Physics.Raycast(raysLeft[i], out TileHit, rayDistance + .001f))
            {
                collisionLeft = true;
            }
            else
                collisionLeft = false;

            //breakout of loop the moment 1 ray has a collision
            if (collisionLeft)
                break;
		}
		
		//-------------------------------
        //          check right
        //-------------------------------
        for (int i = 0; i < raysRight.Count; i++)
        {
            //check all rays
            if (Physics.Raycast(raysRight[i], out TileHit, rayDistance + .001f))
            {
                collisionRight = true;
            }
            else
                collisionRight = false;

            //breakout of loop the moment 1 ray has a collision
            if (collisionRight)
                break;
        }

    }


    //-------------------------------
    //          Functions Debug
    //-------------------------------
    void drawRaycast()
    {
        //draw all rays in list
        for (int i = 0; i < rayPoints.Count; i++)
        {

            //draw up
            if (rayPoints[i].gameObject.name == "up")
                Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y + rayDistance, rayPoints[i].gameObject.transform.position.z), Color.red);

            //draw down
            if (rayPoints[i].gameObject.name == "down")
                Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y - rayDistance, rayPoints[i].gameObject.transform.position.z), Color.red);

            //draw left
            if (rayPoints[i].gameObject.name == "left")
                Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(rayPoints[i].gameObject.transform.position.x - rayDistance, rayPoints[i].gameObject.transform.position.y, rayPoints[i].gameObject.transform.position.z), Color.red);

            //draw right
            if (rayPoints[i].gameObject.name == "right")
                Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(rayPoints[i].gameObject.transform.position.x + rayDistance, rayPoints[i].gameObject.transform.position.y, rayPoints[i].gameObject.transform.position.z), Color.red);

        }

    }

}