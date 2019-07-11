using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipticalOrbit : MonoBehaviour
{
	public float a = 50, b = 30;
	public float h = 0;
	public float angularSpeed = 0.1f;
	public float rotateAngle = 20f;

	public float angle;
	private float x, y, z;
	public Vector3 _pos;

	public float rotateX;
	public float rotateY;
	public float rotateZ;

	public float offsetY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        angle = 0;
        // transform.position = new Vector3(b*Mathf.Sin(0), h*Mathf.Cos(0), a*Mathf.Cos(0));
    }

    bool IsHookShouldSpin()
    {
        // When add load scene, here should add extra check
        if(GameControl.instance.gameStatus != GameControl.GameStatus.GAME_OVER)
            return true;
        else
            return false;
    }

    void FixedUpdate()
    {   
        if(IsHookShouldSpin())
        {
            angle += Time.deltaTime * angularSpeed;
            z = a * Mathf.Cos(angle);
            x = b * Mathf.Sin(angle);
            y = h * Mathf.Cos(angle) + offsetY;
            _pos = new Vector3(x,y,z);

            rotateX = -10 * Mathf.Sin(angle);
            rotateY = rotateAngle * Mathf.Cos(angle);
            rotateZ = rotateAngle * Mathf.Cos(angle);
            transform.rotation = Quaternion.Euler(rotateX, rotateY, rotateZ);

            transform.position = Vector3.Lerp(transform.position, _pos, Time.deltaTime);
        }
    }

}
