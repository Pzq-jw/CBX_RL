using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Reflection;
using static GO_Extensions;
using System.IO;


public class CCMStackAgent : Agent
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
    public bool isDebug;

    public Rigidbody2D[] piecesRb2d;
    public Vector2 agentVelocity;
    public Vector2[] piecesVelocity;
    public Vector2 agentLastPos;
    public Vector2[] piecesLastPos;

    private List<float> perceptionBuffer = new List<float>();
    public List<GameObject> piecesList = new List<GameObject>();
    public List<PiecePosRange> piecesDataList = new List<PiecePosRange>();

    Vector2 pos2root(Rigidbody2D rb2d)
    {
        return root.transform.InverseTransformPoint(rb2d.position);
    }

	public override void InitializeAgent()
	{
		isJustCalledDone = true;
		agentRb2d = GetComponent<Rigidbody2D>();
		academy = FindObjectOfType<CBXAcademy>();
		slingObj = this.transform.parent.Find("Sling").transform;
		configuration = Random.Range(0, 5);
        string rf = this.transform.GetArg("--rf");
        rewardFunc = string.IsNullOrEmpty(rf) ? "New_Hybrid" : rf;
        InitPos();

	}

    void InitPos()
    {
        agentLastPos = pos2root(agentRb2d);
        for(int i=0; i<piecesList.Count; i++)
        {
            piecesRb2d[i] = piecesList[i].GetComponent<Rigidbody2D>();
            piecesLastPos[i] = pos2root(piecesRb2d[i]);
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

            SetSpeed();

        // if(Input.GetKeyDown(KeyCode.P))
        // {
        //     ScreenCapture.CaptureScreenshot("Screenshot" + Time.time + ".png");
        // }

	}

    void SetSpeed()
    {
        agentVelocity = (pos2root(agentRb2d) - agentLastPos) / Time.deltaTime;
        agentLastPos = pos2root(agentRb2d);
        for(int i=0; i<piecesList.Count; i++)
        {
            piecesVelocity[i] = (pos2root(piecesRb2d[i]) - piecesLastPos[i]) / Time.deltaTime;
            piecesLastPos[i] = pos2root(piecesRb2d[i]);
        }        
    }

	public override void AgentReset()
	{
		HookPieceAgent();
		configuration = Random.Range(0, 5);
        ConfigureAgent(configuration);
        columnResetObj.ResetAllPiecesPos_MaxMin();
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
        AddVectorObs(agentVelocity); // 2
        // AddVectorObs((columnTran.localPosition.x + 0.5f) / 1f); // 1
        // AddVectorObs((columnRb2d.rotation + 15f) / 30f); // 1

        AddVectorObs(PerceptPieces()); // 54

        AddVectorObs(rot); // 1

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
        // subList[4] = (piecesVelocity[idx].x);
        // subList[5] = (piecesVelocity[idx].y);

        subList[4] = (piecesVelocity[idx].x - piecesDataList[idx].minVeloX) / piecesDataList[idx].veloRangeX;
        subList[5] = (piecesVelocity[idx].y - piecesDataList[idx].minVeloY) / piecesDataList[idx].veloRangeY;
        // Debug.Log(piecesVelocity[idx].x + " " + piecesVelocity[idx].y + " " + piecesDataList[idx].veloRangeX + " " + piece.name);
        // if(subList[4] > 1f || subList[5] > 1f)
        //     Debug.Log("vel_x = " + subList[4] + " vel_y = " + subList[5] + " " + piece.name);
    }

    private void SetSubList_noY(GameObject piece, float[] subList, int idx)
    {
        Rigidbody2D pieceRb2d = piece.GetComponent<Rigidbody2D>();
        Vector2 piecePos = root.transform.InverseTransformPoint(pieceRb2d.position);
        piecePos.x = (piecePos.x - piecesDataList[idx].minPosX) / piecesDataList[idx].posRangeX;
        subList[0] = piecePos.x;
        subList[1] = (pieceRb2d.rotation + 15f) / 30f;
        subList[2] = (piece.transform.localPosition.x + (0.5f * (idx+1))) / (1 * (idx+1));
        subList[3] = (piecesVelocity[idx].x);
        // Debug.Log("vel_x = " + subList[4] + " vel_y = " + subList[5] + " " + piece.name);
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
