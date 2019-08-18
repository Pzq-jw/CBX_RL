using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Reflection;
using static GO_Extensions;


public class RewardAgent : Agent
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

    [SerializeField]
    private string rewardFunc;

	public bool isJustCalledDone;
    [SerializeField]
    int configuration;
    Rigidbody2D agentRb2d;
    Transform agentTran;
    public bool isDebug;

    public Rigidbody2D[] piecesRb2d;
    public Vector2 agentVelocity;
    public Vector2[] piecesVelocity;
    public Vector2 agentLastPos;
    public Vector2[] piecesLastPos;

    private List<float> perceptionBuffer = new List<float>();
    public List<GameObject> piecesList = new List<GameObject>();
    public List<PiecePosRange> piecesDataList = new List<PiecePosRange>();

    Vector2 pos2root(Vector2 pos)
    {
        return root.transform.InverseTransformPoint(pos);
    }

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
		agentRb2d = GetComponent<Rigidbody2D>();
        agentTran = GetComponent<Transform>();
		academy = FindObjectOfType<CBXAcademy>();
		slingObj = this.transform.parent.Find("Sling").transform;
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "New_Hybrid" : rf;
	}

    void ResetPos()
    {
        Vector2 agentWorldPos = new Vector2(agentTran.position.x, agentTran.position.y);
        agentLastPos = pos2root(agentWorldPos);
        for(int i=0; i<piecesList.Count; i++)
        {
            Vector2 pieceWorldPos = new Vector2(piecesList[i].transform.position.x, piecesList[i].transform.position.y);
            piecesLastPos[i] = pos2root(pieceWorldPos);
        }
    }

	void FixedUpdate()
	{
        if(isJustCalledDone)
        {
            RequestDecision();
        }            
        SetSpeed();
	}

    void SetSpeed()
    {
        Vector2 agentWorldPos = new Vector2(agentTran.position.x, agentTran.position.y);
        agentVelocity = (pos2root(agentWorldPos) - agentLastPos) / Time.deltaTime;
        agentLastPos = pos2root(agentWorldPos);
        for(int i=0; i<piecesList.Count; i++)
        {
            Vector2 pieceWorldPos = new Vector2(piecesList[i].transform.position.x, piecesList[i].transform.position.y);
            piecesVelocity[i] = (pos2root(pieceWorldPos) - piecesLastPos[i]) / Time.deltaTime;
            piecesLastPos[i] = pos2root(pieceWorldPos);
        }
    }

	public override void AgentReset()
	{
		HookPieceAgent();
        ResetPos();
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

    public override void CollectObservations()
    {
        Vector2 agentPos = root.transform.InverseTransformPoint(agentRb2d.position);
        agentPos.x = (agentPos.x + 1.42f) / 2.72f;
        agentPos.y = (agentPos.y - 2.2f) / 1.2f;
        AddVectorObs(agentPos); // 2
        AddVectorObs((agentRb2d.rotation + 20f) / 40f); // 1
        AddVectorObs(agentVelocity); // 2
        AddVectorObs(PerceptPieces()); // 54
    }

    public List<float> PerceptPieces()
    {
        perceptionBuffer.Clear();
        for(int i=0; i<=piecesList.Count-1; i++)
        {
            float[] sublist = new float[6];
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
        subList[4] = (piecesVelocity[idx].x - piecesDataList[idx].minVeloX) / piecesDataList[idx].veloRangeX;
        subList[5] = (piecesVelocity[idx].y - piecesDataList[idx].minVeloY) / piecesDataList[idx].veloRangeY;
        // Debug.Log(piecesVelocity[idx].x + " " + piecesVelocity[idx].y + " " + piecesDataList[idx].veloRangeX + " " + piece.name);
        // if(subList[4] > 1f || subList[5] > 1f)
        //      Debug.Log("vel_x = " + subList[4] + " vel_y = " + subList[5] + " " + piece.name);
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
        // Monitor.Log("DeltaX : ", absDelta);
        // Debug.Log("AbsDeltaX : " + absDelta);
        // Debug.Log("Immidate reward : "+reward.ToString() , gameObject);
    }































}
