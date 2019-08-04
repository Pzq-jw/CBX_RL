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
    public ColumnReset columnResetObj;
    public ColumnSwinging columnObj;
    Rigidbody2D agentRb2d;

    int configuration;

    [SerializeField]
    private string rewardFunc;

    private List<float> perceptionBuffer = new List<float>();
    public List<GameObject> piecesList = new List<GameObject>();
    public List<PiecePosRange> piecesDataList = new List<PiecePosRange>();

    void FixedUpdate()
    {
        if(isJustCalledDone)
        {
            RequestDecision();
        }
    }

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
        academy = FindObjectOfType<CBXAcademy>();
        configuration = Random.Range(0, 5);
        agentRb2d = GetComponent<Rigidbody2D>();
        slingObj = this.transform.parent.Find("Sling").transform;
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "Default_Reward" : rf;
        // Debug.Log(" ! CBXPieceAgent: reward_func = " + rewardFunc);
	}

	public override void AgentReset()
	{
		HookPieceAgent();
        configuration = Random.Range(0, 5);
        ConfigureAgent(configuration);
        columnResetObj.ResetAllPiecesPos();
        // Invoke("ActivateRequestDecision", 1f);
        isJustCalledDone = true;
	}

    void ActivateRequestDecision()
    {
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
    }

    void ConfigureAgent(int config)
    {
        string rot = "rotation"; //(5, 15)
        columnObj.amplitudeRotate = GetResetValue(rot);
    }

    float GetResetValue(string arg)
    {
        float min = academy.resetParameters[arg + "_min"];
        float max = academy.resetParameters[arg + "_max"];
        return (int)(min + Random.value * (max - min));
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
            // float[] sublist = new float[4];
            // SetSubList(piecesList[8], sublist, 8);
            // perceptionBuffer.AddRange(sublist);
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
		// Actions
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
