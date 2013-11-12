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
	
	[SerializeField] Transform m_spawnPoint1;	
	[SerializeField] Transform m_spawnPoint2;
	[SerializeField] Transform m_spawnPoint3;
	[SerializeField] Transform m_spawnPoint4;	
	
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
		
		/*int cellIndex = 0;
		
		// the client start from horizontal cell 0,0 by right should be 1
		//horizontalCell += 1;
		//verticalCell += 1;
		
		if(horizontalCell == 0f)
			cellIndex = verticalCell;
		else if(verticalCell == 0f)
			cellIndex = horizontalCell;
		else
			cellIndex = (horizontalCell * (int) NumberofCols) + verticalCell;*/
			
		//cellIndex = (horizontalCell * (int) NumberofRows) + verticalCell;
			
			
		//cellIndex = (verticalCell * (int) NumberofRows) + horizontalCell;
		
		
		string cellID = horizontalCell+":"+verticalCell;
		Cell currCell = (Cell) zooMapInfoDict[cellID];
		Debug.Log("Checking Cell Index: "+cellID + "  Obstacle? : "+ (currCell.CellType != 0) );
		
		return currCell.CellType != 0;
	}
	
	// Use this for initialization
	void Awake()
	{
		InitZooMap();
		InitSpawnPoint();
		Debug.Log ("Zoo Map Created! Map Size: "+zooMapInfoDict.Count);
		
		zooMapCreated = true;
		
		
		// Draw the grid
		
		
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
	
	private void InitSpawnPoint()
	{
		/*m_spawnPoint1.localPosition = new Vector3(GetHorizontalPos(0), GetVerticalPos(0), 0);
		m_spawnPoint2.localPosition = new Vector3(GetHorizontalPos(horizontalCell-1), GetVerticalPos(0), 0);
		m_spawnPoint3.localPosition = new Vector3(0, GetVerticalPos(verticalCell-1), 0);
		m_spawnPoint4.localPosition = new Vector3(GetHorizontalPos(horizontalCell-1), GetVerticalPos(verticalCell-1), 0);*/
	}
	
	private void InitZooMap()
	{
		// type  empty - 0, box - 1, rock - 2
		// item  no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
		/*for(int index=0; index < (int) ZooMap.NumberofRows * ZooMap.NumberofCols; index++)
		{
			zooMapInfoDict.Add (index, new Cell(index));
		}*/
		
		// column is the x, row is the y
		
		for(int row=0; row < (int) ZooMap.NumberofRows ; row++)
		{
			for(int column=0; column < (int) ZooMap.NumberofCols ; column++)
			{
				string zooCellID = row+":"+column;
				zooMapInfoDict.Add (zooCellID, new Cell(row, column));
			}
		}
	}
	
	/*List<T> CreateList<T>(params T[] values)
	{
    	return new List<T>(values);
	}*/
	
	// Cell number is from 0 to 27*37
	// type  empty - 0, box - 1, rock - 2
	// item  no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
	public void UpdateZooMap(long cellType, long cellItem, long horizontalCellNum, long verticalCellNum, string cellID)
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
			GameObject woodenObj = Instantiate(woodenCratePrefab, 
			new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			
			currCell.CellType = 1;
			currCell.CellGameObject = woodenObj;
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

	void DrawOwnGrid()
	{
		for (int i = 0; i < (int) ZooMap.NumberofRows * ZooMap.NumberofCols; i++) {
			/*Cell cell = (Cell) zooMapInfoDict[i];
			
			// Off set to the first position of the grid
			float fromX = GetHorizontalPos( (float) cell.RowNum) - cellWidth / 2;
			float fromY = GetVerticalPos( (float) cell.ColNum ) - cellHeight / 2;
			//Vector3 fromPosition = new Vector3(fromX,fromY, 0);
			
			float toX = GetHorizontalPos( (float) cell.RowNum) + cellWidth / 2;
			float toY = GetVerticalPos( (float) cell.ColNum ) + cellHeight / 2;

			Gizmos.DrawWireSphere(new Vector3(fromX, 0, 0), cellWidth);
			Gizmos.DrawWireSphere(new Vector3(0, fromY, 0), cellHeight);*/
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
