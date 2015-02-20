using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

//[ExecuteInEditMode]
public class SpaceGridObject : MonoBehaviour {

	public bool DEBUG_ALL;
	public bool DEBUG_BLOCKED;
	public bool DEBUG_BOUNDS;

	public SpaceCell[,,] Cells;
	public float Margin;
	public byte NumCellsX;
	public byte NumCellsY;
	public byte NumCellsZ;
	public float CellSize = 1;

	public Vector3 Center { get; private set; }

	public Bounds GridBounds { get { return gridBounds; } }
	public Vector3 GridPostion;
	public Vector3 GridBoundsCenter;
	public Vector3 GridBoundsSize;

	Vector3 tmpVecPos;
	Vector3 tmpVecSize;
	Bounds gridBounds;
	
	void Start()
	{
		Cells = new SpaceCell[NumCellsX, NumCellsY, NumCellsZ];
		//Debug.Log("-----------------------------------------");
		byte i, j, k = 0;
		for (i = 0; i < NumCellsX; i++) {
			for (j = 0; j < NumCellsY; j++) {
				for (k = 0; k < NumCellsZ; k++) {
					SpaceCell tmpCell = new SpaceCell (i, j, k);
					Cells[i, j, k] = tmpCell;
				}
			}
		}
		GridPostion = transform.position - transform.localScale / 2;

		GridBoundsSize = new Vector3(NumCellsX * CellSize + (NumCellsX * Margin), 
		                             NumCellsY * CellSize + (NumCellsY * Margin),
		                             NumCellsZ * CellSize + (NumCellsZ * Margin));

		GridBoundsCenter = GridPostion + GridBoundsSize / 2f;

		Center = GridBoundsCenter;

		gridBounds = new Bounds(GridBoundsCenter, GridBoundsSize);
	}

	void Update()
	{
	}

	void OnDrawGizmos() {
		if(Cells != null && (DEBUG_ALL || DEBUG_BLOCKED))
			foreach(SpaceCell cell in Cells){
				if(cell != null)
					DrawLines(cell);
			}

		if(DEBUG_BOUNDS){
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(GridBoundsCenter, GridBoundsSize);
		}
	}

	public SpaceCell GetRandomFreeCell(){
		SpaceCell cell = Cells [UnityEngine.Random.Range(0, NumCellsX), UnityEngine.Random.Range(0, NumCellsY), UnityEngine.Random.Range(0, NumCellsZ)];
		if(cell.IsBlocked)
			return GetRandomFreeCell();

		return cell;
	}

	public Bounds GetCellBoundsAtPosition(float posX, float posY, float posZ){
		SpaceCell cell = GetCellAtPos (posX, posY, posZ);
		return new Bounds (new Vector3(	(float)cell.XIndex * CellSize + CellSize / 2f + (Margin * cell.XIndex), 
		                               	(float)cell.YIndex * CellSize + CellSize / 2f + (Margin * cell.YIndex),
		                               	(float)cell.ZIndex * CellSize + CellSize / 2f + (Margin * cell.ZIndex)) + GridPostion,
		                   new Vector3(	CellSize, CellSize, CellSize));
	}

	public Bounds GetCellBoundsAtIndex(int x, int y, int z){
		SpaceCell cell = GetCellAtIndex (x, y, z);
		return new Bounds (new Vector3(	(float)cell.XIndex * CellSize + CellSize / 2f + (Margin * cell.XIndex), 
		                               	(float)cell.YIndex * CellSize + CellSize / 2f + (Margin * cell.YIndex),
		                               	(float)cell.ZIndex * CellSize + CellSize / 2f + (Margin * cell.ZIndex))  + GridPostion,
		                   new Vector3(	CellSize, CellSize, CellSize));
	}
	
	public Vector3 GetCellPositionAtPostion(float posX, float posY, float posZ){
		SpaceCell cell = GetCellAtPos (posX, posY, posZ);
		return new Vector3(	(float)cell.XIndex * CellSize + CellSize / 2f + (Margin * cell.XIndex), 
		                   	(float)cell.YIndex * CellSize + CellSize / 2f + (Margin * cell.YIndex),
		                   	(float)cell.ZIndex * CellSize + CellSize / 2f + (Margin * cell.ZIndex)) + GridPostion;
	}

