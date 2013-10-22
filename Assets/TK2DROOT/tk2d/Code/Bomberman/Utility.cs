using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public static bool IsEmpty(ArrayList list)
	{
		return list.Count == 0;
	}
}
