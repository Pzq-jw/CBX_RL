using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CBXPieceAgent : Agent
{
	public Transform mlTarget;
    public Rigidbody2D mlTargetRb2d;
	public Piece pieceObj;
	public Transform monitorObj;
	public bool isJustCalledDone;
	public int deadcenterCount = 0;

	public override void InitializeAgent()
	{
		isJustCalledDone = true;		
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
        // float absDelta = Mathf.Abs(this.transform.position)
    	float reward = Power_Hybrid_PP4_NP2(absDelta);
    	AddReward(reward);
    	Monitor.Log("DeltaX : ", absDelta, monitorObj);
    	Debug.Log("AbsDeltaX : " + absDelta, monitorObj);
    	Debug.Log("Immidate reward : "+reward.ToString() , gameObject);
    }

    private float Power_N2_Reward(float distance)
    {
    	if(distance < 0.1)
    		return 1f;
    	else if(distance < 2)
    	{
    		return (1 / (distance * distance * 100));
    	}
    	else
    		return 0f;
    }

    private float Power_P04_Reward(float distance)
    {
    	float distance_max = 3f;
    	float reward = 1 - Mathf.Pow(distance / distance_max, 0.4f);
    	return reward;
    }

    private float Power_N3_Penalty(float distance)
    {
        float distance_max = 3f;
        return -Mathf.Pow(distance/distance_max, 3f);
    }

    private float Power_Hybrid(float distance)
    {
        float distance_max = 3f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * 100));
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }

    private float Power_Hybrid_NP2(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * 100));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    private float Power_Hybrid_PP3_NP2(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * 1000));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    private float Power_Hybrid_PP4_NP2(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * distance * 10000));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

}
