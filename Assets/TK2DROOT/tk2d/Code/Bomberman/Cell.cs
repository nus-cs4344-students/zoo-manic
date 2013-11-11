using UnityEngine;
using System.Collections;

public class Cell  
{
	private int cellType = 0;
	private int itemType = 0;
	
	// get from zoomap
	//private float cellWidth = ZooMap.cellWidth;		
	//private float cellHeight = ZooMap.cellHeight;
	
	private int numOfRows = (int) ZooMap.NumberofRows;				// number rows
	private int numOfCols = (int) ZooMap.NumberofCols;				// num colns
	
	// index of the cell, e.g. first cell has the row index of 0, col index 0
	private int rowIndex;
	private int columnIndex;
	
	private GameObject cellObject;
	
	public Cell(int rowIndex, int columnIndex)
	{
		this.rowIndex = rowIndex;
		this.columnIndex = columnIndex;
		//rowIndex = index % numOfCols;
		//columnIndex = index / numOfCols;
	}
	
	public GameObject CellGameObject 
	{
        get { return cellObject; }
        set { cellObject = value; }
    }
	
	public int RowNum
	{
        get { return rowIndex; }
        set { rowIndex = value; }
    }
	
	public int ColNum 
	{
        get { return columnIndex; }
        set { columnIndex = value; }
    }
	
	public string PrintCell()
	{
		return rowIndex+":"+columnIndex;
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
