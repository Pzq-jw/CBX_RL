using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public DoTweenControl doTween;
    public struct StackStatus
    {
        public bool isStackSuccessful;
        public int fallenSide;
        public bool isDeadCenter;
    }
    public StackStatus stackStatus;

    public bool isHooked = false;
    public bool isStacked = true;
    public bool isFallen = false;

    public float deadCenterRange = 0.08f;
    public float stackRange = 0.5f;

	public Rigidbody2D rb2d;
    public Transform columnObj;
    public CCMStackAgent agentObj;
    public CBXPieceAgent cbxPieceAgentObj;
    public RewardAgent rewardAgent;
    public CCMAgent_Position posAgentObj;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        columnObj = this.transform.parent.Find("Column");
    }

    void OnCollisionEnter2D(Collision2D ctl)
    {
        if(!isStacked)
        {
            rb2d.isKinematic = true;
            rb2d.velocity = Vector2.zero;
            Parent2Column();
            // GameControl.instance.mycolObj.SetCollisionInfo(ctl);
            if(agentObj)
            {
                agentObj.ComputeReward();
                agentObj.Done();
            }

            else if(cbxPieceAgentObj)
            {
                cbxPieceAgentObj.ComputeReward();
                cbxPieceAgentObj.Done();
            }
            else if(rewardAgent)
            {
                rewardAgent.ComputeReward();
                rewardAgent.Done();
            }
            else if(posAgentObj)
            {
                posAgentObj.ComputeReward();
                posAgentObj.Done();
            }
        }
    }

    public void Parent2Column()
    {
        transform.SetParent(columnObj, true);        
        transform.localRotation = Quaternion.identity;
        rb2d.angularVelocity = 0;
    }
}