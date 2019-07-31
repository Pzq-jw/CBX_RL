using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GO_Extensions;

public class ColumnReset : MonoBehaviour
{

	public List<Transform> pieceList;
    public float offset;

    // Start is called before the first frame update
    void Start()
    {
		pieceList = new List<Transform>();
		this.transform.GetAllChildsGO(pieceList);
    }

    public void ResetAllPiecesPos()
    {
    	Vector3 lastPos = Vector3.zero;
        // offset = Random.Range(0, 2) == 1 ? offset : -offset;
    	for(int i=0; i<pieceList.Count; i++)
    	{
            offset = Random.Range(-0.5f, 0.5f);
    		Vector3 pos = pieceList[i].transform.localPosition;
    		pos.x = offset + lastPos.x;
    		
    		pieceList[i].transform.localPosition = pos;
    		lastPos = pos;
    	}
    }

}
