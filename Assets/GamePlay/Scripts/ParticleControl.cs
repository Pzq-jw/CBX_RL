using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour
{

	// public GameObject stackDeadCenterEffectGO;
	// public GameObject fallenWaterEffectGO;
	// public GameObject comboPeriodEffectGO;

	public ParticleSystem stackDeadCenterAnim;
	public ParticleSystem fallenWaterAnim;
	public ParticleSystem comboPeriodAnim;

    public void PlayStackDeadCenterAnim(Vector3 pos)
    {
    	if(!stackDeadCenterAnim.isPlaying)
    	{
    		stackDeadCenterAnim.transform.position = pos;
    		stackDeadCenterAnim.Play();
    	}
    }


    public void PlayFallenWaterAnim(Vector3 fallenPos)
    {
    	if(!fallenWaterAnim.isPlaying)
    	{
            fallenWaterAnim.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            fallenWaterAnim.transform.SetAllChildsRotation(1, 0, true);
            fallenPos.y += 1.3f;
            Vector3 cameraPos = GameControl.instance.screenMoveUpObj.cameraPos;
            float distanceFromCamera = Mathf.Abs(cameraPos.z);
            float rotationXoffset = Mathf.Abs(Mathf.Atan(((Mathf.Abs(cameraPos.y) - Mathf.Abs(fallenPos.y))
                 / distanceFromCamera)) * Mathf.Rad2Deg);
            // Debug.Log("rotationXoffset = " + rotationXoffset, gameObject);
            float rotationYoffset = Mathf.Atan(fallenPos.x / distanceFromCamera) * Mathf.Rad2Deg;
            // Debug.Log("rotationYoffset = " + rotationYoffset, gameObject);
    		fallenWaterAnim.transform.position = fallenPos;
            // Debug.Log("fallen pos = " + fallenWaterAnim.transform.position,gameObject);
            
            var rot = fallenWaterAnim.transform.eulerAngles;
            rot.x += rotationXoffset;
            rot.y += rotationYoffset;
            fallenWaterAnim.transform.rotation = Quaternion.Euler(rot);
            fallenWaterAnim.transform.SetAllChildsRotation(1, rotationYoffset/2, false);

    		fallenWaterAnim.Play();
            fallenWaterAnim.Clear();
    	}
    }

    public void PlayComboPeriodAnim()
    {
        float centerPosY = GameControl.instance.columnObj.GetCenterPostion();
        var sh = comboPeriodAnim.shape;
        sh.shapeType = ParticleSystemShapeType.Box;
        sh.position = new Vector3(0, centerPosY, 0);
        sh.scale = (new Vector3(1, GameControl.instance.columnObj.transform.childCount, 0));

        if(!comboPeriodAnim.isPlaying)
        {
            comboPeriodAnim.Clear();
            comboPeriodAnim.Play();
        }
    }

    public void StopComboPeriodAnim()
    {
    	comboPeriodAnim.Stop();
    }
}
