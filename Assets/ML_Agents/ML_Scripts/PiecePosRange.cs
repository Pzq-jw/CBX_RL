using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "New PieceData", menuName = "Piece Data", order = 51)]
public class PiecePosRange : ScriptableObject
{
	[SerializeField]
	private float min_x;
	[SerializeField]
	private float max_x;
	[SerializeField]
	private float min_y;
	[SerializeField]
	private float max_y;
	[SerializeField]
	private float min_rot;
	[SerializeField]
	private float max_rot;

	public float minPosX
	{
		get {return min_x;}
	}

	public float maxPosX
	{
		get {return max_x;}
	}

	public float minPosY
	{
		get {return min_y;}
	}

	public float maxPosY
	{
		get {return max_y;}
	}

	public float rot
	{
		get {return max_rot;}
	}
	
	public float posRangeX
	{
		get {return max_x - min_x;}
	}

	public float posRangeY
	{
		get {return max_y - min_y;}
	}
}
