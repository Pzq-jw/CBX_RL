using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CCMTest : MonoBehaviour
{

	public CCMStackAgent agent;

	public int totalTestNum=10;
	public float tt;
	public int deadCenterNum=0;
	public int failNum=0;
	public int noDC_SuccessfullNum=0;
	public float totalDelta=0f;
	public float totalThinkingTime=0f;
	private int i=0;

    public void Title()
    {
		using(StreamWriter sw = new StreamWriter(agent.brain.name + "velocity_accuracy.csv", true))
		{
			sw.WriteLine(
			 "idx" + "," + "delta" + "," + "thinkingTime" + "," + "rot");
		}   		
    }

    public void Record(float delta, float rot)
    {
		using(StreamWriter sw = new StreamWriter(agent.brain.name + "velocity_accuracy.csv", true))
		{
			sw.WriteLine(
			 i + "," + delta + "," + tt + "," + rot);
		}

        if(delta <= 0.1)
            deadCenterNum++;
        else if(delta <= 0.5)
        {
            totalDelta += delta;
            noDC_SuccessfullNum++;
        }
        else 
            failNum ++;

		totalThinkingTime += tt;
		i++;
		if(i == totalTestNum)
		{
			End();
			// UnityEditor.EditorApplication.isPlaying = false;
		}
    }

    public void End()
    {
		using(StreamWriter sw = new StreamWriter(agent.brain.name + "velocity_accuracy.csv", true))
		{
			sw.WriteLine("\n");
			sw.WriteLine("deadCenterNum = " + deadCenterNum);
			sw.WriteLine("failNum = " + failNum);
			sw.WriteLine("noDC_successfulNum = " + noDC_SuccessfullNum);
			sw.WriteLine("total delta = " + totalDelta);
			sw.WriteLine("total thinkingTime = " + totalThinkingTime);
			sw.WriteLine("mean delta = " + totalDelta / noDC_SuccessfullNum);
			sw.WriteLine("mean thinkingTime = " + totalThinkingTime / totalTestNum);
		}     	
    }
}
