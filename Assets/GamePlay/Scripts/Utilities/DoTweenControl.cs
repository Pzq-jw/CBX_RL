using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenControl : MonoBehaviour
{

	private float absDeltaX;

	public void FallenAnimation(int fallenSide)
	{
		Vector3 targetJump = GameControl.instance.seaLevel;
		 		targetJump -= new Vector3(-4 * fallenSide, 0, 0);
		Vector3 targetRotation = new Vector3(0, -270 * fallenSide, 35);

		transform.DORotate(targetRotation, 2);
		transform.DOJump(targetJump, 2f, 1, 2).SetEase(Ease.Linear).OnComplete(OnJumpComplete);
	}

	private void OnJumpComplete()
	{
		transform.rotation = Quaternion.identity;
	}

	public void StackingNoDeadCenterAnimation(int fallenSide, bool isDeadCenter)
	{
		if(isDeadCenter)
			return;

		Vector3 pivot = GameControl.instance.mycolObj.topPieceLocalPos;
		pivot.x += 0.5f * fallenSide;
		pivot.y += 0.5f;
		// Debug.Log("TopPieceCorenerPos = " + pos.ToString("F4"));
		var go = new GameObject();
		// Debug.Log("new GameObject.position = " + go.transform.position.ToString("F4"));
		// Debug.Log("new GameObject.localPosition = " + go.transform.localPosition.ToString("F4"));
		go.transform.SetParent(GameControl.instance.columnObj.transform, true);
		// Debug.Log("new GameObject.position = " + go.transform.position.ToString("F4"));		
		// Debug.Log("new GameObject.localPosition = " + go.transform.localPosition.ToString("F4"));
		go.transform.localPosition = pivot;
		// Debug.Log("new GameObject.position = " + go.transform.position.ToString("F4"));		
		// Debug.Log("new GameObject.localPosition = " + go.transform.localPosition.ToString("F4"));
		// Debug.Log("TopPoece.position = " + transform.position.ToString("F4"));		
		// Debug.Log("TopPoece.localPosition = " + transform.localPosition.ToString("F4"));
		transform.SetParent(go.transform, true);
		// Debug.Log("TopPiece.position = " + transform.position.ToString("F4"));		
		// Debug.Log("TopPiece.localPosition = " + transform.localPosition.ToString("F4"));
		float rotateAngle = -Mathf.Asin(absDeltaX) * Mathf.Rad2Deg;
		// Debug.Log("rotate angle = " + rotateAngle + "  absDeltaX = " + absDeltaX, gameObject);
		rotateAngle -= GameControl.instance.columnObj.transform.rotation.z * Mathf.Rad2Deg * 2;
		// Debug.Log("rotate angle = " + rotateAngle + "  column z = " + GameControl.instance.columnObj.transform.rotation.z * Mathf.Rad2Deg * 2, gameObject);

		Sequence stackAnimSequence = DOTween.Sequence();
		Tween t = go.transform.DOLocalRotate(new Vector3(0,0,rotateAngle * fallenSide), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.OutExpo);
		stackAnimSequence.Append(t);
		Tween t1 = go.transform.DOLocalRotate(new Vector3(0,0,rotateAngle * -fallenSide), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InCubic);
		stackAnimSequence.Append(t1);
		stackAnimSequence.AppendCallback(OnStackingNoDeadCenterAnimationEnd);
		// go.transform.DOPunchRotation(new Vector3(0,0,rotateAngle * fallenSide), 0.7f, 0, 1f).SetEase(Ease.OutBack).OnComplete(OnStackingNoDeadCenterAnimationEnd);

	}

	private void OnStackingNoDeadCenterAnimationEnd()
	{
		GameObject go = transform.parent.gameObject;
		transform.SetParent(GameControl.instance.columnObj.transform, true);
		Destroy(go);
	}

	public void GetDeltaXFromCollision(float absDeltaX)
	{
		this.absDeltaX = absDeltaX;
	}
}
