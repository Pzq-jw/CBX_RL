using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMLArea : MonoBehaviour
{
	public float rootPosOffsetX = 10;
	public float rootPosOffsetY = 0;
	public float rootPosOffsetZ = 10;

	public Vector3 rootPos;

	void Start()
	{
		rootPos = this.transform.position;
		rootPosOffsetX = rootPos.x;
		rootPosOffsetY = rootPos.y;
		rootPosOffsetZ = rootPos.z; 
	}
}
