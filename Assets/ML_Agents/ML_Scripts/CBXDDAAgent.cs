using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.IO;

public class CBXDDAAgent : Agent
{
	public Transform mlTarget;
	public Transform mlColumn;
	public Piece pieceObj;

	private int thinkingTime;
	private int leftLife;
	private int totalPieceNum;
	private int isAIHelpLastTime;
	private float delta;
	private int curGameStatus; 
    public int cor_count, total_count;
    public string s, t;
    public int isCorrect;
		
	private List<float> perceptionBuffer = new List<float>();

	public override void AgentReset()
	{
		thinkingTime = Random.Range(0, 16);
		GameControl.instance.columnObj.amplitudeRotate = Random.Range(7, 15);
		// leftLife = Random.Range(0, 4);
		// totalPieceNum = Random.Range(0, 1001);
		// isAIHelpLastTime = Random.Range(0, 2);

		// Debug.Log("rotate = " + GameControl.instance.columnObj.amplitudeRotate);

		ResetEnv();

	}

	public override void CollectObservations()
	{
		delta = Mathf.Abs(this.transform.localPosition.x - mlTarget.localPosition.x);
		// Debug.Log("this.x = " + this.transform.localPosition.x.ToString());
		// Debug.Log("target.x = " + mlTarget.localPosition.x.ToString());
		// Debug.Log("delta = " + delta.ToString());
		float[] diffFactors = {thinkingTime, 
								delta,
								GameControl.instance.columnObj.amplitudeRotate};
		string[] detectableObjects = {"easy", "medium", "difficult"};
		AddVectorObs(EvaluateDiffLvl(diffFactors, detectableObjects));
		AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);
		// Debug.Log("delta = " + delta + " time = " + thinkingTime + " rotate = " + GameControl.instance.columnObj.amplitudeRotate);
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
			if(string.Equals(GetRotateLvl(factor), diffLvl[i]))
			{
				subList[i] = 1;
				subList[diffLvl.Length + 1] = (factor-7) / 8f;
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
				subList[diffLvl.Length + 1] = factor / 15f;
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
		if(elapsedTime < 2.5f)
			return "easy";
		else if(elapsedTime < 7.5)
			return "medium";
		else if(elapsedTime <= 15)
			return "difficult";
		else 
			return null;
	}

	public float deltaDegree
	{
		get 
		{
			if(delta < 0.1)
			{
				return delta;
			}
			else if(delta <= 0.5)
			{
				return delta * 2;
			}
			else
			{
				return 1;
			}
		}
	}

	private int GetCurrentState()
	{
		float exp = 0.2f * (thinkingTime / 15f) + 0.7f * deltaDegree
					+ 0.1f * ((GameControl.instance.columnObj.amplitudeRotate-7f) / 8f);
		if(exp < 0.2)
			return 1; // easy
		else if(exp < 0.75)
			return 0; // medium
		else if(exp <= 1)
			return 2; // difficult
		else
			return 0;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int state = GetCurrentState();
		int turnSignal = Mathf.FloorToInt(vectorAction[0]);
		// Debug.Log("current state = " + state);
		// Debug.Log("tuneSignal = " + turnSignal);
		Accuracy(state, turnSignal);
		if(turnSignal == 0)
		{
			AddReward(-1f/agentParameters.maxStep);
			AgentReset();
		}
		else if(turnSignal == 1)  // turn to more difficult
		{
			if(state == 1)
			{
				SetReward(1f);
			}
			else
			{
				SetReward(-1f);
			}
			Done();
		}
		else if(turnSignal == 2) // turn to easier
		{
			if(state == 2)
			{
				SetReward(1f); 
			}
			else
			{
				SetReward(-1f); 
				// try more negative reward than for listen action
				// Agent MaxStep can try to reduce
			}
			Done();
		}
	}

    public void ResetEnv()
    {
        this.transform.localPosition = new Vector3(Random.Range(-1.3f, 1.3f), 0.62f, 0);
        mlTarget.transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), 4.3f, 0);
        mlColumn.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), -5f, 0);
        mlColumn.localRotation = Quaternion.Euler(new Vector3(0,0,
        		Random.Range(0f, GameControl.instance.columnObj.amplitudeRotate)));
    }

    public void Accuracy(int curState, int turnSignal)
    {
		using(StreamWriter sw = new StreamWriter("dda_rl_accuracy.txt", true))
		{
	    	total_count++;

	    	if(curState == 1)
	    	{
	    		s = "easy";
	    		if(turnSignal == 1)
	    		{
	    			t = "turn_diff";
	    			cor_count++;
	    			isCorrect = 1;
	    		}
	    		else if(turnSignal == 0)
	    		{
	    			t = "turn_medi";
	    			isCorrect = 0;
	    		}
	    		else if(turnSignal == 2)
	    		{
	    			t = "turn_easy";
	    			isCorrect = 0;
	    		}
	    	}
	    	else if(curState == 0)
	    	{
	    		s = "medi";
	    		if(turnSignal == 1)
	    		{
	    			t = "turn_diff";
	    			isCorrect = 0;
	    		}
	    		else if(turnSignal == 0)
	    		{
	    			t = "turn_medi";
	    			cor_count++;
	    			isCorrect = 1;
	    		}
	    		else if(turnSignal == 2)
	    		{
	    			t = "turn_easy";
	    			isCorrect = 0;
	    		}
	    	}
	    	else if(curState == 2)
	    	{
	    		s = "diff";
	    		if(turnSignal == 1)
	    		{
	    			t = "turn_diff";
	    			isCorrect = 0;   			
	    		}
	    		else if(turnSignal == 0)
	    		{
	    			t = "turn_medi";
	    			isCorrect = 0;    			
	    		}
	    		else if(turnSignal == 2)
	    		{
	    			t = "turn_easy";
	    			isCorrect = 1;
	    			cor_count++;
	    		}
	    	}
		
			sw.WriteLine(
			 isCorrect + "\t" + delta.ToString("F2") + "\t" + thinkingTime + "\t" +  
			 GameControl.instance.columnObj.amplitudeRotate + "\t" +
			 s + "\t" + t + "\t" + cor_count);
		}

    }
}
