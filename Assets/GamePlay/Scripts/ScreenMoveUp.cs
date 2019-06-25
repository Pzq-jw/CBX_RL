using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenMoveUp : MonoBehaviour
{

	public GameObject skyObj_1;
	public GameObject skyObj_2;
	public GameObject cameraObj;

	public float pieceSize = 1f;
	public float backgroundHeight = 10f;
	public float moveSpeed = 2f;

	private float slingMoveStep;
	private Vector3 cameraDestination;
	private float slingPosYOffsetDestination;

	public Vector3 cameraPos
	{
		get {return cameraObj.transform.position;}
	}

	void FixedUpdate()
	{
		if(GameControl.instance.isGameRunning)
		{
			cameraObj.transform.position = Vector3.MoveTowards(
				cameraObj.transform.position, cameraDestination, moveSpeed*Time.fixedDeltaTime);
			GameControl.instance.slingObj.offsetY = Mathf.MoveTowards(
				GameControl.instance.slingObj.offsetY, slingPosYOffsetDestination, moveSpeed*Time.fixedDeltaTime);
		}
		
	}

	public void MoveUp()
	{
		BackgroundMoveUp();
		SetCameraMoveDestination();
		SetSlingMoveDestination();
	}

	private void SetCameraMoveDestination()
	{
		cameraDestination = cameraObj.transform.position;
		cameraDestination.y += GameControl.instance.columnObj.columnHeightIncrement;
	}

	private void SetSlingMoveDestination()
	{
		slingPosYOffsetDestination = GameControl.instance.slingObj.offsetY;
		slingPosYOffsetDestination += GameControl.instance.columnObj.columnHeightIncrement;
	}

	private void BackgroundMoveUp()
	{
		Vector3 posSkyObj_1 = skyObj_1.transform.position;
		Vector3 posSkyObj_2 = skyObj_2.transform.position;
		Vector3 posCamera = cameraObj.transform.position;

		if(posSkyObj_1.y < posSkyObj_2.y)
		{
			if(posCamera.y - posSkyObj_2.y >= 0)
			{
				posSkyObj_1.y += 2 * backgroundHeight;
				skyObj_1.transform.position = posSkyObj_1;
			}
		}
		else
		{
			if(posCamera.y - posSkyObj_1.y >= 0)
			{
				posSkyObj_2.y += 2 * backgroundHeight;
				skyObj_2.transform.position = posSkyObj_2;
			}
		}
	}

	public void ShakeCamera()
	{
		cameraObj.transform.DOShakePosition(0.5f, new Vector3(0.1f, 0.1f, 0), 50, 60);
	}














}
