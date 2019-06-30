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
    public Rigidbody2D mlTargetRb2d;
	public Piece pieceObj;
	public Transform monitorObj;

	public bool isJustCalledDone;
	public int deadcenterCount = 0;

    private string rewardFunc;

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "Default_Reward" : rf;
	}

	public override void AgentReset()
	{
		HookNewPiece4ML();
		// mlTarget.position = new Vector3(Random.value*3-1.5f, -3.7f, 0);
	}

	public override void CollectObservations()
	{
		AddVectorObs(this.transform.position);
		AddVectorObs(mlTarget.position);
        AddVectorObs(mlTarget.rotation.z);
        AddVectorObs(mlTargetRb2d.velocity.x);
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
	}

	void FixedUpdate()
	{
		if(isJustCalledDone)
		{
			RequestDecision();
		}
	}

    void HookNewPiece4ML()
    {
        this.transform.parent = null; // avoid x offset when hooking the piece from column
        this.transform.position = new Vector3(0, -2.25f, 0);
        this.transform.SetParent(GameControl.instance.slingObj.transform,false);
        pieceObj.isHooked = true;
        pieceObj.isStacked = false;
        this.transform.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void ComputeReward()
    {
    	float absDelta = Mathf.Abs(this.transform.localPosition.x - mlTarget.transform.localPosition.x);

        System.Type rwType = swingRewardObj.GetType(); 
        MethodInfo rf = rwType.GetMethod(rewardFunc);
        object rewardObj = rf.Invoke(swingRewardObj, new object[]{absDelta});
        float reward = (float)rewardObj;

    	AddReward(reward);
    	Monitor.Log("DeltaX : ", absDelta, monitorObj);
    	Debug.Log("AbsDeltaX : " + absDelta, monitorObj);
    	Debug.Log("Immidate reward : "+reward.ToString() , gameObject);
    }

}
