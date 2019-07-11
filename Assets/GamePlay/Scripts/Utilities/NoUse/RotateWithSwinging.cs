using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithSwinging : MonoBehaviour
{

	public float rotateX;
	public float rotateY;
	public float rotateZ;
	public float rotateAngle = 20f;

	private float angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	angle = GameControl.instance.slingObj.angle;
        rotateX = -rotateAngle * Mathf.Sin(angle);
        rotateY = rotateAngle * Mathf.Cos(angle);
        rotateZ = rotateAngle * Mathf.Cos(angle);
        transform.rotation = Quaternion.Euler(rotateX, rotateY, rotateZ);
    }
}
