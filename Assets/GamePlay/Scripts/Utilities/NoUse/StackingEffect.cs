using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackingEffect : MonoBehaviour
{

	private bool stackStatus;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void stack2ColumnSuccess()
    {
    	stackStatus = true;
    }

    public void stack2ColumnFail ()
    {
    	stackStatus = false;
    }
}
