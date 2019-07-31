using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GO_Extensions;

public class ColumnSwinging : MonoBehaviour
{

	public float amplitudeMove = 0.5f;
	public float amplitudeRotate;
	public float comboAmplitudeRotate = 1f;
	public float amplitudeIncrement = 0.5f;
	public float maxAmplitudeRotate = 15f;
	public Rigidbody2D rb2d;

	// public float idealDistanceColumn2Sling; //Todo: use this control distance
	public float columnHeightIncrement;

	private float angle;
	private float angularSpeed = 1f;

	public float timeOffset;
	public bool isReset = false;
	public bool isDDA = false;
	private float _t = 0;

	void Start()
	{
		angle = 0;
		rb2d = GetComponent<Rigidbody2D>();	
	}

	public void ResetColumnPos()
	{
		isReset = true;
		timeOffset = Random.Range(0, 6.28f);
		// amplitudeRotate = Random.Range(5f, 15f);
	}

	void FixedUpdate()
	{
		if(!isDDA)
		{
			_t += Time.fixedDeltaTime * angularSpeed;
			if(isReset)
			{
				_t -= timeOffset;
				isReset = false;
			}
			
			transform.localPosition = new Vector3(
				Mathf.Sin(_t) * amplitudeMove, 
				transform.localPosition.y, 
				0f);

			transform.localRotation = Quaternion.Euler(
				0,
				0,
				-Mathf.Sin(_t)*amplitudeRotate);
		}
	}

	public void SwingingCenterMoveUp()
	{
		if(GameControl.instance.stackedPieceNum >= GameControl.instance.piecePoolObj.piecePoolSize - 1)
		{
			Vector3 _pos = transform.position;
			_pos.y += columnHeightIncrement;
			transform.MoveOnlyParent(_pos);
		}
	}

	public void AddAmplitudeRotate()
	{
		if(amplitudeRotate < maxAmplitudeRotate)
		{
			amplitudeRotate += amplitudeIncrement;
		}
	}

	public void SetAmplitudeIncrementAndMax()
	{
		int totalPieces = GameControl.instance.populationScore;
		if(totalPieces < 100){
			amplitudeIncrement = 0.5f;
			maxAmplitudeRotate = 7f;
			comboAmplitudeRotate = 1f;
		}
		else if(totalPieces < 200){
			amplitudeIncrement = 1f;
			maxAmplitudeRotate = 10f;
			comboAmplitudeRotate = 2f;
		}
		else if(totalPieces < 300){
			amplitudeIncrement = 1.5f;
			maxAmplitudeRotate = 13f;
			comboAmplitudeRotate = 3f;
		}
		else{
			amplitudeIncrement = 2f;
			maxAmplitudeRotate = 15f;
			comboAmplitudeRotate = 5f;
		}
	}

	public void Set2ComboSwingingAmplitude()
	{
		if(amplitudeRotate > comboAmplitudeRotate * 2)
		{
			amplitudeRotate -= comboAmplitudeRotate;
		}
		else
		{
			amplitudeRotate = comboAmplitudeRotate;			
		}
	}

	public float GetCenterPostion()
	{
		return transform.GetCenterPosition(1);
	}

	public void FlashColumnOnCombo()
	{

	}
}
