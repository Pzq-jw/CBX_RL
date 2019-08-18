using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.IO;

public class DDATestHelper : MonoBehaviour
{
	public CBXDDAAgent[] agents;
	public Transform mlTarget;
	public Transform mlColumn;
    public ColumnReset columnResetObj;

	public int thinkingTime;
	public float rot;
	public string[] modelName;

	void Start()
	{
		agents = GetComponents<CBXDDAAgent>();
		Title();
	}

    public void ResetEnv()
    {
		thinkingTime = Random.Range(0, 13);
		rot = Random.Range(5, 17);
        mlColumn.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), -5f, 0);
        columnResetObj.ResetAllPiecesPos();
        this.transform.localPosition = new Vector3(Random.Range(-1.4f, 1.4f), 10.05f, 0);
        mlColumn.localRotation = Quaternion.Euler(new Vector3(0,0,
        		Random.Range(-rot, rot)));
    }

    void FixedUpdate()
    {
    	bool isAllDone = true;
    	foreach(CBXDDAAgent a in agents)
    	{
    		if(!a.isDone)
    		{
    			isAllDone = false;
    		}
    	}

    	if(isAllDone)
    	{
    		foreach(CBXDDAAgent a in agents)
    		{
    			ResetEnv();
    			a.Done();
    		}
    	}
    }

    void Title()
    {
    	foreach(string name in modelName)
    	{
			using(StreamWriter sw = new StreamWriter(name + "_accuracy.txt", true))
			{
				sw.WriteLine(
				 "isError" + "\t" + "delta" + "\t" + "thinkingTime" + "\t" +  
				 "rot" + "\t" + "cur_s" + "\t" + "signal" + "\t" + "err_count" + "\t" +
				 "e2e" + "\t" + "e2m" + "\t" + "e2d" + "\t" +
				 "m2e" + "\t" + "m2m" + "\t" + "m2d" + "\t" +
				 "d2e" + "\t" + "d2m" + "\t" + "d2d");
			}   		
    	}
    }

    public string Brain2Model(string brain)
    {
		switch(brain)
		{
			case "SaverLearning_LSTM":
				return "LSTM";
			case "SaverLearning_sv1":
				return "SV1";
			case "SaverLearning_sv3":
				return "SV3";
			case "SaverLearning_sv5":
				return "SV5";
		}
		return null;
    }



















}
