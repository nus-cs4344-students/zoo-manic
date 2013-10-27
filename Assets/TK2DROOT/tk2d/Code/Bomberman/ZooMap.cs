using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ZooMap : MonoBehaviour {
	
	//float heightDimension = 100f;
	//float widthDimension = 138.0f;
	
	// Number of cells
	public static float horizontalCell = 38.0f;		
	public static float verticalCell = 28.0f;
	
	// Cell dimensions
	static float cellWidth = 3.40f;
	static float cellHeight = 3.0f;
	
	bool isCellCreated = false;
	
	[SerializeField] GameObject woodenCratePrefab;	
	[SerializeField] GameObject rockPrefab;	
	
	[SerializeField] Transform m_spawnPoint1;	
	[SerializeField] Transform m_spawnPoint2;
	[SerializeField] Transform m_spawnPoint3;
	[SerializeField] Transform m_spawnPoint4;	
	
	//horizontalCellNum, verticalCellNum, type, item
	

	// Store the cells info in a dictionary for efficiency
	// cellNum, object{type, item,x, y}
	// the game object here refers to the powerup, or the brick etc
	private static Dictionary<int, Cell> zooMapInfoDict = new Dictionary<int, Cell>();		// Static so it won't disappear when scene change
	//private Dictionary<int, List<GameObject>> zooMapItemObjectDict = new Dictionary<int, List<GameObject>>();
	
	public static float GetHorizontalCell(float horizontalPos)
	{
		return (horizontalPos - cellWidth) / cellWidth;
	}
	
	public static float GetVerticalCell(float verticalPos)
	{
		return (verticalPos - cellHeight)  / cellHeight;
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
	
	// Use this for initialization
	void Awake()
	{
		InitZooMap();
		InitSpawnPoint();
		Debug.Log ("Zoo Map Created! Map Size: "+zooMapInfoDict.Count);
		
		// Make this game object and all its transform children
		// survive when loading a new scene.
		DontDestroyOnLoad (transform.gameObject);
	}
	
	void Start () 
	{
		
	}
	
	private void InitSpawnPoint()
	{
		m_spawnPoint1.localPosition = new Vector3(GetHorizontalPos(0), GetVerticalPos(0), 0);
		m_spawnPoint2.localPosition = new Vector3(GetHorizontalPos(horizontalCell-1), GetVerticalPos(0), 0);
		m_spawnPoint3.localPosition = new Vector3(0, GetVerticalPos(verticalCell-1), 0);
		m_spawnPoint4.localPosition = new Vector3(GetHorizontalPos(horizontalCell-1), GetVerticalPos(verticalCell-1), 0);
	}
	
	private void InitZooMap()
	{
		// type  empty - 0, box - 1, rock - 2
		// item  no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
		for(int index=0; index< (int) ZooMap.horizontalCell * ZooMap.verticalCell; index++)
		{
			zooMapInfoDict.Add (index, new Cell());
		}
	}
	
	/*List<T> CreateList<T>(params T[] values)
	{
    	return new List<T>(values);
	}*/
	
	// Cell number is from 0 to 27*37
	// type  empty - 0, box - 1, rock - 2
	// item  no item - 0, 1 - bomb range, 2 - haste, 3 - invunerable, 4 - more bombs, 5 - shakable
	public void UpdateZooMap(long cellType, long cellItem, long horizontalCellNum, long verticalCellNum, int cellNum)
	{	
		if(zooMapInfoDict == null || zooMapInfoDict.Count == 0)
		{
			Debug.Log ("Map has not been INITIALISED");
			return;
		}
		
		Cell currCell = (Cell) zooMapInfoDict[cellNum];
		
		// If server says add wooden crate, but your side has no crate
		if(cellType == 1 && currCell.CellType != 1)
		{
			// Add the wooden crate into the scene
			GameObject woodenObj = Instantiate(woodenCratePrefab, 
			new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			
			currCell.CellType = 1;
		}
		else if(cellType == 2 && currCell.CellType != 2)
		{
			GameObject rockObj = Instantiate(rockPrefab, 
			new Vector3( GetHorizontalPos( (float)horizontalCellNum) , GetVerticalPos( (float)verticalCellNum),0), transform.rotation) as GameObject;
			
			currCell.CellType = 2;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
