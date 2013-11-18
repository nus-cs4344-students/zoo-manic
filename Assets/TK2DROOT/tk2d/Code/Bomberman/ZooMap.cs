using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ZooMap : MonoBehaviour {
	
	//float heightDimension = 100f;
	//float widthDimension = 138.0f;
	
	// Number of cells
	public static float NumberofRows = 16.0f;				// number rows
	public static float NumberofCols = 11.0f;				// num colns
	
	// Cell dimensions
	public static float cellWidth = 8.0f;		// previously is 3.4, 3.0
	public static float cellHeight = 8.0f;
	
	bool isCellCreated = false;
	
	[SerializeField] GameObject woodenCratePrefab;	
	[SerializeField] GameObject rockPrefab;	
	
	[SerializeField] GameObject icyCratePrefab;	
	[SerializeField] GameObject lakeCratePrefab;	
	[SerializeField] GameObject desertCratePrefab;	

	[SerializeField] GameObject m_InvulnerablePowerup;
	[SerializeField] GameObject m_RangePowerup;
	[SerializeField] GameObject m_ShakePowerup;
	[SerializeField] GameObject m_SpeedPowerup;
	[SerializeField] GameObject m_TrickPowerup;
	
	int powerupValue = 0;
	GameObject powerUpToCreate;

	//horizontalCellNum, verticalCellNum, type, item
	

	// Store the cells info in a dictionary for efficiency
	// cellNum, object{type, item,x, y}
	// the game object here refers to the powerup, or the brick etc
	private static Dictionary<string, Cell> zooMapInfoDict = new Dictionary<string, Cell>();		// Static so it won't disappear when scene change
	//private Dictionary<int, List<GameObject>> zooMapItemObjectDict = new Dictionary<int, List<GameObject>>();
	
	
	// Drawing of the Grid
	bool zooMapCreated = false;
 
	int columns;
	int rows;
 
	float deltaWidth;
	float deltaHeight;
	Vector3[,] gridArray;

	public static float GetHorizontalCell(float horizontalPos)
	{
		// round down
		return Mathf.Floor ( (horizontalPos - cellWidth) / cellWidth);
	}
	
	public static float GetVerticalCell(float verticalPos)
	{
		
		return Mathf.Floor ( (verticalPos - cellHeight)  / cellHeight);
	}
	
	public static float GetHorizontalPos(float horizontalCell)
	{
		// Start from the first cell
		return (horizontalCell * cellWidth) + cellWidth;
	}
	
	public static float GetVerticalPos(float verticalCell)
	{
		// Start from the first cell
		return (verticalCell * cellHeight) + cellHeight;
	}
	
	public static bool IsObstacle(int horizontalCell, int verticalCell)
	{
		if(zooMapInfoDict == null || zooMapInfoDict.Count == 0)
		{
			Debug.Log ("Map is EMPTY");
			return false;
		}
		
		string cellID = horizontalCell+":"+verticalCell;
		
		bool hasObstacle = false;
		
		if(zooMapInfoDict.ContainsKey(cellID))
		{
			Cell currCell = (Cell) zooMapInfoDict[cellID];
			
			if(currCell.CellType != 0)
				hasObstacle = true;
			else if(currCell.IsABomb)
				hasObstacle = true;
			
			Debug.Log("Checking Cell Index: "+cellID + "  Obstacle? : "+ hasObstacle );
		}

		return hasObstacle;
	}
	
	// Use this for initialization
	void Awake()
	{
		// Make this game object and all its transform children
		// survive when loading a new scene.
		DontDestroyOnLoad (transform.gameObject);
	}
	
	/*void InitDrawGrid()
	{
		//Does the math to find the relative distance between points (Index-Based means we have to subtract 1 from column/row count)
	 	deltaWidth = ( cellWidth / ( columns-1 ));
	 	deltaHeight = ( cellHeight / ( rows-1 ));
	 
	  	gridArray = new Vector3[ rows , columns ];
	 
	  	for( var u = 0; u < rows; u++ )
		{
	    	for( var i = 0; i < columns; i++ )
			{
	     		//Constructing grid using the transform as the UPPER LEFT CORNER
	    		 gridArray[u,i] = new Vector3( transform.position.x + (deltaWidth*i), transform.position.y + ( -deltaHeight*u ), transform.position.z );
	     		//These next two lines offset the points so that the transform is now used as the CENTER --(remove these if you want to use it as the corner)--
	     		gridArray[u,i].x = gridArray[u,i].x - ( cellWidth/2);
	     		gridArray[u,i].y = gridArray[u,i].y + ( cellHeight/2);         
	  		}
		}
	}*/
	
	public void StartZooMap()
	{
		zooMapInfoDict.Clear();
		InitZooMap();
		zooMapCreated = true;
	}
	
	
	void Start () 
	{
		
	}
	
	public void SpawnPowerUp(PowerupType powerupType, Cell thisCell, float horizontalCell, float verticalCell)
	{
		switch (powerupType)
		{
			case PowerupType.Invulnerability:
				powerUpToCreate = m_InvulnerablePowerup;
				break;
			case PowerupType.Range:
				powerUpToCreate = m_RangePowerup;
				break;
			case PowerupType.Shake:
				powerUpToCreate = m_ShakePowerup;
				break;
			case PowerupType.Speed:
				powerUpToCreate = m_SpeedPowerup;
				break;
			case PowerupType.Trick:
				powerUpToCreate = m_TrickPowerup;
				break;
		}
		
		Debug.Log("SPAWNING POWERUP: "+powerupType + "   CellX: "+horizontalCell + "   Vertical Cell: "+verticalCell);

		// Spawn a powerup then destroy the game object
		Vector3 powerupPos = new Vector3(GetHorizontalPos(horizontalCell), GetVerticalPos(verticalCell), powerUpToCreate.transform.position.z);
		GameObject powerupInstance = Instantiate(powerUpToCreate, powerupPos, transform.rotation) as GameObject;
		
		thisCell.CellGameObject = powerupInstance;
	}
	
	private void InitZooMap()
	{
		// type  empty - 0, box - 1, rock - 2
		// item  no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
		// column is the x, row is the y
		
		for(int row=0; row < (int) ZooMap.NumberofRows ; row++)
		{
			for(int column=0; column < (int) ZooMap.NumberofCols ; column++)
			{
				string zooCellID = row+":"+column;
				if(zooMapInfoDict.ContainsKey(zooCellID) == false)
					zooMapInfoDict.Add (zooCellID, new Cell(row, column));
			}
		}
	}
	
	public void UpdateCellWithBomb(string cellID, bool bombExist)
	{
		if(zooMapInfoDict.ContainsKey(cellID))
		{
			Cell currCell = (Cell) zooMapInfoDict[cellID];
			currCell.IsABomb = bombExist;
		}
	}
	
	/*List<T> CreateList<T>(params T[] values)
	{
    	return new List<T>(values);
	}*/
	
	// Cell number is from 0 to 27*37
	// type  empty - 0, box - 1, rock - 2
	// item  no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
	public void UpdateZooMap(long cellType, long cellItem, long horizontalCellNum, long verticalCellNum, string cellID, int mapType)
	{	
		if(zooMapInfoDict == null || zooMapInfoDict.Count == 0)
		{
			Debug.Log ("Map has not been INITIALISED");
			return;
		}
		
		Cell currCell = (Cell) zooMapInfoDict[cellID];
		
		// If server says add wooden crate, but your side has no crate
		if(cellType == 1 && currCell.CellType == 0)
		{
			// Add the wooden crate into the scene
			GameObject crateObj = null;
			
			if(mapType == 1)
			{
				crateObj = Instantiate(woodenCratePrefab, 
				new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			}
			else if(mapType == 2)
			{
				crateObj = Instantiate(lakeCratePrefab, 
				new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			}
			else if(mapType == 3)
			{
				crateObj = Instantiate(icyCratePrefab, 
				new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			}
			else if(mapType == 4)
			{
				crateObj = Instantiate(desertCratePrefab, 
				new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			}
			
			currCell.CellType = 1;
			currCell.CellGameObject = crateObj;
		}
		else if(cellType == 2 && currCell.CellType == 0)
		{
			GameObject rockObj = Instantiate(rockPrefab, 
			new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			
			currCell.CellType = 2;
			currCell.CellGameObject = rockObj;
		}
		else if(cellType == 0)
		{
			// Receive the updated type from the server
			currCell.CellType = (int) cellType;
			
			// delete previous game object
			if(cellType == 0)
			{
				if(currCell.CellGameObject != null)
				{
					Destroy (currCell.CellGameObject);
					currCell.CellGameObject = null;
				}
			}
			
			if(cellItem == 1)
				SpawnPowerUp(PowerupType.Range, currCell, horizontalCellNum, verticalCellNum);
			else if(cellItem == 2)
				SpawnPowerUp(PowerupType.Speed, currCell, horizontalCellNum, verticalCellNum);
			else if(cellItem == 3)
				SpawnPowerUp(PowerupType.Invulnerability, currCell, horizontalCellNum, verticalCellNum);
			else if(cellItem == 4)
				SpawnPowerUp(PowerupType.Trick, currCell, horizontalCellNum, verticalCellNum);
			else if(cellItem == 5)
				SpawnPowerUp(PowerupType.Shake, currCell, horizontalCellNum, verticalCellNum);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	/*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
		DrawOwnGrid();
    }*/
}
