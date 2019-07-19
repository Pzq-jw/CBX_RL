using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CCMStackAgent : Agent
{
	public Rigidbody2D targetRb2d;
	public Transform targetTran;
    public Rigidbody2D columnRb2d;
	public Piece pieceObj;
	public Transform monitorObj;
	public Academy academy;
	public Transform slingObj;

	public bool isJustCalledDone;
    [SerializeField]
    int configuration;
    Rigidbody2D agentRb2d;

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
		agentRb2d = GetComponent<Rigidbody2D>();
		monitorObj = GameControl.instance.monitorObj;
		academy = FindObjectOfType<CBXAcademy>();
		slingObj = this.transform.parent.Find("Sling").transform;
		configuration = Random.Range(0, 5);
	}

	void FixedUpdate()
	{
		if(isJustCalledDone)
		{
			RequestDecision();
		}
	}

	public override void AgentReset()
	{
		HookNewPiece4ML();
		configuration = Random.Range(0, 5);
        ConfigureAgent(configuration);
	}

    void HookNewPiece4ML()
    {
        this.transform.parent = null; // avoid x offset when hooking the piece from column
        this.transform.localPosition = new Vector3(0, -2.25f, 0);
        this.transform.rotation = Quaternion.identity;
        this.transform.SetParent(slingObj,false);
        pieceObj.isHooked = true;
        pieceObj.isStacked = false;
        this.transform.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    void ConfigureAgent(int config)
    {
    	// Debug.Log("config = " + config);
    	var col = GameControl.instance.columnObj;
    	string rot = "rotation";
    	string pos = "position";
    	string offset = "tc_offset";
    	if(config == 0)
    	{
    		col.amplitudeRotate = 0;
    		col.amplitudeMove = 0;
    	}
    	else
    	{
    		col.amplitudeRotate = GetResetValue(rot);
    		col.amplitudeMove = GetResetValue(pos);
    		targetTran.transform.localPosition = 
    			new Vector3(GetResetValue(offset), 4.3f, 0);
    		// Debug.Log("rot = " + col.amplitudeRotate);
    		// Debug.Log("pos = " + col.amplitudeMove);
    		// Debug.Log("offset = " + targetTran.localPosition.x);
    	}
    }

    float GetResetValue(string arg)
    {
    	float min = academy.resetParameters[arg + "_min"];
    	float max = academy.resetParameters[arg + "_max"];
    	return min + Random.value * (max - min);
    }

    public override void CollectObservations()
    {
    	Vector2 agentPos = new Vector2(agentRb2d.position.x / 1.28f,
    								(agentRb2d.position.y - 2.23f) / 1.14f);
    	Vector2 targetPos = new Vector2(targetRb2d.position.x / 2f,
    								 (targetRb2d.position.y + 0.85f) / 0.15f);
    	AddVectorObs(agentPos); // 2
    	AddVectorObs(targetPos); // 2
    	AddVectorObs(agentRb2d.rotation / 20f); // 1
    	AddVectorObs(targetRb2d.rotation / 15f); // 1
    	AddVectorObs(columnRb2d.position.x / 0.5f); // 1
    	AddVectorObs(columnRb2d.rotation / 15f); // 1
    	// AddVectorObs(GameControl.instance.columnObj.amplitudeRotate / 15f); // 1
    	// AddVectorObs(GameControl.instance.columnObj.amplitudeMove / 0.5f); // 1
    	AddVectorObs(targetTran.localPosition.x / 0.5f); // 1 

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
		int dropSignal = Mathf.FloorToInt(vectorAction[0]);
		Monitor.Log("drop signal : ", dropSignal.ToString(), monitorObj);
		Monitor.Log("reward : ", GetCumulativeReward().ToString(), monitorObj);
		if(dropSignal == 1)
		{
        	transform.parent = null;
            Vector3 p = transform.position;
            p.z = 0 + slingObj.GetComponent<EllipticalOrbit>().offsetZ;
            transform.position = p;
        	transform.rotation = Quaternion.identity;
        	pieceObj.GetComponent<Rigidbody2D>().isKinematic = false;
        	pieceObj.isHooked = false;
        	isJustCalledDone = false;
		}
        else if(dropSignal == 0)
        {
            AddReward(-1f / agentParameters.maxStep);
        }    	
    }

    public void m_SetRewrd(float r)
    {
        SetReward(r);
    }






























}
