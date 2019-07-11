﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GO_Extensions;

public class GameControl : MonoBehaviour
{

	public static GameControl instance;
    public Rigidbody2D sparse_test;

	public PiecePool piecePoolObj;
    public ColumnSwinging columnObj;
    public ScreenMoveUp screenMoveUpObj;
    public EllipticalOrbit slingObj;
    public ComboControl comboControlObj;
    public DoTweenControl doTweenObj;
    public ParticleControl particleObj;
    public MyCollisionControl mycolObj;
    public CBXPieceAgent agentObj;
    public CBXSaverAgent saverAgentObj;
    public CBXAcademy academyObj;

    public Text scoreText;
    public Text missText;
    public GameObject gameOverText;
    public GameObject columnGameObj;

    public int populationScore = 0;
    public int stackedPieceNum = 0;
    public int missNum = 0;
    public Vector3 seaLevel;

    public enum GameStatus
    {
        GAME_READY = 0,  // Scene has been loaded, but game not start
        GAME_START, // Game start, but first piece not fallen
        GAME_RUNNING, // Game is running but not combo, the first piece has fallen
        GAME_COMBO, // Game is running and in combo phase
        GAME_OVER // Game is over.
    }
    public GameStatus gameStatus;

    public bool isGameRunning
    {
        get {return (gameStatus == GameStatus.GAME_RUNNING 
            || gameStatus == GameStatus.GAME_COMBO);}
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(!instance)
        {
        	instance = this;
        }else if(instance)
        {
        	Destroy(gameObject);
        }
    }

    void Start()
    {
        seaLevel = new Vector3(0, -10, 0);
        gameStatus = GameStatus.GAME_START;
    }

    void Update()
    {
        if(gameStatus == GameStatus.GAME_OVER 
            && (Input.GetKeyDown("space") || Input.touchCount > 0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            gameStatus = GameStatus.GAME_START; 
        }
    }

    public void OnPieceStacking()
    {
        PieceStacked();
    }

    public void AfterPieceStackingSuccessfully(bool isDeadCenter)
    {
        // CheckFirstPieceIfStacked();
        // Scored();
        // ScreenMoveUp();
        // SetColumnSwinging();
        // seaLevel.y++;

        if(isDeadCenter)
        {
            gameStatus = GameStatus.GAME_COMBO;
            agentObj.ComputeReward();
            agentObj.Done();
            agentObj.isJustCalledDone = true;
            agentObj.deadcenterCount++;
            // comboControlObj.Combo();
            // columnObj.Set2ComboSwingingAmplitude();
            // particleObj.PlayComboPeriodAnim();
        }
        else
        {
            agentObj.ComputeReward();
            agentObj.Done();
            agentObj.isJustCalledDone = true;
        }
        // else if(gameStatus == GameStatus.GAME_COMBO)
        // {
        //     comboControlObj.AddComboNum();            
        // }
    }

    public void AfterPieceStackingFailed(int fallenSide)
    {
        agentObj.ComputeReward();
        agentObj.Done();
        agentObj.isJustCalledDone = true;
        // Missed();
        // CheckMissNum();
        // screenMoveUpObj.ShakeCamera();
    }

    void PieceStacked()
    {
    	// piecePoolObj.HookNewPiece();
    }

    void Scored()
    {
        stackedPieceNum++;
        populationScore++;
        scoreText.text = "Score:" + populationScore.ToString();
    }

    void Missed()
    {
        missNum++;
        missText.text = "Miss:" + missNum.ToString();
    }

    void CheckMissNum()
    {
        if(missNum > 2)
        {
            gameOverText.SetActive(true);
            if(gameStatus == GameStatus.GAME_COMBO)
            {
                comboControlObj.comboTimer.EndTiming();
                comboControlObj.comboScored();
            }
            gameStatus = GameStatus.GAME_OVER;
        }
    }

    void ScreenMoveUp()
    {
        screenMoveUpObj.MoveUp();
    }

    void CheckFirstPieceIfStacked()
    {
        if(gameStatus == GameStatus.GAME_START)
        {
            gameStatus = GameStatus.GAME_RUNNING;
        }
    }

    void SetColumnSwinging()
    {
        columnObj.SwingingCenterMoveUp();
        columnObj.AddAmplitudeRotate();
        columnObj.SetAmplitudeIncrementAndMax();
    }

    // Use this to control the ideal distance
    // void LockDistanceColumn2Sling()
    // {
    //     Debug.Log(columnObj.GetDistanceColumn2Sling);
    //     if(!Mathf.Approximately(columnObj.initialDistance, 
    //         columnObj.GetDistanceColumn2Sling))
    //         {
    //             columnObj.transform.position = slingObj.transform.position 
    //                 - new Vector3(0, columnObj.initialDistance - slingObj.offsetY, 0);
    //         }  
    // }
















}
