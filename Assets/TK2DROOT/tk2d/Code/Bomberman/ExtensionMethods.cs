using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is a script used in the Youtube video "RayCasting Script to detect sides of 2D collision in Unity. C#"
// Youtube Link - http://www.youtube.com/watch?v=glMs6qZOOV8

internal static class ExtensionMethods
{
    // ------------------------------------------
    // Unity Extensions
    // ------------------------------------------

    //get list of children
    public static List<GameObject> GetChildren(this GameObject go)
    {

        List<GameObject> children = new List<GameObject>();

        foreach (Transform tran in go.transform)
        {
            children.Add(tran.gameObject);
        }

        return children;
    }
	
	public static void SetAlpha (this Material material, float value) {
		if(material)
		{
	    	Color color = material.color;
    		color.a = value;
    		material.color = color;
		}
	}
}