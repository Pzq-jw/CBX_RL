using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using static GO_Extensions;

public class CBXPieceAgent : Agent
{
    public Rigidbody2D targetRb2d;
    public Transform targetTran;
    public Rigidbody2D columnRb2d;
    public Transform columnTran;
    public Piece pieceObj;
    public Academy academy;
    public Transform slingObj;
	public bool isJustCalledDone;
    public CBXSwingReward swingRewardObj;
    public Transform root;
    Rigidbody2D agentRb2d;

    [SerializeField]
    private string rewardFunc;

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
        agentRb2d = GetComponent<Rigidbody2D>();
        slingObj = this.transform.parent.Find("Sling").transform;
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "Default_Reward" : rf;
        // Debug.Log(" ! CBXPieceAgent: reward_func = " + rewardFunc);
	}

	public override void AgentReset()
	{
		HookNewPiece4ML();
        ResetEnv();
	}


    public void ResetEnv()
    {
        // GameControl.instance.slingObj.angle = Random.Range(0f, 360f);
        // GameControl.instance.columnObj.ResetColumnPos();
        targetTran.transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), 9.01f, 0);
    }

    public override void CollectObservations()
    {
        Vector2 agentPos = root.transform.InverseTransformPoint(agentRb2d.position);
        Vector2 targetPos = root.transform.InverseTransformPoint(targetRb2d.position);

        // Debug.Log("agentPos = "  + agentPos + "targetPos = " + targetPos);

        agentPos.x = agentPos.x / 1.3f;
        agentPos.y = (agentPos.y - 7.4f) / 1.2f;

        targetPos.x = targetPos.x / 3.5f;
        targetPos.y = (targetPos.y - 3.6f) / 0.41f;

        // Debug.Log("agentPosScale = "  + agentPos + "targetPosScale = " + targetPos);

        AddVectorObs(agentPos); // 2
        AddVectorObs(targetPos); // 2
        AddVectorObs(agentRb2d.rotation / 20f); // 1
        AddVectorObs(targetRb2d.rotation / 15f); // 1
        AddVectorObs(columnTran.localPosition.x / 0.5f); // 1
        AddVectorObs(columnRb2d.rotation / 15f); // 1
        AddVectorObs(targetTran.localPosition.x / 0.5f); // 1
        // try remove column obs
    }

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		// Actions
		int dropSignal = Mathf.FloorToInt(vectorAction[0]);
		Monitor.Log("drop signal : ", dropSignal.ToString());
		Monitor.Log("reward : ", GetCumulativeReward().ToString());
		if(dropSignal == 1)
		{
            isJustCalledDone = false;
            transform.parent = null;
            Vector3 p = transform.position;
            p.z = 0 + slingObj.GetComponent<EllipticalOrbit>().offsetZ;
            transform.position = p;
            transform.rotation = Quaternion.identity;
            pieceObj.GetComponent<Rigidbody2D>().isKinematic = false;
            pieceObj.isHooked = false;
		}
        else if(dropSignal == 0)
        {
            AddReward(-1f / agentParameters.maxStep);
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
        this.transform.localPosition = new Vector3(0, -2.25f, 0);
        this.transform.localRotation = Quaternion.identity;
        this.transform.SetParent(slingObj,false);
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
        // Debug.Log("target,localX = " + targetTran.localPosition.x.ToString());
    	float absDelta = Mathf.Abs(this.transform.localPosition.x - targetTran.localPosition.x);

        System.Type rwType = swingRewardObj.GetType(); 
        MethodInfo rf = rwType.GetMethod(rewardFunc);
        object rewardObj = rf.Invoke(swingRewardObj, new object[]{absDelta});
        float reward = (float)rewardObj;

    	SetReward(reward);
    	Monitor.Log("DeltaX : ", absDelta);
    	// Debug.Log("AbsDeltaX : " + absDelta);
    	// Debug.Log("Immidate reward : "+reward.ToString() , gameObject);
    }

}
