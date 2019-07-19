using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.IO;

public class CBXSaverAgent : Agent
{
	public Transform mlTarget;
	public Transform monitor;
	public Transform mlColumn;
	public Piece pieceObj;

	private int leftLife;
	private int totalPieceNum;
	private int isAIHelpLastTime;
	private int gameDiffLvl;
	private int curGameStatus;

    public int cor_count, total_count;
    public string s, t;
    public int isCorrect;
		
	private List<float> perceptionBuffer = new List<float>();

	public override void AgentReset()
	{
		leftLife = Random.Range(0, 4);
		totalPieceNum = Random.Range(0, 1001);
		isAIHelpLastTime = Random.Range(0, 2);
	}

	public override void CollectObservations()
	{
		int[] situFactors = {leftLife, 
								totalPieceNum,
								gameDiffLvl};
		string[] detectableObjects = {"pleasant", "normal", "dangerous"};
		AddVectorObs(EvaluatesituType(situFactors, detectableObjects));
		AddVectorObs(isAIHelpLastTime);
		AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);
		// Debug.Log("delta = " + delta + " time = " + thinkingTime + " rotate = " + GameControl.instance.columnObj.amplitudeRotate);
	}

	public List<float> EvaluatesituType(int[] situFactors, string[] situType)
	{
		perceptionBuffer.Clear();
		for(int factorIdx=0; factorIdx<situFactors.Length; factorIdx++)
		{
			float[] subList = new float[situType.Length + 2];
			switch(factorIdx)
			{
				case 0:
					EvaluateLeftLifeSituation(situFactors[factorIdx], subList, situType);
					break;
				case 1:
					EvaluateTotalPieceNumSituType(situFactors[factorIdx], subList, situType);
					break;
				case 2:
					EvaluateGameDiffLvlSituType(situFactors[factorIdx], subList, situType);
					break;
			}
			perceptionBuffer.AddRange(subList);
		}
		return perceptionBuffer;
	}

	private void EvaluateLeftLifeSituation(int factor, float[] subList, string[] situType)
	{
		for(int i=0; i<situType.Length; i++)
		{
			if(string.Equals(GetLeftLifeSituType(factor), situType[i]))
			{
				subList[i] = 1;
				subList[situType.Length + 1] = factor / 3;
				break;
			}
		}
	}

	public string GetLeftLifeSituType(int leftLife)
	{
		if(leftLife == 3)
			return "pleasant";
		else if(leftLife == 2)
			return "normal";
		else if(leftLife == 1)
			return "dangerous";
		else 
			return null;
	}

	private void EvaluateTotalPieceNumSituType(float factor, float[] subList, string[] situType)
	{
		for(int i=0; i<situType.Length; i++)
		{
			if(string.Equals(GetTotalPieceNumSituType(factor), situType[i]))
			{
				subList[i] = 1;
				subList[situType.Length + 1] = factor / 300;
				break;
			}
		}
	}

	private string GetTotalPieceNumSituType(float num)
	{
		if(num <= 100)
			return "pleasant";
		else if(num <= 200)
			return "normal";
		else 
			return "dangerous";
	}

	private void EvaluateGameDiffLvlSituType(float factor, float[] subList, string[] situType)
	{
		for(int i=0; i<situType.Length; i++)
		{
			if(string.Equals(GetGameDiffLvlSituType(factor), situType[i]))
			{
				subList[i] = 1;
				subList[situType.Length + 1] = (factor-7) / 8f;
				break;
			}
		}
	}

	private string GetGameDiffLvlSituType(float lvl)
	{
		if(lvl == 0)
			return "pleasant";
		else if(lvl == 1)
			return "normal";
		else if(lvl == 2)
			return "dangerous";
		else
			return null;
	}

	private int GetCurrentSituation()
	{
		float exp = 0.3f * (leftLife / 3) 
					+ 0.3f * (totalPieceNum / 300)
					+ 0.3f * (gameDiffLvl / 2)
					+ 0.1f * isAIHelpLastTime;
		if(exp < 0.3)
			return 0; // pleasant
		else if(exp < 0.6)
			return 1; // normal
		else if(exp <= 1)
			return 3; // dangerous
		else
			return 0;
	}

	public void SetGameDiffLvl(int turnSignal)
	{
		if(turnSignal == 1)
			gameDiffLvl = 0;
		else if(turnSignal == 2)
			gameDiffLvl = 2;
		else 
			gameDiffLvl = 0;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int situation = GetCurrentSituation();
		int saveDegree = Mathf.FloorToInt(vectorAction[0]);
		AddReward(-1f/agentParameters.maxStep);
		// Debug.Log("current situation = " + situation);
		// Debug.Log("saveSignal = " + saveSignal);
		// Accuracy(situation, saveSignal);
		if(situation == saveDegree)
			SetReward(1f);
		else
			SetReward(-1f);
	}

  //   public void Accuracy(int curState, int turnSignal)
  //   {
		// using(StreamWriter sw = new StreamWriter("dda_rl_accuracy.txt", true))
		// {
	 //    	total_count++;

	 //    	if(curState == 1)
	 //    	{
	 //    		s = "easy";
	 //    		if(turnSignal == 1)
	 //    		{
	 //    			t = "turn_diff";
	 //    			cor_count++;
	 //    			isCorrect = 1;
	 //    		}
	 //    		else if(turnSignal == 0)
	 //    		{
	 //    			t = "turn_medi";
	 //    			isCorrect = 0;
	 //    		}
	 //    		else if(turnSignal == 2)
	 //    		{
	 //    			t = "turn_easy";
	 //    			isCorrect = 0;
	 //    		}
	 //    	}
	 //    	else if(curState == 0)
	 //    	{
	 //    		s = "medi";
	 //    		if(turnSignal == 1)
	 //    		{
	 //    			t = "turn_diff";
	 //    			isCorrect = 0;
	 //    		}
	 //    		else if(turnSignal == 0)
	 //    		{
	 //    			t = "turn_medi";
	 //    			cor_count++;
	 //    			isCorrect = 1;
	 //    		}
	 //    		else if(turnSignal == 2)
	 //    		{
	 //    			t = "turn_easy";
	 //    			isCorrect = 0;
	 //    		}
	 //    	}
	 //    	else if(curState == 2)
	 //    	{
	 //    		s = "diff";
	 //    		if(turnSignal == 1)
	 //    		{
	 //    			t = "turn_diff";
	 //    			isCorrect = 0;   			
	 //    		}
	 //    		else if(turnSignal == 0)
	 //    		{
	 //    			t = "turn_medi";
	 //    			isCorrect = 0;    			
	 //    		}
	 //    		else if(turnSignal == 2)
	 //    		{
	 //    			t = "turn_easy";
	 //    			isCorrect = 1;
	 //    			cor_count++;
	 //    		}
	 //    	}
		
		// 	sw.WriteLine(
		// 	 isCorrect + "\t" + delta.ToString("F2") + "\t" + thinkingTime + "\t" +  
		// 	 GameControl.instance.columnObj.amplitudeRotate + "\t" +
		// 	 s + "\t" + t + "\t" + cor_count);
		// }

  //   }
}

