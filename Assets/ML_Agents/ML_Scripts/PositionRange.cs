using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PositionRange : MonoBehaviour
{
    // Start is called before the first frame update
	private Rigidbody2D rb2d;
	private float max_x = 0f;
	private float min_x = 0f;
	private float max_y;
	private float min_y;

    private float min_rot;
    private float max_rot;

    string fileName;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        max_y = rb2d.position.y;
        min_y = rb2d.position.y;
        fileName = "PieceData_" + this.gameObject.name + "_data.txt";
    }

    void FixedUpdate()
    {
    	GetPositionRange();
    }

    void GetPositionRange()
    {
    	Vector2 pos = rb2d.position;
        float rot = rb2d.rotation;

    	if(pos.x > max_x){
    		max_x = pos.x;
    		Debug.Log("max_x = " + max_x);
    	}
    	else if(pos.x < min_x){
    		min_x = pos.x;
    		Debug.Log("min_x = " + min_x);    		
    	}

    	if(pos.y > max_y){
    		max_y = pos.y;
    		Debug.Log("max_y = " + max_y);
    	}
    	else if(pos.y < min_y){
    		min_y = pos.y;
    		Debug.Log("min_y = " + min_y);
    	}

        if(rot > max_rot)
        {
            max_rot = rot;
        }
        else if(min_rot < rot)
        {
            min_rot = rot;
        }

    	using(StreamWriter sw = new StreamWriter(fileName , true))
    	{
    		sw.WriteLine(
    			max_x.ToString("F2") + "\t" + min_x.ToString("F2") + "\t" + 
    			max_y.ToString("F2") + "\t" + min_y.ToString("F2") + "\t" + 
                max_rot.ToString("F2") + "\t" + min_rot.ToString("F2"));
    	}

    }

}