	public Vector3 GetCellPositionAtIndex(int x, int y, int z){
		SpaceCell cell = GetCellAtIndex (x, y, z);
		return new Vector3(	(float)cell.XIndex * CellSize + CellSize / 2f + (Margin * cell.XIndex), 
		                   	(float)cell.YIndex * CellSize + CellSize / 2f + (Margin * cell.YIndex),
		                   	(float)cell.ZIndex * CellSize + CellSize / 2f + (Margin * cell.ZIndex)) + GridPostion;
	}

	public SpaceCell GetCellAtPos (float posX, float posY, float posZ)
	{
		int x = (int)Mathf.Floor (posX / CellSize);
		int y = (int)Mathf.Floor (posY / CellSize);
		int z = (int)Mathf.Floor (posZ / CellSize);

		if(!SpaceCell.IsIndexValid(x, y ,z))
			return null;

		if ((x >= NumCellsX) || (y >= NumCellsY) || (z >= NumCellsZ))
			return null;
		
		return Cells [x, y, z];
	}
	
	public SpaceCell GetCellAtPos (Vector3 pos)
	{	
		return GetCellAtPos (pos.x, pos.y, pos.z);
	}
	
	public SpaceCell GetCellAtIndex (int x, int y, int z)
	{
		if(!SpaceCell.IsIndexValid(x, y ,z))
			return null;
		
		if ((x >= NumCellsX) || (y >= NumCellsY) || (z >= NumCellsZ))
			return null;

		return Cells [x, y, z];
	}

	public void DrawLines(Vector3 pos)
	{
		SpaceCell tmpCell = GetCellAtPos(pos);
		DrawLines(tmpCell);
	}
	
	public void DrawLines(int x, int y, int z)
	{
		SpaceCell tmpCell = GetCellAtIndex(x, y, z);
		DrawLines(tmpCell);
	}

	public void DrawLines(SpaceCell cell)
	{
		if(cell != null){
			tmpVecPos.x = (float)cell.XIndex * CellSize + CellSize / 2f + (Margin * cell.XIndex);
			tmpVecPos.y = (float)cell.YIndex * CellSize + CellSize / 2f + (Margin * cell.YIndex);
			tmpVecPos.z = (float)cell.ZIndex * CellSize + CellSize / 2f + (Margin * cell.ZIndex);
			tmpVecSize.x = CellSize;
			tmpVecSize.y = CellSize;
			tmpVecSize.z = CellSize;

			if(DEBUG_BLOCKED && cell.IsBlocked){
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(tmpVecPos, tmpVecSize);
			} else if(DEBUG_ALL){
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(tmpVecPos, tmpVecSize);
			}
		}
	}
}

[System.Serializable]
public class SpaceCell : IEquatable<SpaceCell>
{			
	public byte XIndex;
	public byte YIndex;
	public byte ZIndex;

	public bool IsBlocked;

	public SpaceCell (byte xindex, byte yindex, byte zindex)
	{
		XIndex = xindex;
		YIndex = yindex;
		ZIndex = zindex;
	}

	public static bool IsValid (SpaceCell cell)
	{
		if ((cell.XIndex < 0) || (cell.YIndex < 0) || (cell.ZIndex < 0)) {
			return false;
		}
		
		return true;
	}
	
	public static bool IsIndexValid (int x, int y, int z)
	{
		if ((x < 0) || (y < 0 ) || (z < 0 )) {
			return false;
		}

		return true;
	}

	public override int GetHashCode()
	{
		return -1;
	}

	public override bool Equals( object obj )
	{
		var other = obj as SpaceCell;
		if( other == null ) return false;
		
		return Equals (other);             
	}

	public bool Equals (SpaceCell other)
	{
		if( other == null ){
			return false;
		}

		if ((XIndex == other.XIndex) && (YIndex == other.YIndex) && (ZIndex == other.YIndex)) {
			return true;
		}
		
		return false;
	}
}