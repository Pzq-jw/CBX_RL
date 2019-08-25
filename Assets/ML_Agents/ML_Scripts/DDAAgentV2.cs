using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.IO;

public class DDAAgentV2 : Agent
{
	public Transform mlTarget;
	public Transform mlColumn;
	public Piece pieceObj;
    public ColumnReset columnResetObj;
    public DDATestHelper testHelperObj;

	private float thinkingTime;
	private int leftLife;
	private int totalPieceNum;
	private int isAIHelpLastTime;
	private float delta;
	private float rot;
	private int curGameStatus; 
    public int err_count=0, total_count=0;
    public int m2d=0, m2e=0, m2m=0;
    public int e2e=0, e2m=0, e2d=0;
    public int d2e=0, d2m=0, d2d=0;
    public string s, t;
    public int isError = 0;

    public bool isDone;

    [SerializeField]
    private bool isDebug;
    [SerializeField]
    private bool isTest;

	private List<float> perceptionBuffer = new List<float>();

	public override void AgentReset()
	{
		// thinkingTime = Random.Range(0f, 12f);
		// rot = Random.Range(5, 17);
  //       mlColumn.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), -5f, 0);
  //       columnResetObj.ResetAllPiecesPos();
  //       this.transform.localPosition = new Vector3(Random.Range(-1.4f, 1.4f), 10.05f, 0);
  //       mlColumn.localRotation = Quaternion.Euler(new Vector3(0,0,
  //       		Random.Range(-rot, rot)));
  		testHelperObj.ResetEnv();	
	}

	public override void CollectObservations()
	{
		delta = Mathf.Abs(this.transform.localPosition.x - mlTarget.localPosition.x);
		// Debug.Log("this.x = " + this.transform.localPosition.x.ToString());
		// Debug.Log("target.x = " + mlTarget.localPosition.x.ToString());
		// Debug.Log("delta = " + delta.ToString() + " thinkingTime = " + thinkingTime + " rot = " + rot);
		thinkingTime = testHelperObj.thinkingTime;
		rot = testHelperObj.rot;
		AddVectorObs(thinkingTime / 12f);
		AddVectorObs((rot - 5f) / 16f);
		AddVectorObs(delta / 5f);
		AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);
		if(isDebug)
			Debug.Log("delta = " + delta + " time = " + thinkingTime + " rotate = " + GameControl.instance.columnObj.amplitudeRotate);
	}

	public float deltaDegree
	{
		get 
		{
			if(delta < 0.1)
			{
				return 0;
			}
			else if(delta < 0.5)
			{
				return (float)Mathf.Pow(delta*2, 0.3f);
			}
			else
			{
				return 1;
			}
		}
	}

	public float timeDegree
	{
		get
		{
				return (float)Mathf.Pow(thinkingTime / 12f, 0.4f);
		}
	}

	private int GetCurrentState()
	{
		float exp = 0.2f * timeDegree + 0.7f * deltaDegree
					+ 0.1f * ((GameControl.instance.columnObj.amplitudeRotate-5f) / 11f);
		if(isDebug)
		{
			Debug.Log("deltaDegree = " + deltaDegree + " " + "timeDegree = " + timeDegree + " " +
						"exp = " + exp);
		}
		if(exp < 0.3)
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
		if(isDebug)
			Debug.Log("current state = " + state + "  " + "tuneSignal = " + turnSignal);

		if(isTest)
			Accuracy(state, turnSignal);

		if(turnSignal == 0)
		{
			AddReward(-1f/agentParameters.maxStep);
			AgentReset();
			// isDone = true;
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
			// isDone = true;
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
			// isDone = true;
			Done();
		}
	}

    public void Accuracy(int curState, int turnSignal)
    {	
		string name = testHelperObj.Brain2Model(this.brain.name);
		using(StreamWriter sw = new StreamWriter(name + "_accuracy.txt", true))
		{
	    	total_count++;
	    	if(curState == 1)
	    	{
	    		s = "easy";
	    		if(turnSignal == 2)
	    		{
	    			t = "turn_easy";
	    			err_count++;
	    			isError = 1;
	    			e2e++;
	    		}
	    		else if(turnSignal == 0)
	    		{
	    			t = "turn_medi";
	    			isError = 0;
	    			e2m++;
	    		}
	    		else if(turnSignal == 1)
	    		{
	    			t = "turn_diff";
	    			isError = 0;
	    			e2d++;
	    		}
	    	}
	    	else if(curState == 0)
	    	{
	    		s = "medi";
	    		if(turnSignal == 1)
	    		{
	    			t = "turn_diff";
	    			isError = 0;
	    			m2d++;
	    		}
	    		else if(turnSignal == 0)
	    		{
	    			t = "turn_medi";
	    			m2m++;
	    			isError = 0;
	    		}
	    		else if(turnSignal == 2)
	    		{
	    			t = "turn_easy";
	    			isError = 0;
	    			m2e++;
	    		}
	    	}
	    	else if(curState == 2)
	    	{
	    		s = "diff";
	    		if(turnSignal == 1)
	    		{
	    			t = "turn_diff";
	    			isError = 1;
	    			err_count++;
	    			d2d++;		
	    		}
	    		else if(turnSignal == 0)
	    		{
	    			t = "turn_medi";
	    			isError = 0;
	    			d2m++; 			
	    		}
	    		else if(turnSignal == 2)
	    		{
	    			t = "turn_easy";
	    			isError = 0;
	    			d2e++;
	    		}
	    	}
		
			sw.WriteLine(
			 isError + "\t" + delta.ToString("F2") + "\t" + thinkingTime + "\t" +  
			 rot + "\t" +
			 s + "\t" + t + "\t" + err_count + "\t" +
			 e2e + "\t" + e2m + "\t" + e2d + "\t" +
			 m2e + "\t" + m2m + "\t" + m2d + "\t" +
			 d2e + "\t" + d2m + "\t" + d2d);
		}

    }

}


