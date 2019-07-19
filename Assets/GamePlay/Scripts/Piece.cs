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

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        columnObj = this.transform.parent.Find("Column");
    }

    // Update is called once per frame
    void Update()
    {
    	if(isHooked)
    	{
	        if(Input.GetKeyDown("space") || Input.touchCount > 0)
	        {
	        	transform.parent = null;
                Vector3 p = transform.position;
                p.z = 0;
                transform.position = p;
	        	transform.rotation = Quaternion.identity;
	        	rb2d.isKinematic = false;
	        	isHooked = false;
	        }
    	}
    }

    void OnCollisionEnter2D(Collision2D ctl)
    {
        if(!isStacked)
        {
            GameControl.instance.OnPieceStacking();
            rb2d.isKinematic = true;
            rb2d.velocity = Vector3.zero;
            Parent2Column();        
        }
    }

    public void Parent2Column()
    {

        // transform.SetParent(GameControl.instance.columnObj.transform, true);
        transform.SetParent(columnObj, true);        
        // set the rotation for subObject using this way
        transform.localEulerAngles = Vector3.zero;
        rb2d.angularVelocity = 0;
    }

    void OnCollisionExit2D(Collision2D ctl)
    {
        GameControl.instance.mycolObj.SetCollisionInfo(ctl);
        if(!isStacked)
        {
            GameControl.instance.mycolObj.GetColumnHeightIncrement();
            if(CheckIfCanStack(ctl))
            {
                stackStatus.isStackSuccessful = true;
                GameControl.instance.AfterPieceStackingSuccessfully(stackStatus.isDeadCenter);
                // doTween.StackingNoDeadCenterAnimation(stackStatus.fallenSide, stackStatus.isDeadCenter);
            }
            else
            {
                stackStatus.isStackSuccessful = false;
                GameControl.instance.AfterPieceStackingFailed(stackStatus.fallenSide);
                // OnStackingFailed();

            }
            // GameControl.instance.saverAgentObj.RequestDecision();
            if(stackStatus.isDeadCenter)
                agentObj.m_SetRewrd(1f);
            else
                agentObj.m_SetRewrd(-1f);
            agentObj.Done();
            agentObj.isJustCalledDone = true;
            // Debug.Log("culmalitive reward = " + agentObj.GetReward().ToString(), gameObject);
        }
        isStacked = true;
    }

    public bool CheckIfCanStack(Collision2D ctl)
    {
        float absDeltaX = Mathf.Abs(GameControl.instance.mycolObj.deltaX);

        if(ctl.collider.gameObject.tag == "Ground")
        {
            return false;
        }

        checkFallenSide();
        // doTween.GetDeltaXFromCollision(absDeltaX);
       
        if(absDeltaX < stackRange)
        {
            isFallen = false;
            checkIfDeadCenter(absDeltaX, GameControl.instance.mycolObj.topPieceLocalPos.x, ctl.otherCollider);
            // Debug.Log(ctl.collider.gameObject.name + "  " + ctl.collider.transform.localPosition.x + " | " 
            //     + ctl.otherCollider.gameObject.name + "  " + ctl.otherCollider.transform.localPosition.x + " || " + "drop true");
            return true;
        }
        else 
        {
            // checkFallenSide(deltaX);
            // Debug.Log(ctl.collider.gameObject.name + "  " + ctl.collider.transform.localPosition.x + " | " 
            //     + ctl.otherCollider.gameObject.name + "  " + ctl.otherCollider.transform.localPosition.x + " || " + "drop false");
            isFallen = true;
            return false;
        }
    }

    private void checkIfDeadCenter(float absDeltaX, float topPiecePosX, Collider2D other)
    {
        if(absDeltaX < deadCenterRange)
        {
            stackStatus.isDeadCenter = true;
            Vector3 pos = other.transform.localPosition;
            pos.x = topPiecePosX;
            other.transform.localPosition = pos;
            pos = other.transform.position;
            pos.z -= 0.5f;
            pos.y -= 0.5f;
            // GameControl.instance.particleObj.PlayStackDeadCenterAnim(pos);
            // GameControl.instance.particleObj.PlayComboPeriodAnim();
        }
        else
        {
            stackStatus.isDeadCenter = false;
        }
    }

    private void checkFallenSide()
    {
        if(GameControl.instance.mycolObj.deltaX > 0)
        {
            stackStatus.fallenSide = 1;  // right side
        }
        else
        {
            stackStatus.fallenSide = -1;  // left side
        }
        // Debug.Log("fallen side = " + stackStatus.fallenSide);
    }

    private void OnStackingFailed()
    {
        // transform.position = new Vector3(0, -10f, 0);
        // GameControl.instance.doTweenObj.FallenAnimation(1); // cannot get the true transform
        // doTween.FallenAnimation(stackStatus.fallenSide);
        transform.parent = null;
    }
}
