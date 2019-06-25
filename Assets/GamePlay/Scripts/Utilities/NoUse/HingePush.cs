using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingePush : MonoBehaviour
{

	public Rigidbody2D rb2d;
	public float leftPushRange;
	public float rightPushRange;
	public float velocityThreshold;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.angularVelocity = velocityThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        Push();
    }

    private void Push()
    {
    	if(transform.rotation.z > 0
    		&& transform.rotation.z < rightPushRange
    		&& (rb2d.angularVelocity > 0)
    		&& rb2d.angularVelocity < velocityThreshold)
    	{
    		rb2d.angularVelocity = velocityThreshold;
    	}
    	else if(transform.rotation.z < 0
    		&& transform.rotation.z > leftPushRange
    		&& (rb2d.angularVelocity < 0)
    		&& rb2d.angularVelocity > velocityThreshold * -1)
    	{
    		rb2d.angularVelocity = velocityThreshold * -1;
    	}
    }
}
