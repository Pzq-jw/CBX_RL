using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public static class GO_Extensions
{

	public static void PositionToLastChild(this Transform aParent)
	{
		var childs = aParent.Cast<Transform>().ToList();
		float pos = childs.Min(c => c.position.y);
		foreach(var c in childs)
		{
			c.parent = null;
		}
		aParent.position = new Vector3(0, pos, 0);
		foreach(var c in childs)
		{
			c.parent = aParent;
		}
	}

	public static void MoveOnlyParent(this Transform aParent, Vector3 _pos)
	{
		var childs = aParent.Cast<Transform>().ToList();
		foreach(var c in childs)
		{
			c.parent = null;
		}
		aParent.position = _pos;
		foreach(var c in childs)
		{
			c.parent = aParent;
		}
	}

	public static float GetCenterPosition(this Transform aParent, int axis)
	{
		var childs = aParent.Cast<Transform>().ToList();
		float posAxis = 0f;
		foreach(var c in childs)
		{
			// Debug.Log("piece name = " + c.gameObject.name + "pos = " + c.localPosition[axis], c.gameObject);
			posAxis += c.localPosition[axis];
		}
		// Debug.Log("center pos = " +  posAxis / childs.Count + "childs.count = " + childs.Count);
		// Debug.Log("column pos = " + aParent.transform.position);
		return posAxis / childs.Count;
	}

	public static void SetAllChildsRotation(this Transform aParent, int axis, float eularAngle, bool isReset)
	{
		var childs = aParent.Cast<Transform>().ToList();
		foreach(var c in childs)
		{
			var rot = c.transform.localEulerAngles;
			if(isReset)
				rot[axis] = 0;
			else
				rot[axis] += eularAngle;				
			c.transform.localRotation = Quaternion.Euler(rot);
			// Debug.Log("c.transform.localRotation = " + c.transform.localEulerAngles);
			// Debug.Log("c.transform.rotation = " + c.transform.eulerAngles);
		}
	}

	public static string GetArg(this Transform aParent, string argName)
	{
		string[] args = System.Environment.GetCommandLineArgs();
		for(int i=0; i<args.Length; i++)
		{
			if(args[i] == argName && args.Length > i+1)
			{
				return args[i+1];
			}
		}
		return null;
	}
}
