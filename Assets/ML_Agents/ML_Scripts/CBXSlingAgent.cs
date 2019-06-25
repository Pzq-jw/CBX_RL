using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CBXSlingAgent : Agent
{
	public Transform target;
	public bool isDrop;
	public override void AgentReset()
	{
		this.isDrop = false;
		target.position = new Vector3(Random.value*2-1, -4f, 0);
	}

	public override void CollectObservations()
	{
		AddVectorObs(this.transform.position);
		AddVectorObs(target.position);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		int dropSignal = Mathf.FloorToInt(vectorAction[0]);
		Monitor.Log("isDrop", isDrop.ToString());

		if(dropSignal == 1)
		{
			Monitor.Log("Released Piece!!!", "Make drop Decesion");
		}

		if(this.transform.position.x - target.position.x < 0.1)
		{
			AddReward(1f);
			Done();
		}
		else if(this.transform.position.x - target.position.x < 0.5)
		{
			AddReward(0.1f);
			Done();
		}
		else{
			AddReward(-1f);
			Done();
		}
	}
}
