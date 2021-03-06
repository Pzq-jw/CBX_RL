﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Reflection;
using static GO_Extensions;


public class CCMAgent_Position : Agent
{
	public Rigidbody2D targetRb2d;
	public Transform targetTran;
    public Rigidbody2D columnRb2d;
    public Transform columnTran;
	public Piece pieceObj;
	public Academy academy;
	public Transform slingObj;
    public Transform root;
    public ColumnReset columnResetObj;
    public CBXSwingReward swingRewardObj;
    public ColumnSwinging columnObj;
    public CCMTest tester;

    [SerializeField]
    private string rewardFunc;

	public bool isJustCalledDone;
    [SerializeField]
    int configuration;
    Rigidbody2D agentRb2d;
    public bool isDebug;
    public bool isTest;

    private List<float> perceptionBuffer = new List<float>();
    public List<GameObject> piecesList = new List<GameObject>();
    public List<PiecePosRange> piecesDataList = new List<PiecePosRange>();

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
		agentRb2d = GetComponent<Rigidbody2D>();
		academy = FindObjectOfType<CBXAcademy>();
		slingObj = this.transform.parent.Find("Sling").transform;
		configuration = Random.Range(0, 5);
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "New_Hybrid" : rf;

        if(isTest)
        {
            tester = GetComponent<CCMTest>();
            tester.Title();
        }
	}

	void FixedUpdate()
	{
        // if(isDebug)
        // {
        //     if(Input.GetKeyDown(KeyCode.H))
        //     {
        //         RequestDecision();
        //     }
        // }
        // else 
        // {
            if(isJustCalledDone)
            {
                RequestDecision();
            }            
        // }

	}

	public override void AgentReset()
	{
		HookPieceAgent();
		configuration = Random.Range(0, 5);
        ConfigureAgent(configuration);
        columnResetObj.ResetAllPiecesPos_MaxMin();
        isJustCalledDone = true;
	}

    void HookPieceAgent()
    {
        this.transform.SetParent(slingObj, true);
        this.transform.localPosition = new Vector3(0, -2.25f, 0);
        this.transform.localRotation = Quaternion.identity;
        pieceObj.isHooked = true;
        pieceObj.isStacked = false;
        this.transform.GetComponent<Rigidbody2D>().isKinematic = true;
        if(isTest)
            tester.tt = Time.time;
    }

    void ConfigureAgent(int config)
    {
    	// Debug.Log("config = " + config);
    	string rot = "rotation"; //(5, 15)
		columnObj.amplitudeRotate = GetResetValue(rot);
        columnResetObj.min = academy.resetParameters["offset" + "_min"];
        columnResetObj.max = academy.resetParameters["offset" + "_max"];
		// Debug.Log("rot = " + columnObj.amplitudeRotate);
		// Debug.Log("pos = " + col.amplitudeMove);
		// Debug.Log("offset = " + targetTran.localPosition.x);
    }

    float GetResetValue(string arg)
    {
    	float min = academy.resetParameters[arg + "_min"];
    	float max = academy.resetParameters[arg + "_max"];
    	return min + Random.value * (max - min);
    }

    public override void CollectObservations()
    {
        float rot = (columnObj.amplitudeRotate - 5f) / 10f;
        Vector2 agentPos = root.transform.InverseTransformPoint(agentRb2d.position);
        agentPos.x = (agentPos.x + 1.42f) / 2.72f;
        agentPos.y = (agentPos.y - 2.2f) / 1.2f;

        AddVectorObs(agentPos); // 2
        AddVectorObs((agentRb2d.rotation + 20f) / 40f); // 1

        AddVectorObs((columnTran.localPosition.x + 0.5f) / 1f); // 1
        AddVectorObs((columnRb2d.rotation + 15f) / 30f); // 1

        AddVectorObs(PerceptPieces()); // 36

        AddVectorObs(rot); // 1

    }

    public List<float> PerceptPieces()
    {
        perceptionBuffer.Clear();
        for(int i=0; i<piecesList.Count; i++)
        {
            float[] sublist = new float[4];
            SetSubList(piecesList[i], sublist, i);
            perceptionBuffer.AddRange(sublist);
        }
        return perceptionBuffer;
    }

    private void SetSubList(GameObject piece, float[] subList, int idx)
    {
        Rigidbody2D pieceRb2d = piece.GetComponent<Rigidbody2D>();
        Vector2 piecePos = root.transform.InverseTransformPoint(pieceRb2d.position);
        piecePos.x = (piecePos.x - piecesDataList[idx].minPosX) / piecesDataList[idx].posRangeX;
        piecePos.y = (piecePos.y - piecesDataList[idx].minPosY) / piecesDataList[idx].posRangeY;
        subList[0] = piecePos.x;
        subList[1] = piecePos.y;
        subList[2] = (pieceRb2d.rotation + 15f) / 30f;
        subList[3] = (piece.transform.localPosition.x + (0.5f * (idx+1))) / (1 * (idx+1));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
		int dropSignal = Mathf.FloorToInt(vectorAction[0]);
		Monitor.Log("drop signal : ", dropSignal.ToString());
		Monitor.Log("reward : ", GetCumulativeReward().ToString());
		if(dropSignal == 1)
		{
            DropPieceAgent();
		}
        else if(dropSignal == 0)
        {
            AddReward(-1f / agentParameters.maxStep);
            // if(isDebug)
            // {
            //     RequestDecision();
            // }
        }    	
    }

    void DropPieceAgent()
    {
        isJustCalledDone = false;
        transform.SetParent(root, true);
        Vector3 p = transform.localPosition;
        p.z = 0;
        transform.localPosition = p;
        transform.localRotation = Quaternion.identity;
        pieceObj.GetComponent<Rigidbody2D>().isKinematic = false;
        pieceObj.isHooked = false;
        if(isTest)
            tester.tt = Time.time - tester.tt;
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
        if(isTest)
            tester.Record(absDelta, columnObj.amplitudeRotate);
    }































}
