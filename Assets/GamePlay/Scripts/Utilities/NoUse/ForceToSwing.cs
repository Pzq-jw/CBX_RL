using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceToSwing : MonoBehaviour
{
	public Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * 20 * Time.fixedDeltaTime);
        
    }
}
