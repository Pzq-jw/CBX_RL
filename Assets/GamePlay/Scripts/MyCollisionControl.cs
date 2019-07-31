using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCollisionControl : MonoBehaviour
{

    public Collider2D dropPieceCol;
    public Collider2D topPieceCol;

    // public Vector3 dropPiecePos;
    // public Vector3 topPiecePos;
    public float deltaX;

    public void SetCollisionInfo(Collision2D ctl)
    {
        dropPieceCol = ctl.otherCollider;
        topPieceCol = ctl.collider;
        deltaX = dropPieceLocalPos.x - topPieceLocalPos.x;
        // LogCollidersPos();
    }

    public Vector3 dropPieceLocalPos
    {
        get {return dropPieceCol ? dropPieceCol.transform.localPosition : Vector3.zero;}
    }

    public Vector3 topPieceLocalPos
    {
        get {return topPieceCol ? topPieceCol.transform.localPosition : Vector3.zero;}
    }

    public void GetColumnHeightIncrement()
    {
        float topPiecePosY = dropPieceLocalPos.y;
        float dropPiecePosY = topPieceLocalPos.y;
        GameControl.instance.columnObj.columnHeightIncrement = Mathf.Abs(dropPiecePosY - topPiecePosY);
    }

    public void LogCollidersPos()
    {
        Debug.Log("dropPieceCol-" + dropPieceCol.name + " pos = " + 
            dropPieceCol.transform.localPosition.ToString("F3"), gameObject);
        Debug.Log("topPieceCol-" + topPieceCol.name + " pos = " + 
            topPieceCol.transform.localPosition.ToString("F3"), gameObject);
        Debug.Log("deltaX = " + deltaX);
    }
}
