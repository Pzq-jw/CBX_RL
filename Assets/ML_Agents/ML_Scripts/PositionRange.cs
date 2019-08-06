using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PositionRange : MonoBehaviour
{
    // Start is called before the first frame update
    public CCMStackAgent agentObj;
	public Vector2[] velocitys;
    private Rigidbody2D rb2d;
	public float[] max_x;
	public float[] min_x;
	public float[] max_y;
	public float[] min_y;

    private float min_rot;
    private float max_rot;

    string fileName;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        velocitys = agentObj.piecesVelocity;
    }

    void FixedUpdate()
    {
    	// GetPositionRange();
        GetVelocityRange();
    }

    void GetVelocityRange()
    {
        velocitys = agentObj.piecesVelocity;
        for(int i=0; i<9; i++)
        {
        fileName = "PieceVelocity_t" + i.ToString() + "_data.txt";
            if(velocitys[i].x > max_x[i]){
                max_x[i] = velocitys[i].x;
                // Debug.Log("max_x = " + max_x);
            }
            else if(velocitys[i].x < min_x[i]){
                min_x[i] = velocitys[i].x;
                // Debug.Log("min_x = " + min_x);          
            }

            if(velocitys[i].y > max_y[i]){
                max_y[i] = velocitys[i].y;
                // Debug.Log("max_y = " + max_y);
            }
            else if(velocitys[i].y < min_y[i]){
                min_y[i] = velocitys[i].y;
                // Debug.Log("min_y = " + min_y);
            }
            using(StreamWriter sw = new StreamWriter(fileName , true))
            {
                sw.WriteLine(
                    max_x[i].ToString("F2") + "\t" + min_x[i].ToString("F2") + "\t" + 
                    max_y[i].ToString("F2") + "\t" + min_y[i].ToString("F2"));
            }
        }      
    }

    // void GetPositionRange()
    // {
    // 	Vector2 pos = rb2d.position;
    //     float rot = rb2d.rotation;

    // 	if(pos.x > max_x){
    // 		max_x = pos.x;
    // 		Debug.Log("max_x = " + max_x);
    // 	}
    // 	else if(pos.x < min_x){
    // 		min_x = pos.x;
    // 		Debug.Log("min_x = " + min_x);    		
    // 	}

    // 	if(pos.y > max_y){
    // 		max_y = pos.y;
    // 		Debug.Log("max_y = " + max_y);
    // 	}
    // 	else if(pos.y < min_y){
    // 		min_y = pos.y;
    // 		Debug.Log("min_y = " + min_y);
    // 	}

    //     if(rot > max_rot)
    //     {
    //         max_rot = rot;
    //     }
    //     else if(min_rot < rot)
    //     {
    //         min_rot = rot;
    //     }

    // 	using(StreamWriter sw = new StreamWriter(fileName , true))
    // 	{
    // 		sw.WriteLine(
    // 			max_x.ToString("F2") + "\t" + min_x.ToString("F2") + "\t" + 
    // 			max_y.ToString("F2") + "\t" + min_y.ToString("F2") + "\t" + 
    //             max_rot.ToString("F2") + "\t" + min_rot.ToString("F2"));
    // 	}

    // }

}
