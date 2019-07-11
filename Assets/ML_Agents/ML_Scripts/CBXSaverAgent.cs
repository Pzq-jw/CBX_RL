using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CBXSaverAgent : Agent
{
	public Transform mlTarget;
	public Transform monitor;
	public Transform mlColumn;
	public Piece pieceObj;

	private int thinkingTime;
	private int leftLife;
	private int totalPieceNum;
	private float delta;
		
	private List<float> perceptionBuffer = new List<float>();

	public float deltaDegree
	{
		get 
		{
			if(delta > 0.5)
			{
				return 0.5f + delta/2.8f;
			}
			else
			{
				return delta;
			}
		}
	}

	public override void AgentReset()
	{
		thinkingTime = Random.Range(0, 10);
		// leftLife = Mathf.FloorToInt(Random.Range(1, 3));
		// totalPieceNum = Mathf.FloorToInt(Random.Range(0, 1000));
		GameControl.instance.columnObj.amplitudeIncrement = Random.Range(0, 2);
		GameControl.instance.columnObj.amplitudeRotate = Random.Range(7, 15);

		ResetEnv();

	}

	public override void CollectObservations()
	{
		// AddVectorObs(this.transform.position);
		// AddVectorObs(mlTarget.position);
  //       AddVectorObs(mlTarget.rotation.z);
  //       AddVectorObs(mlColumn.position.x);
		delta = Mathf.Abs(this.transform.position.x - mlTarget.transform.position.x);
		float[] diffFactors = {thinkingTime, 
								delta,
								GameControl.instance.columnObj.amplitudeRotate};
		string[] detectableObjects = {"easy", "medium", "difficult"};
		AddVectorObs(EvaluateDiffLvl(diffFactors, detectableObjects));
		Debug.Log("delta = " + delta + " time = " + thinkingTime + " rotate = " + GameControl.instance.columnObj.amplitudeRotate);
	}

	public List<float> EvaluateDiffLvl(float[] diffFactors, string[] diffLvl)
	{
		perceptionBuffer.Clear();
		for(int factorIdx=0; factorIdx<diffFactors.Length; factorIdx++)
		{
			float[] subList = new float[diffLvl.Length + 2];
			switch(factorIdx)
			{
				case 0:
					EvaluateTimeDiffLvl(diffFactors[factorIdx], subList, diffLvl);
					break;
				case 1:
					EvaluateDeltaDiffLvl(diffFactors[factorIdx], subList, diffLvl);
					break;
				case 2:
					EvaluateRotateDiffLvl(diffFactors[factorIdx], subList, diffLvl);
					break;
			}
			perceptionBuffer.AddRange(subList);
		}
		return perceptionBuffer;
	}

	private void EvaluateRotateDiffLvl(float factor, float[] subList, string[] diffLvl)
	{
		for(int i=0; i<diffLvl.Length; i++)
		{
			if(string.Equals(GetTimeLvl(factor), diffLvl[i]))
			{
				subList[i] = 1;
				subList[diffLvl.Length + 1] = factor / 15f;
				break;
			}
		}
	}

	private void EvaluateTimeDiffLvl(float factor, float[] subList, string[] diffLvl)
	{
		for(int i=0; i<diffLvl.Length; i++)
		{
			if(string.Equals(GetTimeLvl(factor), diffLvl[i]))
			{
				subList[i] = 1;
				subList[diffLvl.Length + 1] = factor / 10f;
				break;
			}
		}
	}

	private void EvaluateDeltaDiffLvl(float factor, float[] subList, string[] diffLvl)
	{
		for(int i=0; i<diffLvl.Length; i++)
		{
			if(string.Equals(GetDeltaLvl(factor), diffLvl[i]))
			{
				subList[i] = 1;
				subList[diffLvl.Length + 1] = deltaDegree;
				break;
			}
		}
	}

	private string GetRotateLvl(float rotate)
	{
		if(rotate < 10f && rotate >= 7f)
			return "easy";
		else if(rotate < 12f)
			return "medium";
		else if(rotate <= 15f)
			return "difficult";
		else
			return null;
	}

	private string GetDeltaLvl(float delta)
	{
		if(delta <= 0.1f)
			return "easy";
		else if(delta <= 0.5f)
			return "medium";
		else if(delta < 2.8f)
			return "difficult";
		else
			return null;
	}

	public string GetTimeLvl(float elapsedTime)
	{
		if(elapsedTime < 1.2f)
			return "easy";
		else if(elapsedTime < 7)
			return "medium";
		else if(elapsedTime <= 10)
			return "difficult";
		else 
			return null;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int state = GetCurrentState();
		int tuneSignal = Mathf.FloorToInt(vectorAction[0]);
		Debug.Log("current state = " + state);
		Debug.Log("tuneSignal = " + tuneSignal);
		if(tuneSignal == 0)
		{
			AddReward(-0.001f);
		}
		else if(tuneSignal == 1)  // turn to more difficult
		{
			if(state == 1)
			{
				AddReward(1f);
			}
			else
			{
				AddReward(-0.1f);
			}
			Done();
		}
		else if(tuneSignal == 2) // turn to easier
		{
			if(state == 2)
			{
				AddReward(1f);
			}
			else
			{
				AddReward(-0.1f);
			}
			Done();
		}
	}

	private int GetCurrentState()
	{
		float exp = 0.2f * (thinkingTime / 10f) + 0.7f * deltaDegree
					+ 0.1f * (GameControl.instance.columnObj.amplitudeRotate / 15f);
		if(exp < 0.2)
			return 1; // easy
		else if(exp < 0.7)
			return 0; // medium
		else if(exp <= 1)
			return 2; // difficult
		else
			return 0;
	}

    public void ResetEnv()
    {
        // GameControl.instance.slingObj.angle = Random.Range(0f, 360f);
        this.transform.position = new Vector3(Random.Range(-1.3f, 1.3f), -1.2f, 0);
    }

    public void ComputeReward()
    {
        Debug.Log("this.localX = " + this.transform.localPosition.x.ToString());
        Debug.Log("target,localX = " + mlTarget.transform.localPosition.x.ToString());
    	float absDelta = Mathf.Abs(this.transform.localPosition.x - mlTarget.transform.localPosition.x);
    }
}
