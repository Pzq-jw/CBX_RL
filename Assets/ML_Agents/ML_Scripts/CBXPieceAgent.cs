using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using static GO_Extensions;

public class CBXPieceAgent : Agent
{
    public CBXSwingReward swingRewardObj;
	public Transform mlTarget;
    public Transform mlColumn;
	public Piece pieceObj;
	public Transform monitorObj;

	public bool isJustCalledDone;
	public int deadcenterCount = 0;

    [SerializeField]
    private string rewardFunc;

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "Default_Reward" : rf;
        Debug.Log(" ! CBXPieceAgent: reward_func = " + rewardFunc);
	}

	public override void AgentReset()
	{
		HookNewPiece4ML();
		// mlTarget.position = new Vector3(Random.value*3-1.5f, -3.7f, 0);
        ResetEnv();
	}

	public override void CollectObservations()
	{
		AddVectorObs(this.transform.position);
        AddVectorObs(mlTarget.position);
		AddVectorObs(mlTarget.localPosition);
        AddVectorObs(mlColumn.eulerAngles.z);
        AddVectorObs(mlColumn.position.x);

        // Monitor.Log("rotation", mlTarget.rotation.z);
        // Monitor.Log("rotation Euler", mlTarget.rotation.eulerAngles.z);
        // Debug.Log("rotation Euler = "+mlTarget.rotation.eulerAngles.z);
        // Debug.Log("rotation = "+mlTarget.rotation.z);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		// Actions
		int dropSignal = Mathf.FloorToInt(vectorAction[0]);
		Monitor.Log("drop signal : ", dropSignal.ToString(), monitorObj);
		Monitor.Log("reward : ", GetCumulativeReward().ToString(), monitorObj);
		Monitor.Log("deadcenter num : ", deadcenterCount.ToString(), monitorObj);
		if(dropSignal == 1)
		{
        	transform.parent = null;
            Vector3 p = transform.position;
            p.z = 0;
            transform.position = p;
        	transform.rotation = Quaternion.identity;
        	pieceObj.GetComponent<Rigidbody2D>().isKinematic = false;
        	pieceObj.isHooked = false;
        	isJustCalledDone = false;
		}
        // else if(dropSignal == 0)
        // {
        //     AddReward(-1f / agentParameters.maxStep);
        // }
	}

	void FixedUpdate()
	{
		if(isJustCalledDone)
		{
			RequestDecision();
		}
        Debug.DrawLine(new Vector3(transform.position.x, -30, 0), new Vector3(transform.position.x, 30, 0));
	}

    void HookNewPiece4ML()
    {
        this.transform.parent = null; // avoid x offset when hooking the piece from column
        this.transform.position = new Vector3(0, -2.25f, 0);
        this.transform.rotation = Quaternion.identity;
        this.transform.SetParent(GameControl.instance.slingObj.transform,false);
        pieceObj.isHooked = true;
        pieceObj.isStacked = false;
        this.transform.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void m_SetRewrd(float r)
    {
        SetReward(r);
    }


    public void ComputeReward()
    {
        // Debug.Log("this.localX = " + this.transform.localPosition.x.ToString());
        // Debug.Log("target,localX = " + mlTarget.transform.localPosition.x.ToString());
    	float absDelta = Mathf.Abs(this.transform.localPosition.x - mlTarget.transform.localPosition.x);

        System.Type rwType = swingRewardObj.GetType(); 
        MethodInfo rf = rwType.GetMethod(rewardFunc);
        object rewardObj = rf.Invoke(swingRewardObj, new object[]{absDelta});
        float reward = (float)rewardObj;

    	AddReward(reward);
    	Monitor.Log("DeltaX : ", absDelta, monitorObj);
    	Debug.Log("AbsDeltaX : " + absDelta, monitorObj);
    	Debug.Log("Immidate reward : "+reward.ToString() , gameObject);
     //    Debug.Log("CurrentStepCount : " + GetStepCount());
    }

    public void ResetEnv()
    {
        // GameControl.instance.slingObj.angle = Random.Range(0f, 360f);
        GameControl.instance.columnObj.ResetColumnStatus();
        mlTarget.transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), 4.3f, 0);
    }

}
