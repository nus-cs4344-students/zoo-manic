using UnityEngine;
using System.Collections;

public class Cell  
{
	private int cellType = 0;
	private int itemType = 0;
	
	private GameObject cellObject;
	
	public Cell()
	{
	}
	
	public GameObject CellGameObject 
	{
        get { return cellObject; }
        set { cellObject = value; }
    }
	
	public int CellType 
	{
        get { return cellType; }
        set { cellType = value; }
    }
	
	public int ItemType 
	{
        get { return itemType; }
        set { itemType = value; }
    }
}
